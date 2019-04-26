// Learn more about F# at http://fsharp.org

open System
open  System.Text.RegularExpressions
exception NegativeValues of List<int>
exception WrongPattren of String

let rec Sum (x:List<String>) =
      match x with
      | [] -> 0, []
      | a::tail ->
          let a = System.Int32.Parse a
          if a> 999 then
             let (sum , negative) = Sum tail
             sum, negative
          elif a >= 0 then
             let (sum , negative) = Sum tail
             (sum + a), negative
          else
             let (sum, negative) = Sum tail
             sum, a :: negative
      
      
let ExtractDelimiter (x: String) =
    let Matches = Regex.Match( x, "^\/\/\[(.+)\]\n(.+)")
    match Matches.Groups.Count with
      | 3 -> Matches.Groups.[1].Value , Matches.Groups.[2].Value
      | _ -> "," , x
          
          
let Add (x : String) =
  
  let (delimiter, numbers) = ExtractDelimiter x
  
  let Pattern = String.concat delimiter ["^(-?[0-9]+"; ")*-?[0-9]+$"] 
  match Regex.IsMatch( numbers, Pattern) with
    | true ->
        let (sum, negative) = Sum (Array.toList (numbers.Split ( [|"," ; delimiter|], StringSplitOptions.RemoveEmptyEntries)))
        match negative with
        | [] -> sum
        | _ -> raise (NegativeValues negative)
    | false -> raise (WrongPattren numbers)

[<EntryPoint>]
let main argv =
    let Actor = MailboxProcessor.Start(fun inbox-> 
        
    // the message processing function
        let rec messageLoop() = async{
            
            // read a message
            let! msg = inbox.Receive()
            
            // process a message
            try
              printfn "sum of %s : %i" msg (Add msg)
            with
              | NegativeValues(neglist) -> printfn "negatives not allowed %A" neglist
              | WrongPattren(str) -> printfn "wrong pattren %s" msg

            // loop to top
            return! messageLoop()  
            }
        // start the loop 
        messageLoop()
    )
    Actor.Post ""
    Actor.Post "1"
    Actor.Post "1,-2"
    Actor.Post "//;\n2;14;44"
    Actor.Post "//[cc]\n2;14;44"
    Actor.Post "//[cc]\n2cc14cc44cc4000cc100"
    Actor.Post "//[cc]\n2;14;"
    Actor.Post "4,"
    Console.WriteLine("Press any key...")
    Console.ReadLine() |> ignore
    0
