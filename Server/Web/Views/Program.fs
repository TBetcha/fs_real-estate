module Todo.Program

open System
open System.IO
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Configuration
open Giraffe
//open Giraffe.Serialization.Json
open Npgsql
open NodaTime.Serialization.JsonNet

// ---------------------------------
// Models
// ---------------------------------

type Message =
  {
      Text : string
  }

// ---------------------------------
// Views
// ---------------------------------

// module Views =
//     open Giraffe.ViewEngine

//     let layout (content: XmlNode list) =
//         html [] [
//             head [] [
//                 title []  [ encodedText "todo" ]
//                 link [ _rel  "stylesheet"
//                        _type "text/css"
//                        _href "/main.css" ]
//             ]
//             body [] content
//         ]

//     let partial () =
//         h1 [] [ encodedText "todo" ]

//     let index (model : Message) =
//         [
//             partial()
//             p [] [ encodedText model.Text ]
//         ] |> layout

// ---------------------------------
// Web app
// ---------------------------------

// let indexHandler (name : string) =
//     let greetings = sprintf "Hello %s, from Giraffe!" name
//     let model     = { Text = greetings }
//     let view      = Views.index model
//     htmlView view

// ---------------------------------
// Error handler
// ---------------------------------

let errorHandler (ex : Exception) (logger : ILogger) =
  logger.LogError(ex, "An unhandled exception has occurred while executing the request.")
  clearResponse >=> setStatusCode 500 >=> text ex.Message

// ---------------------------------
// Config and Main
// ---------------------------------

// let configureSerialization (serives:IServiceCollection) : IServiceCollection =
//   let serializer = NewtonsoftJsonSerializer.DefaultSettings
//   serializer.Converters.Add(NodaPatternConverter<NodaTime.ZonedDateTime>(NodaTime.Text.ZonedDateTimePattern.CreateWithInvariantCulture("yyyy-MM-ddTHH:mm:sso<g>",NodaTime.DatetimeZoneProvider.Tzdb)))

let configureAppConfiguration (context:WebHostBuilderContext)(config:IConfigurationBuilder) = 
  config 
    .AddJsonFile("appsettings.json",false,true)  
    .AddJsonFile(sprintf "appsettings.%s.json" context.HostingEnvironment.EnvironmentName, true)
    .AddEnvironmentVariables() |> ignore

let configureCors (builder : CorsPolicyBuilder) =
  builder
      .WithOrigins(
          "http://localhost:5000",
          "https://localhost:5001")
      .AllowAnyMethod()
      .AllowAnyHeader()
      |> ignore

let configureApp (app : IApplicationBuilder) =
  let env = app.ApplicationServices.GetService<IWebHostEnvironment>()
  (match env.IsDevelopment() with
  | true  ->
      app.UseDeveloperExceptionPage()
  | false ->
      app .UseGiraffeErrorHandler(errorHandler)
          .UseHttpsRedirection())
      .UseCors(configureCors)
      .UseStaticFiles()
      .UseGiraffe(Todo.Web.API.Routing.routes)

let configureServices (services : IServiceCollection) : unit=
  let config = services.BuildServiceProvider().GetRequiredService<IConfiguration>()
  services.AddCors()    |> ignore
  services.AddGiraffe() |> ignore
  // let serializer = NewtonsoftJsonSerializer.DefaultSettings
  // serializer.Converters.Add(NodaPatternConverter<NodaTime.ZonedDateTime>(NodaTime.Text.ZonedDateTimePattern.CreateWithInvariantCulture("yyyy-MM-ddTHH:mm:sso<g>",NodaTime.DatetimeZoneProvider.Tzdb)))
  ignore <| NpgsqlConnection.GlobalTypeMapper.UseNodaTime()
  ignore <|services
     .AddTransient<Todo.Util.DB.IConnectionFactory>(fun _ -> 
      Todo.Util.DB.ConnectionFactory(config.GetConnectionString("connectionString")) :>_
    )


let configureLogging (builder : ILoggingBuilder) =
  builder.AddConsole()
          .AddDebug() |> ignore

[<EntryPoint>]
let main args =
  let contentRoot = Directory.GetCurrentDirectory()
  let webRoot     = Path.Combine(contentRoot, "Web")
  Host.CreateDefaultBuilder(args)
      .ConfigureWebHostDefaults(
          fun webHostBuilder ->
              webHostBuilder
                  .UseContentRoot(contentRoot)
                  .UseWebRoot(webRoot)
                  .ConfigureAppConfiguration(System.Action<WebHostBuilderContext,IConfigurationBuilder> configureAppConfiguration)
                  .Configure(Action<IApplicationBuilder> configureApp)
                  .ConfigureLogging(configureLogging)
                  .ConfigureServices(configureServices)
                  |> ignore)
      .Build()
      .Run()
  0



























