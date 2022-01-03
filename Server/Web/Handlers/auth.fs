module Cribs.Handlers.Auth
open Giraffe
open FSharp.Control.Tasks
open TODO.Server.Types.Auth
open BCrypt.Net

let login: HttpHandler = fun next ctx -> task{
  let db = ctx.GetService<Cribs.Util.DB.IConnectionFactory>()
  let! creds = ctx.BindJsonAsync<Credentials>()
  let! res = db.WithConnection <| fun conn -> async {
    let! retrievedUser = ctx.GetService<Cribs.DAL.User.IUserRepo>().getUserPassword conn creds.email
    match BCrypt.Verify (creds.password, retrievedUser) with
    | false -> return! failwith "Invalid credentials"
    | true  -> None |> ignore
  }
  return! Successful.OK  "you did it" next ctx
}