module Cribs.DAL.House

open Npgsql
open Cribs.Types.Users
open Cribs.Types.House

type IHouseRepo () = 

  member _.addHouse (conn: NpgsqlConnection) (house:House) (userId:System.Guid)= async {
    use command = conn.CreateCommand()
    command.CommandText <- "
      INSERT INTO houses (
        id, user_id, bedroom, bathroom, square_feet, street_number, city, state, zip
      ) VALUES (
        @id, @user_id, @bedroom, @bathroom, @square_feet, @street_number, @city, @state, @zip
      )
    "
    ignore <| command.Parameters.Add(command.CreateParameter(ParameterName="id", Value=System.Guid.NewGuid()))
    ignore <| command.Parameters.Add(command.CreateParameter(ParameterName="user_id", Value=userId))
    ignore <| command.Parameters.Add(command.CreateParameter(ParameterName="bedroom", Value=house.bedroom))
    ignore <| command.Parameters.Add(command.CreateParameter(ParameterName="bathroom", Value=house.bathroom))
    ignore <| command.Parameters.Add(command.CreateParameter(ParameterName="square_feet", Value=house.squareFeet))
    ignore <| command.Parameters.Add 
        (command.CreateParameter (ParameterName="street_number", Value=house.address.streetNum))
    ignore <| command.Parameters.Add(command.CreateParameter(ParameterName="city", Value=house.address.city))
    ignore <| command.Parameters.Add(command.CreateParameter(ParameterName="state", Value=house.address.state))
    ignore <| command.Parameters.Add(command.CreateParameter(ParameterName="zip", Value=house.address.zip))
    return command.ExecuteNonQuery()
  }
