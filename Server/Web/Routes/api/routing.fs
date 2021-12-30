module Cribs.Web.API.Routing
open Cribs.Server.Util.Token

open Giraffe
open FSharp.Control.Tasks


let routes: HttpHandler =
  choose [
    choose [
      POST >=> route "/token" >=> handlePostToken
      POST >=> route "/login" >=> Cribs.Handlers.Auth.login
      route "/secured" >=> authorize >=> handleGetSecured ]
    subRoute "/api"
    <| choose [
        subRoute "/users"
          <| choose [
            route "/register" >=> Cribs.Handlers.Users.userRegister 
            route "/getuser" >=> Cribs.Handlers.Users.getUserByUsername
          ]
        subRoute "/houses"
        <| choose [
          POST >=> routef  "/%s/add"  Cribs.Handlers.Houses.addHouse
          GET >=> route "/bar" >=> text "ok...not really" ] ]]

// let routes: HttpHandler =
//     choose [
//         GET >=>
//             choose [
//                 route "/"  
//                 route "/hello/"            ]
//         setStatusCode 404 >=> text "Not Found" ]