module Cribs.Handlers.Houses
open System
open FSharp.Control.Tasks
open System.Linq
open FSharp.Data
open Giraffe


let earlyReturn : HttpFunc = Some >> System.Threading.Tasks.Task.FromResult


let addHouse : HttpHandler = fun  next ctx -> task {
  let! temp = ctx.BindModelAsync<Cribs.Types.House.House>()
  let dal = ctx.GetService<Cribs.DAL.House.IHouseRepo>()
  let db = ctx.GetService<Cribs.Util.DB.IConnectionFactory>()
  let! res = db.WithConnection <| fun conn  -> async {
    let! x = dal.addHouse conn temp
    return x
  }
  match res with 
  | 1  -> return! Successful.OK res next ctx
  | _ -> return failwith "no" 
} 