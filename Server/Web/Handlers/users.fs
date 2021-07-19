module Todo.Handlers.Users

open FSharp.Control.Tasks
open Giraffe

let userRegister: HttpHandler = 
  handleContext(
    fun ctx ->
      task{
        let! (user:Todo.Util.Types.User) = ctx.BindModelAsync<Todo.Util.Types.User>()
        sprintf "printing stuff %s" user.First_name |> ignore<string>
        return! ctx.WriteTextAsync user.First_name
        // let db = ctx.GetService<Todo.Util.DB.ConnectionFactory>()
        // db.
    }) 
  
  // let db = ctx.GetService<IConnectionFactory>()
  // db.