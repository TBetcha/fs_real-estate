module Cribs.Handlers.Houses

open FSharp.Control.Tasks
open Giraffe


let earlyReturn: HttpFunc = Some >> System.Threading.Tasks.Task.FromResult


let addHouse (x: string) : HttpHandler =
  fun next ctx ->
    task {
      let! temp = ctx.BindModelAsync<Cribs.Types.House.House>()
      let dal = ctx.GetService<Cribs.DAL.House.IHouseRepo>()
      let db = ctx.GetService<Cribs.Util.DB.IConnectionFactory>()

      let! res =
        db.WithConnection
        <| fun conn ->
          async {
            let! user = ctx.GetService<Cribs.DAL.User.IUserRepo>().getUser conn x
            let! x = dal.addHouse conn temp user.Id
            return x
          }

      match res with
      | 1 -> return! Successful.OK res next ctx
      | _ -> return failwith "no"
    }
