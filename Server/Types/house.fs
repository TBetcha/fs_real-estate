module Cribs.Types.House

type Address = {
  streedNum:string
  city:string
  state:string
  zip:int
}

type House = {
  listingId:System.Guid
  address:Address
  owner:Users.User
  bedroom:int
  bathroom:float
  squareFeet:int
}
