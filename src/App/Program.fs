// Learn more about F# at http://fsharp.org

open System
open  System.Text.RegularExpressions

let ExtractDelimiter (x: String) =
    let Matches = Regex.Match( x, "^\/\/(.+)\n(.+)")
    match Matches.Groups.Count with
      | 3 -> Matches.Groups.[1].Value , Matches.Groups.[2].Value
      | _ -> "," , x
          
          
let Add (x : String) =
  
  let (delimiter, x) = ExtractDelimiter x
  
  let Pattern = String.concat delimiter ["^([0-9]+["; "])*[0-9]+"] 
  match Regex.IsMatch( x, Pattern) with
    | true -> Array.sumBy System.Int32.Parse (x.Split ( [|"," ; delimiter|], StringSplitOptions.RemoveEmptyEntries))
    | false -> -1

[<EntryPoint>]
let main argv =
    let Actor = MailboxProcessor.Start(fun inbox-> 
        
    // the message processing function
        let rec messageLoop() = async{
            
            // read a message
            let! msg = inbox.Receive()
            
            // process a message
            try
              printfn "message is: %i" (Add msg)
            with
              | invalidArg -> printfn "invalid arguments passed"

            // loop to top
            return! messageLoop()  
            }
        // start the loop 
        messageLoop()
    )
    Actor.Post ""
    Actor.Post "1"
    Actor.Post "1,2"
    Actor.Post "//;\n2;14;44"
    Actor.Post "//cc\n2;14;44"
    Actor.Post "//cc\n2cc14cc44"
    Actor.Post "//cc\n2;14;"
    Actor.Post "4,"
    Console.WriteLine("Press any key...")
    Console.ReadLine() |> ignore
    0
