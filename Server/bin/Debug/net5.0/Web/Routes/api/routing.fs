module Todo.Web.API.Routing

open Giraffe

let routes: HttpHandler =
  choose
    [ GET >=> choose [ route "/"; route "/hello/" ]
      setStatusCode 404 >=> text "Not Found" ]
