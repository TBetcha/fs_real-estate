module Todo.DAL.User
open Npgsql

let storeUser (conn:NpgsqlConnection) (user:Todo.Util.Types.User) = async {
  let command = conn.CreateCommand()
  command.CommandText <- "
    INSERT INTO users (
      id, first_name, last_name
    ) VALUES (
      @id, @first_name, @last_name
    )"
  
  ignore <| command.Parameters.Add(command.CreateParameter(ParameterName="id", Value=System.Guid.NewGuid()))
  ignore <| command.Parameters.Add(command.CreateParameter(ParameterName="first_name", Value=user.First_name))
  ignore <| command.Parameters.Add(command.CreateParameter(ParameterName="last_name", Value=user.Last_name))
  ignore <| command.ExecuteNonQuery()
  

}
