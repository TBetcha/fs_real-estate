module Cribs.Handlers.Users

open System
open FSharp.Control.Tasks
open System.Linq
open FSharp.Data
open Giraffe
open Cribs.Types.Users

let userRegister: HttpHandler = 
  handleContext(
    fun ctx ->
      task{
        let! (user:Cribs.Types.Users.User) = ctx.BindModelAsync<Cribs.Types.Users.User>()
        let db = ctx.GetService<Cribs.Util.DB.IConnectionFactory>()
        let dal = ctx.GetService<Cribs.DAL.User.IUserRepo>()
        let! res = db.WithConnection <| fun conn -> async {
          let! storedUser = dal.storeUser conn user
          return storedUser
        }
        return! ctx.WriteJsonAsync user
      }) 

let getUserByUsername : HttpHandler = 
  handleContext (
    fun ctx ->
      task{
        let db = ctx.GetService<Cribs.Util.DB.IConnectionFactory>()
        let dal = ctx.GetService<Cribs.DAL.User.IUserRepo>()
        let! temp = ctx.BindJsonAsync<userQuery>()
        let! res = db.WithConnection <| fun conn -> async {
          let! retrievedUser = dal.getUserByUsername conn temp.username
          return  retrievedUser
        }
        return! ctx.WriteJsonAsync res
      }
  )

//test
// let getStuff : HttpHandler = 
//   handleContext(
//     fun ctx -> 
//       task{
//         let db = ctx.GetService<Cribs.Util.DB.IConnectionFactory>()
//         let dal = ctx.GetService<Cribs.DAL.User.IUserRepo>()
//         let! res = db.WithConnection <| fun conn -> async {
//           let! u = dal.getStuff conn "sdf"
//           return u
//         }
//         return! ctx.WriteJsonAsync res
//       }
//   )