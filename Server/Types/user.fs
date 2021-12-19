module Cribs.Types.Users

type User = {
  Id: System.Guid
  Username: string
  Password: string
  First_name: string
  Last_name: string
}

type userQuery = {
  username:string
}