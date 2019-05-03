// Learn more about F# at http://fsharp.org

open System
open  System.Text.RegularExpressions
exception NegativeValues of List<int>
exception WrongPattren of String

// calcuate the sum of numbers in string list            
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
// extract the delimiter String and numbers string from string, default delimiter if pattern not found , 
let ExtractDelimiter (x: String) =
    let Matches = Regex.Match( x, "^\/\/(\[.+\])+\n(.+)")
    match Matches.Groups.Count with
      | 3 -> Matches.Groups.[1].Value.Split ([| "[" ; "]" |], StringSplitOptions.RemoveEmptyEntries) , Matches.Groups.[2].Value
      | _ -> [|","|] , x

// add method to hanadle addition of string containing dilimiter and numbers
let Add (x : String) =
 
  let Delimiter, Numbers = ExtractDelimiter x
  
  // build regex pattren  
  let Pattern = String.concat (Array.map Regex.Escape Delimiter |> String.concat "|") ["^(-?[0-9]+("; "))*[0-9]+$"]
  
  // make sure pattren is of numbers matching the delimiter
  match Regex.IsMatch( Numbers, Pattern) with
  | true ->
      let sum , negative = Sum (Array.toList (Numbers.Split ( Delimiter, StringSplitOptions.RemoveEmptyEntries)))
      match negative with
      | [] -> sum
      | _ -> raise (NegativeValues negative)
  | false -> raise (WrongPattren Numbers)
  
  

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
    Actor.Post "//[oo][****][////]\n2****14////44oo4000oo100"
    Actor.Post "//[***][222]\n1***2***3"
    Actor.Post "//[;]\n2;14;44"
    Actor.Post "//[oo]\n2cc14cc44cc4000cc100"
    Actor.Post "//[;;][cc]\n2;;14;;22cc44"
    Actor.Post "1,2,3"
    Actor.Post "1"
    Actor.Post "//[oo]\n2cc14cc44cc4000cc100cc"
    Actor.Post "//[oo][****][////]\n2****14****44oo4000oo100"
    Actor.Post "//[oo][****][////]\n2****-14****44oo4000oo100"
    Actor.Post "//[oo][****][////]\n2****-14,44oo4000oo100"
    Actor.Post "[oo][****][////]\n2****-14,44oo4000oo100"
    Actor.Post "//[oo][****][////]\n2****14////44oo4000oo100"
    
    Console.WriteLine("Press any key...")
    Console.ReadLine() |> ignore
    0
