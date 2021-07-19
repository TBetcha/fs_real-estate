module Todo.Web.API.Routing

open Giraffe


let routes: HttpHandler =
    choose [
        GET >=>
            choose [
                route "/"  
                route "/hello/"            ]
        subRoute "/api"
            (choose [
                subRoute "/users"
                    (choose [
                        route "/register" >=> Todo.Handlers.Users.userRegister 
                        route "/bar" >=> text "Bar 1" ])
                subRoute "/v2"
                    (choose [
                        route "/foo" >=> text "Foo 2"
                        route "/bar" >=> text "Bar 2" ]) ])]

// let routes: HttpHandler =
//     choose [
//         GET >=>
//             choose [
//                 route "/"  
//                 route "/hello/"            ]
//         setStatusCode 404 >=> text "Not Found" ]
