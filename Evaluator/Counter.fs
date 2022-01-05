namespace Evaluator

open NaiveSharpEval

module Counter =
    open Avalonia.Controls
    open Avalonia.FuncUI.DSL
    open Avalonia.Layout
    open Avalonia.FuncUI.Elmish
    open Avalonia.FuncUI.Components.Hosts
    open System
    open Elmish
    open Avalonia
    open Avalonia.Controls
    open Avalonia.Input
    
    type State = { 
        request : string
        response : string }
    let init = { response = "Waiting for a run..."; request = "1 + 1" }, Cmd.none

    type Msg =
        | Run
        | TextChanged of string
        | Display of string

    let update (msg: Msg) (state: State) =
        match msg with
        | Run ->
            let respond dispatch : unit =
                let task =
                    task {
                        let! result = Executor.ExecuteFSharp state.request
                        let toDisplay =
                             match result with
                             | Some r -> r
                             | None -> "Error!";                       
                        dispatch (Display toDisplay)
                    }
                ()
            state, Cmd.ofSub respond
        | TextChanged s -> { state with request = s }, Cmd.none
        | Display response -> { state with response = response }, Cmd.none
    
    let view (state: State) (dispatch) =
        DockPanel.create [
            DockPanel.children [
                Button.create [
                    Button.dock Dock.Bottom
                    Button.onClick (fun _ -> dispatch Run)
                    Button.content "Run"
                ]
                TextBox.create [
                    TextBox.dock Dock.Top
                    TextBox.onTextChanged (fun s -> dispatch (TextChanged s) )
                ]
                TextBlock.create [
                    TextBlock.dock Dock.Top
                    TextBlock.fontSize 16.0
                    TextBlock.verticalAlignment VerticalAlignment.Center
                    TextBlock.horizontalAlignment HorizontalAlignment.Center
                    TextBlock.text (state.response)
                ]
            ]
        ]       