// Learn more about F# at http://fsharp.org

open System
open  System.Text.RegularExpressions

let Add (x : String) =
  match Regex.IsMatch( x, "([0-9]+,)*[0-9]" ) with
    | true -> Array.sumBy System.Int32.Parse (x.Split ( "," , StringSplitOptions.RemoveEmptyEntries))
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
    Actor.Post "2,14"
    Actor.Post "4,"
    Console.WriteLine("Press any key...")
    Console.ReadLine() |> ignore
    0
