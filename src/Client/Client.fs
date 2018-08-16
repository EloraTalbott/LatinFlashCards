module Client

open Elmish
open Elmish.React

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.PowerPack.Fetch

open Shared

open Fulma
open System.Net.Http
open System.Net.Http.Headers


// The model holds data that you want to keep track of while the application is running
// in this case, we are keeping track of a counter
// we mark it as optional, because initially it will not be available from the client
// the initial value will be requested from server
type Model = { FlashCard: FlashCardData option; ShowAnswer:bool; }

// The Msg type defines what events/actions can occur while the application is running
// the state of the application changes *only* in reaction to these events
type Msg =
  | ShowAnswer
  | SetCard of FlashCardData
  | FetchQuestion
  | CardLoaded of Result<FlashCardData, exn>

let fetchQuestion () : Cmd<Msg> =
  // Cmd.none
  Cmd.ofPromise
    (fetchAs<FlashCardData> "/api/init")
    []
    (Ok >> CardLoaded)
    (Error >> CardLoaded)

// defines the initial state and initial command (= side-effect) of the application
let init () : Model * Cmd<Msg> =
    let initialModel = { FlashCard = None; ShowAnswer = false; }
    let loadCardCmd = fetchQuestion ()
    initialModel, loadCardCmd

// The update function computes the next state of the application based on the current state and the incoming events/messages
// It can also run side-effects (encoded as commands) like calling the server via Http.
// these commands in turn, can dispatch messages to which the update function will react.
let update (msg : Msg) (currentModel : Model) : Model * Cmd<Msg> =
    match currentModel.FlashCard, msg with
    | Some x, ShowAnswer ->
        let nextModel = { currentModel with ShowAnswer = true }
        nextModel, Cmd.none
    | Some x, SetCard cardInfo ->
        let nextModel = { currentModel with FlashCard = Some cardInfo; ShowAnswer = false; }
        nextModel, Cmd.none
    | _, CardLoaded (Ok initialCard)->
        let nextModel = { FlashCard = Some initialCard; ShowAnswer = false; }
        nextModel, Cmd.none

    | Some x, FetchQuestion ->
        let nextModel = { FlashCard = None; ShowAnswer = false; }
        nextModel, fetchQuestion()
    | _ -> currentModel, Cmd.none//fetchQuestion()


let show = function
| { FlashCard = Some x } -> "Click to reveal answer"
| { FlashCard = None   } -> "Loading..."

let showCard model (dispatch : Msg -> unit) =
  match model with
  | { FlashCard = None   } -> str ""
  | { FlashCard = Some x } ->
    match model.ShowAnswer with
    | true ->
        Hero.hero [ Hero.Color IsInfo; Hero.IsHalfHeight; ] [
          Hero.head [][
            Columns.columns [] [
              Column.column [] [ str (sprintf "Declension: %A" x.Back.Declension) ]
              Column.column [] [ Text.p [Modifiers[ Modifier.TextAlignment (Screen.All, TextAlignment.Right)]] [str (sprintf "Lesson: %i" x.Back.Lesson)]]]
          ]
          
          Hero.body [ Props[ OnClick (fun _ -> dispatch FetchQuestion);] ][
            Container.container [][
              Columns.columns [ Columns.IsCentered; Columns.IsMultiline; ]
                [ 
                  Column.column [ 
                    Column.Width(Screen.All, Column.IsHalf); 
                  ] [ 
                    Heading.h1 [ Heading.Modifiers[ Modifier.TextAlignment (Screen.All, TextAlignment.Centered) ]][str (x.Back.EnglishTranslation)]
                  ]

                ]
            ] 
          ]
          Hero.foot [][
            Columns.columns [] [
              Column.column [ ] [ str (sprintf "Derivatives: %s" (System.String.Join(", ", x.Back.EnglishDerivatives))) ]
              Column.column [ ] [ Text.p [Modifiers[ Modifier.TextAlignment (Screen.All, TextAlignment.Right)]] [str (sprintf "Gender: %A" x.Back.Gender)]]]
          ]
        ]
        
      
    | false ->
        Hero.hero [ Hero.Color IsPrimary; Hero.IsHalfHeight; ] [
          Hero.head [][str " "]
          Hero.body [ Props[ OnClick (fun _ -> dispatch ShowAnswer);] ][
            Container.container [][
              Heading.h1 [ Heading.Modifiers[ Modifier.TextAlignment (Screen.All, TextAlignment.Centered)]][str (x.Front.Nominative + " / " + x.Front.Genitive)]
          ]
          ]
          Hero.foot [][str " "]
        ]

let view (model : Model) (dispatch : Msg -> unit) =
  div [] [ 
    Container.container [] [ 
      Content.content [ Content.Modifiers [ Modifier.TextAlignment (Screen.All, TextAlignment.Centered) ] ]
        [ Heading.h3 [] [ str (show model) ]]
      Columns.columns [] [
        Column.column [] [ showCard model dispatch ] 
        ] 
      ] 
    ]


#if DEBUG
open Elmish.Debug
open Elmish.HMR
#endif

Program.mkProgram init update view
#if DEBUG
|> Program.withConsoleTrace
|> Program.withHMR
#endif
|> Program.withReact "elmish-app"
#if DEBUG
|> Program.withDebugger
#endif
|> Program.run
