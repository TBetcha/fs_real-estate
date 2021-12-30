module Cribs.Server.Util.Token

open System
open System.Text
open System.Security.Claims
open System.IdentityModel.Tokens.Jwt
open System.Xml
open Microsoft.AspNetCore.Http
open Microsoft.IdentityModel.Protocols
open Microsoft.IdentityModel.Tokens
open Microsoft.Extensions.Configuration
open Giraffe
open Microsoft.AspNetCore.Authentication.JwtBearer
open TODO.Server.Types.Auth

let secret = "spadR2dre#u-ruBrE@TepA&*Uf@U"
//let config (config:IConfigurationBuilder) = config.AddJsonFile("appsettings.json",false,true)

//!! Fix this secret
//let secret = Configuration.<string>(config, "Secret")

let authorize: HttpFunc -> HttpContext -> HttpFuncResult = requiresAuthentication (challenge JwtBearerDefaults.AuthenticationScheme)

let generateToken email =
  let claims =
    [| Claim(JwtRegisteredClaimNames.Sub,email)
       Claim(JwtRegisteredClaimNames.Jti,System.Guid.NewGuid().ToString()) |]

  let expires = System.Nullable(System.DateTime.UtcNow.AddHours(1.0))
  let notBefore = System.Nullable(System.DateTime.UtcNow)
  let securityKey = SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
  let signingCredentials = SigningCredentials(key = securityKey,algorithm = SecurityAlgorithms.HmacSha256)

  let token =
    JwtSecurityToken(
      issuer = "jwtwebapp.net",
      audience = "jwtwebapp.net",
      claims = claims,
      expires = expires,
      notBefore = notBefore,
      signingCredentials = signingCredentials
    )

  let tokenResult = { Token = JwtSecurityTokenHandler().WriteToken(token) }

  tokenResult

let handleGetSecured =
  fun (next: HttpFunc) (ctx: HttpContext) ->
    let email = ctx.User.FindFirst ClaimTypes.NameIdentifier

    text ("User " + email.Value + " is authorized to access this resource.") next ctx

let handlePostToken =
  fun (next: HttpFunc) (ctx: HttpContext) ->
    task {
      let! model = ctx.BindJsonAsync<Credentials>()

      // authenticate user

      let tokenResult = generateToken model.email

      return! json tokenResult next ctx
    }