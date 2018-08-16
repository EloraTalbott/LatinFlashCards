module Login

open Giraffe.GiraffeViewEngine

let view = [
    a [ _href "./signin-github" ] [rawText "Login with Github"]
]

let layout =
    html [] [
        yield App.headLayout []
        yield! view
    ]