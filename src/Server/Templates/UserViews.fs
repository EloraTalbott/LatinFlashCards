module UserViews

open Giraffe.GiraffeViewEngine

module AdminPage =
    let view = [
        h1 [] [rawText "I'm admin"]
    ]
    let layout = App.layout view

module UserPage =
    let view = [
        h1 [] [rawText "I'm a logged in user"]
    ]
    let layout = App.layout view