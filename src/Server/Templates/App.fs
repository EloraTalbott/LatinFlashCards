module App

open Giraffe.GiraffeViewEngine

let headLayout (content: XmlNode list) = 
    head [] [
        meta [_charset "utf-8"]
        meta [_name "viewport"; _content "width=device-width, initial-scale=1" ]
        title [] [encodedText "Latin FlashCards"]
        link [_rel "stylesheet"; _href "https://cdnjs.cloudflare.com/ajax/libs/bulma/0.6.1/css/bulma.min.css" ]
        link [_rel "stylesheet"; _href "https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css" ]
        link [_rel "stylesheet"; _href "https://fonts.googleapis.com/css?family=Open+Sans" ]
        link [_rel "shortcut icon"; _type "image/png"; _href "/Images/favicon.png" ]

    ]
let headBody (content: XmlNode list) = 
    body [] [
        yield div [ _id "elmish-app" ] [
        ]
        yield! content
        yield script [_src "/js/bundle.js"] []
    ]

let layout (content: XmlNode list) =
    html [] [
        yield headLayout content
        yield headBody content
    ]
