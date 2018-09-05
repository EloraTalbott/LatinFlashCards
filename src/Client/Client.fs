module Client

open Elmish
open Elmish.React

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.PowerPack.Fetch

open Shared

open Fulma

type FlashCard = { FlashCard: FlashCardData; ShowAnswer:bool; }

// The model holds data that you want to keep track of while the application is running
// in this case, we are keeping track of a counter
// we mark it as optional, because initially it will not be available from the client
// the initial value will be requested from server
type Model = 
  | FlashCard of FlashCard
  | FullList of FlashCardData list

// The Msg type defines what events/actions can occur while the application is running
// the state of the application changes *only* in reaction to these events
type Msg =
  | ShowAnswer
  | SetCard of FlashCardData
  | FetchQuestion
  | CardLoaded of Result<FlashCardData, exn>
  | SetList of Result<FlashCardData list, exn>
  | ToggleList

let fetchList () : Cmd<Msg> =
  Cmd.ofPromise
    (fetchAs<FlashCardData list> "/api/getAll")
    []
    (Ok >> SetList)
    (Error >> CardLoaded)

let fetchQuestion () : Cmd<Msg> =
  // Cmd.none
  Cmd.ofPromise
    (fetchAs<FlashCardData> "/api/init")
    []
    (Ok >> CardLoaded)
    (Error >> CardLoaded)

// defines the initial state and initial command (= side-effect) of the application
let init () : Model * Cmd<Msg> =
    let initialModel = FullList []
    let loadCardCmd = fetchQuestion ()
    initialModel, loadCardCmd

// The update function computes the next state of the application based on the current state and the incoming events/messages
// It can also run side-effects (encoded as commands) like calling the server via Http.
// these commands in turn, can dispatch messages to which the update function will react.
let update (msg : Msg) (currentModel : Model) : Model * Cmd<Msg> =
    match currentModel, msg with    
    | Model.FlashCard x, ShowAnswer ->
        let nextModel = { x with ShowAnswer = true }
        FlashCard nextModel, Cmd.none
    | Model.FlashCard x, SetCard cardInfo ->
        let nextModel = { x with FlashCard = cardInfo; ShowAnswer = false; }
        FlashCard nextModel, Cmd.none
    | _, CardLoaded (Ok initialCard)->
        let nextModel = { FlashCard = initialCard; ShowAnswer = false; }
        FlashCard nextModel, Cmd.none
    | Model.FlashCard x, FetchQuestion ->
        currentModel, fetchQuestion()
    | Model.FlashCard x, ToggleList ->
        currentModel, fetchList()
    | Model.FullList x, ToggleList ->
        currentModel, fetchQuestion()
    | _, SetList (Ok newList) ->
        Model.FullList newList, Cmd.none
    | _ -> currentModel, Cmd.none//fetchQuestion()


let show = function
| Model.FlashCard x when x.ShowAnswer -> "Click for next card"
| Model.FlashCard x when not x.ShowAnswer -> "Click to reveal answer"
| Model.FullList x when x.Length = 0 -> "Loading..."
| _ -> ""

let latinTextToElements items =
  items
  |> List.map (fun s ->
    match s with
    | LatinText.Normal v -> str v
    | LatinText.Macron v -> span [(DangerouslySetInnerHTML { __html = "&" + v.ToString() + "macr;" }) :> IHTMLProp] []
  )


let endingString flashCard =
  match flashCard.Back.Ending with
  | EndingType.NotApplicable -> ""
  | EndingType.Conjugation v -> (sprintf "Conjugation: %A" v)
  | EndingType.Declension v -> (sprintf "Declension: %A" v)
  
let showCard x (dispatch : Msg -> unit) =

  match x.ShowAnswer with
  | true ->
      Hero.hero [ Hero.Color IsInfo; Hero.IsHalfHeight; ] [
        Hero.head [][
          Columns.columns [] [
            Column.column [] [ str (endingString x.FlashCard) ]
            Column.column [] [ Text.p [Modifiers[ Modifier.TextAlignment (Screen.All, TextAlignment.Right)]] [str (sprintf "Lesson: %i" x.FlashCard.Back.Lesson)]]]
        ]
        
        Hero.body [ Props[ OnClick (fun _ -> dispatch FetchQuestion);] ][
          Container.container [][
            Columns.columns [ Columns.IsCentered; Columns.IsMultiline; ]
              [ 
                Column.column [ 
                  Column.Width(Screen.All, Column.IsHalf); 
                ] [ 
                  Heading.h1 [ Heading.Modifiers[ Modifier.TextAlignment (Screen.All, TextAlignment.Centered) ]][str (x.FlashCard.Back.EnglishTranslation)]
                ]

              ]
          ] 
        ]
        Hero.foot [][
          Columns.columns [] [
            Column.column [ ] [ str (sprintf "Derivatives: %s" (System.String.Join(", ", x.FlashCard.Back.EnglishDerivatives))) ]
            Column.column [ ] [ Text.p [Modifiers[ Modifier.TextAlignment (Screen.All, TextAlignment.Right)]] [str (sprintf "Gender: %A" x.FlashCard.Back.Gender)]]]
        ]
      ]
      
    
  | false ->

      let nominative = latinTextToElements x.FlashCard.Front.Nominative
      let genitive = latinTextToElements x.FlashCard.Front.Genitive
      let combinedFrontTextElements = [
        yield! nominative
        yield (str " / ")
        yield! genitive
      ]

      Hero.hero [ Hero.Color IsPrimary; Hero.IsHalfHeight; ] [
        Hero.head [][str " "]
        Hero.body [ Props[ OnClick (fun _ -> dispatch ShowAnswer);] ][
          Container.container [][
            Heading.h1 [ Heading.Modifiers[ Modifier.TextAlignment (Screen.All, TextAlignment.Centered)]][yield! combinedFrontTextElements]
        ]
        ]
        Hero.foot [][str " "]
      ]

let showCardList cards (dispatch : Msg -> unit) =
  [ 
    for card in cards do
      yield Columns.columns [] [
        Column.column [] [yield! latinTextToElements card.Front.Nominative]
        Column.column [] [yield! latinTextToElements card.Front.Genitive]
        Column.column [] [str card.Back.EnglishTranslation]
        Column.column [] [str (System.String.Join(",",card.Back.EnglishDerivatives))]
        Column.column [] [str (sprintf "%i" card.Back.Lesson)]
        Column.column [] [str (sprintf "%A" card.Back.Gender)]
        Column.column [] [str (endingString card)]
      ]
  ]

let view (model : Model) (dispatch : Msg -> unit) =
  let toggleButtonLabel = 
    match model with
    | Model.FlashCard _ -> "List"
    | Model.FullList _ -> "Card"
    
  div [] [ 
    Container.container [] [ 
      Content.content [ Content.Modifiers [ Modifier.TextAlignment (Screen.All, TextAlignment.Centered) ] ]
        [ 
          Heading.h3 [] [ str (show model) ]
          Button.button [Button.OnClick (fun _ -> dispatch ToggleList)] [str toggleButtonLabel]
        ]
      Columns.columns [] [
        Column.column [] [ 
          match model with
          | Model.FlashCard v -> yield showCard v dispatch
          | Model.FullList v -> yield! showCardList v dispatch
        ]] 
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
