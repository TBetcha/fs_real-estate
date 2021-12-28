module Cribs.DAL.User
open Npgsql
open BCrypt.Net
open Cribs.Types.Users
open Cribs.Types.House
open Dapper



let querySingleAsync<'a> (conn:NpgsqlConnection) command param = async {
    
    let! result = conn.QuerySingleOrDefaultAsync<'a>(command, param) |> Async.AwaitTask
    if isNull (box result) then
        return None
    else
        return Some result
}

let inline (=>) a b = a, box b

type IUserRepo() = 

  member _.storeUser (conn:NpgsqlConnection) (user:'T) = async {
    let command = conn.CreateCommand()
    command.CommandText <- "
      INSERT INTO users (
        id, username, password, first_name, last_name
      ) VALUES (
        @id, @username, @password, @first_name, @last_name
      )
    "
    
    ignore <| command.Parameters.Add(command.CreateParameter(ParameterName="id", Value=System.Guid.NewGuid()))
    ignore <| command.Parameters.Add(command.CreateParameter(ParameterName="username", Value=user.Username))
    ignore <| command.Parameters.Add(command.CreateParameter(ParameterName="password", Value=BCrypt.HashPassword user.Password))
    ignore <| command.Parameters.Add(command.CreateParameter(ParameterName="first_name", Value=user.First_name))
    ignore <| command.Parameters.Add(command.CreateParameter(ParameterName="last_name", Value=user.Last_name))
    ignore <| command.ExecuteNonQuery()
  }

  member _.getUser (conn: NpgsqlConnection) (user: string):Async<'T>  = async  {
    use command = conn.CreateCommand()
    command.CommandText <-"
      SELECT
        id, first_name, last_name, username, password
      FROM users
      WHERE 1=1
        AND username = @user;
    "
    ignore <| command.Parameters.Add(command.CreateParameter(ParameterName="user", Value=user))
    use reader = command.ExecuteReader()
    let user = List.tryExactlyOne<|[
      while reader.Read() do
        let record = Cribs.Util.DB.readerToDict reader
        yield {|
          Id = record.getGuid("id")
          Username = record.getString("username") 
          First_name = record.getString("first_name") 
          Last_name = record.getString("last_name") 
        |}
    ]
    match  user  with
    | Some  x -> return x
    | None -> return! failwith "No user found" 
  } 

  // Dapper 
  // member _.getUserByUsername  (conn:NpgsqlConnection) name= 
  //   async {
  //       let username = name
  //       let command = "SELECT * from users where username = @username"
  //       let param = dict ["username", box username]
  //       return!  querySingleAsync conn command param
  //   }
