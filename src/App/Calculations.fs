module Calcluation
open System

let Add (x : String) =
  match x.Length with
    | 0 -> 0
    | _ -> x.Split ',' |>  Array.sumBy  System.Int32.Parse