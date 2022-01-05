module NaiveSharpEval

open Microsoft.DotNet.Interactive.FSharp
open Microsoft.DotNet.Interactive.CSharp
open Microsoft.DotNet.Interactive
open Microsoft.DotNet.Interactive.Commands
open System
open Microsoft.DotNet.Interactive.Events
open System.Runtime.InteropServices
open Microsoft.DotNet.Interactive.Formatting

let fsharpKernel =
    lazy
        new FSharpKernel ()

let csharpKernel =
    lazy
        new CSharpKernel ()

Formatter.SetPreferredMimeTypeFor(typeof<obj>, "text/plain")

type Executor =
    static member private Execute (kernel : DotNetKernel, code) =
        task {
            let submitCode = SubmitCode code
            let! computed = kernel.SendAsync submitCode

            let mutable response : string Option = None
            let mutable res : string Option = None

            computed.KernelEvents.Subscribe (new Action<KernelEvent>(fun e ->
                        match e with
                        | :? CommandSucceeded ->
                            match response with
                            | Some nonEmpty -> res <- Some nonEmpty
                            | None -> res <- Some ""
                        | :? DisplayEvent as display ->
                            response <- (Seq.head display.FormattedValues).Value |> Some
                        | _ -> ()
                ))
            |> (fun observer -> observer.Dispose())

            return res
        }

    static member ExecuteFSharp code =
        Executor.Execute ((fsharpKernel.Value), code)