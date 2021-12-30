module Cribs.Handlers.Auth
open Giraffe
open FSharp.Control.Tasks
open TODO.Server.Types.Auth

let login: HttpHandler = fun next ctx -> task{
  let! creds = ctx.BindJsonAsync<Credentials>()
  return! Successful.OK creds next ctx
}