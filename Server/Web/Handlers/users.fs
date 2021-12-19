module Cribs.Handlers.Users

open System
open FSharp.Control.Tasks
open System.Linq
open FSharp.Data
open Giraffe


let userRegister: HttpHandler = 
  handleContext(
    fun ctx ->
      task{
        let! (user:Cribs.Types.Users.User) = ctx.BindModelAsync<Cribs.Types.Users.User>()
        let db = ctx.GetService<Cribs.Util.DB.IConnectionFactory>()
        let! res = db.WithConnection <| fun conn -> async {
          let! storedUser = Cribs.DAL.User.storeUser conn user
          return storedUser
        }
        return! ctx.WriteJsonAsync user
      }) 

let getUserByUsername: HttpHandler = 
  handleContext (
    fun ctx ->
      task{
        let db = ctx.GetService<Cribs.Util.DB.IConnectionFactory>()
        let! temp = ctx.BindJsonAsync<string>()
        let! res = db.WithConnection <| fun conn -> async {
          let! retrievedUser = Cribs.DAL.User.getUser conn temp
          return Seq.tryExactlyOne retrievedUser
        }
        return! ctx.WriteJsonAsync res
      }
  )