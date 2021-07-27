module Todo.Handlers.Users

open System
open FSharp.Control.Tasks
open System.Linq
open FSharp.Data
open Giraffe

let userRegister: HttpHandler = 
  handleContext(
    fun ctx ->
      task{
        let! (user:Todo.Util.Types.User) = ctx.BindModelAsync<Todo.Util.Types.User>()
        sprintf "printing stuff %s" user.First_name |> ignore<string>
       // return! ctx.WriteTextAsync user.First_name
        let db = ctx.GetService<Todo.Util.DB.IConnectionFactory>()
        let! res = db.WithConnection <| fun conn -> async {
          let! storedUser = Todo.DAL.User.storeUser conn user 
          return storedUser
        }
         return! ctx.WriteJsonAsync user
        }) 

let getUserByUsername (user:string): HttpHandler = 
  handleContext (
    fun ctx ->
      task{
        let db = ctx.GetService<Todo.Util.DB.IConnectionFactory>()
        let! res = db.WithConnection <| fun conn -> async {
          let! retrievedUser = Todo.DAL.User.getUser conn user
          return Seq.tryExactlyOne retrievedUser
        }
        return! ctx.WriteJsonAsync res
      }
  )