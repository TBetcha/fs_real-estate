module Cribs.Handlers.Houses
open System
open FSharp.Control.Tasks
open System.Linq
open FSharp.Data
open Giraffe


let earlyReturn : HttpFunc = Some >> System.Threading.Tasks.Task.FromResult


let addHouse (x:string) : HttpHandler = fun  next ctx -> task {
  let! temp = ctx.BindModelAsync<Cribs.Types.House.House>()
  let dal = ctx.GetService<Cribs.DAL.House.IHouseRepo>()
  let db = ctx.GetService<Cribs.Util.DB.IConnectionFactory>()
  let! res = db.WithConnection <| fun conn  -> async {
    let! user = ctx.GetService<Cribs.DAL.User.IUserRepo>().getUserByUsername conn x
    match user with 
    | Some y ->
      let! x = dal.addHouse conn temp y
      return x
    | None -> return failwith "Cannot find user"
  }
  match res with 
  | 1  -> return! Successful.OK res next ctx
  | _ -> return failwith "no" 
} 