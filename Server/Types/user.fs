module Cribs.Types.Users

type User =
  { Id: System.Guid
    Username: string
    Password: string
    FirstName: string
    LastName: string }

type UserDto =
  { Id: System.Guid
    Username: string
    FirstName: string
    LastName: string }

type userQuery = { username: string }
