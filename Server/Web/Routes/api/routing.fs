module Cribs.Web.API.Routing

open Giraffe
open FSharp.Control.Tasks


let routes: HttpHandler =
  choose [
      choose [
            route "/"  
            route "/hello/"            ]
      subRoute "/api"
      <| choose [
              subRoute "/users"
                  <| choose [
                      route "/register" >=> Cribs.Handlers.Users.userRegister 
                      GET >=> route "/get" >=> Cribs.Handlers.Users.getUserByUsername
                    ]
              subRoute "/v2"
                  <| choose [
                      route "/foo" >=> text "Foo 2"
                      route "/bar" >=> text "Bar 2" ] ]]

// let routes: HttpHandler =
//     choose [
//         GET >=>
//             choose [
//                 route "/"  
//                 route "/hello/"            ]
//         setStatusCode 404 >=> text "Not Found" ]
