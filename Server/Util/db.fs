module Cribs.Util.DB

open Npgsql
open Npgsql.TypeMapping

type DbValue
  = DbGuid of System.Guid
  | DbDate of NodaTime.LocalDate
  | DbInt of int
  | DbString of string
  | DbBool of bool
  | DbDateTime of NodaTime.LocalDateTime
  | DbNull

type private Paramaters = System.Collections.Generic.IDictionary<string, obj>

let taskToAsync<'a> : System.Threading.Tasks.Task<'a> -> Async<'a> = Async.AwaitTask
let taskToAsync' : System.Threading.Tasks.Task -> Async<unit> = Async.AwaitTask

let readerToDict (reader:System.Data.Common.DbDataReader) = 
  let mutable prefix = ""
  let mapping = dict <| [
    for i in [0..reader.FieldCount-1] do
      match reader.GetName(i) with
      | "__prefix" -> prefix <- reader.GetString(i)
      |x -> yield (prefix+x,i)
  ]
  {|
    getGuid=fun k -> reader.GetFieldValue<System.Guid>(mapping.[k])
    getDate=fun k -> reader.GetFieldValue<NodaTime.LocalDate>(mapping.[k])
    getDateTime=fun k -> reader.GetFieldValue<NodaTime.ZonedDateTime>(mapping.[k])
    getDateTimeN=fun k -> if reader.IsDBNull(mapping.[k]) then None else Some (reader.GetFieldValue<NodaTime.ZonedDateTime>(mapping.[k]))
    getInt=fun k -> reader.GetInt64(mapping.[k])
    getUInt=fun k -> reader.GetInt64(mapping.[k]) |> uint64
    getUIntN=fun k -> if reader.IsDBNull(mapping.[k]) then None else Some (reader.GetInt64(mapping.[k]) |> uint64)
    getDouble=fun k -> reader.GetDouble(mapping.[k]) |> double
    getDoubleN=fun k ->if reader.IsDBNull(mapping.[k]) then None else Some (reader.GetDouble(mapping.[k]) |> double)
    getString=fun k -> reader.GetString(mapping.[k])
    getStringN=fun k -> if reader.IsDBNull(mapping.[k]) then None else Some (reader.GetString(mapping.[k]))
  |}


let private _executeNonQuery (connection:NpgsqlConnection) (query:string) (parameters:Paramaters): int Async = async {
  use command = connection.CreateCommand() 
  command.CommandText <- query 
  for kvp in parameters do 
    ignore<NpgsqlParameter> (command.Parameters.AddWithValue(kvp.Key,kvp.Value))
  return! taskToAsync (command.ExecuteNonQueryAsync())

}


type IConnectionFactory = 
  abstract member WithConnection : (NpgsqlConnection -> 'a Async ) -> 'a Async
  abstract member WithTransaction : (NpgsqlTransaction -> 'a Async ) -> 'a Async

type ConnectionFactory(connectionString:string) =
  member _.WithTransaction f = async {
    use connection = new NpgsqlConnection(connectionString)
    do! taskToAsync' (connection.OpenAsync())
    ignore<INpgsqlTypeMapper>(connection.TypeMapper.UseNodaTime())
    let transaction = connection.BeginTransaction()
    try 
      let! result = f transaction
      do! taskToAsync' (transaction.CommitAsync())
      return result
    with
    | :? NpgsqlException as e ->
      printfn "%s %s" e.Message e.StackTrace 
      do! taskToAsync' (transaction.RollbackAsync())
      return raise e
  }

  member this.WithConnection f = this.WithTransaction <| fun transaction -> f transaction.Connection

  interface IConnectionFactory with
    override this.WithConnection f = this.WithConnection f
    override this.WithTransaction f = this.WithTransaction f
    