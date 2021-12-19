module Cribs.DAL.User
open Npgsql
open BCrypt.Net
open Cribs.Types.Users
open Cribs.Types.House

let storeUser (conn:NpgsqlConnection) (user:'T) = async {
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

let getUser (conn: NpgsqlConnection) (user: string):Async<User list>  = async  {
  use command = conn.CreateCommand()
  command.CommandText <- "
  SELECT
    id, first_name, last_name, username, password
  FROM users
  WHERE 1=1
    AND username = @user;
"
  ignore <| command.Parameters.Add(command.CreateParameter(ParameterName="user", Value=user))
  // let dataReader = command.ExecuteReader()
  //   seq {
  //       while dataReader.Read() do
  //           yield [for i in [0..dataReader.FieldCount-1] -> dataReader.[i]]
  //   } |> ignore
  use reader = command.ExecuteReader()
  return  [
      let record = Cribs.Util.DB.readerToDict reader
      yield {
        Id = record.getString("id") 
        Username = record.getString("username") 
        First_name = record.getString("first_name") 
        Last_name = record.getString("last_name") 
        Password = record.getString("password") 
      }
  ]
} 

// let addHouse (conn: NpgsqlConnection) (house:'T) (user:'A)= async {
//   use command = conn.CreateCommand()
//   command.CommandText <- "
//     INSERT INTO todos (
//       id, title, content, completed, user_id
//     ) VALUES (
//       @id, @title, @content, @completed, @user_id
//     )
//   "
//   ignore <| command.Parameters.Add(command.CreateParameter(ParameterName="id", Value=System.Guid.NewGuid()))
//   ignore <| command.Parameters.Add(command.CreateParameter(ParameterName="title", Value=todo.Title))
//   ignore <| command.Parameters.Add(command.CreateParameter(ParameterName="content", Value=todo.Content))
//   ignore <| command.Parameters.Add(command.CreateParameter(ParameterName="completed", Value=todo.Completed))
//   ignore <| command.Parameters.Add(command.CreateParameter(ParameterName="user_id", Value=todo.User_id))
//   ignore <| command.ExecuteNonQuery()
// }