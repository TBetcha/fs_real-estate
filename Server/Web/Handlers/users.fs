module Cribs.Handlers.Users

open FSharp.Control.Tasks
open Giraffe
open Cribs.Types.Users

let userRegister: HttpHandler =
  handleContext (fun ctx ->
    task {
      let! (user: Cribs.Types.Users.User) = ctx.BindModelAsync<Cribs.Types.Users.User>()
      let db = ctx.GetService<Cribs.Util.DB.IConnectionFactory>()
      let dal = ctx.GetService<Cribs.DAL.User.IUserRepo>()

      let! res =
        db.WithConnection
        <| fun conn ->
          async {
            let! storedUser = dal.storeUser conn user
            return storedUser
          }
      
      match res with 
      | Ok _ -> return! ctx.WriteJsonAsync user
      | Error e -> 
        ctx.SetStatusCode 400
        return! ctx.WriteJsonAsync e
    })

let getUserByUsername: HttpHandler =
  handleContext (fun ctx ->
    task {
      let db = ctx.GetService<Cribs.Util.DB.IConnectionFactory>()
      let dal = ctx.GetService<Cribs.DAL.User.IUserRepo>()
      let! temp = ctx.BindJsonAsync<userQuery>()

      let! res =
        db.WithConnection
        <| fun conn ->
          async {
            let! retrievedUser = dal.getUser conn temp.username
            return retrievedUser
          }

      return! ctx.WriteJsonAsync res
    })

// test for dapper
// let getStuff : HttpHandler =
//   handleContext(
//     fun ctx ->
//       task{
//         let db = ctx.GetService<Cribs.Util.DB.IConnectionFactory>()
//         let dal = ctx.GetService<Cribs.DAL.User.IUserRepo>()
//         let! res = db.WithConnection <| fun conn -> async {
//             let! u = dal.getItem
//           return u
//         }
//         return! ctx.WriteJsonAsync res
//       }
//   )
