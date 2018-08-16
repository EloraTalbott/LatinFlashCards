module Router

open Saturn
open Giraffe.Core
open Giraffe.ResponseWriters
open Users
open UserViews
open System.Threading.Tasks
open Giraffe
open Saturn
open Shared

let getInitCounter() : Task<FlashCardData> = task { return (FlashCards.getRandom()) }
let getHello() : Task<string> = task { return "Hello you" }

// let webApp = router {
//     get "/api/hello" (fun next ctx -> 
//         task { 
//             let! h = getHello()
//             return! Successful.OK h next ctx
//         })
// }

let browser = pipeline {
    plug acceptHtml
    // plug acceptJson
    // plug putSecureBrowserHeaders
    plug fetchSession
    set_header "x-pipeline-type" "Browser"
}

let defaultView = router {
    get "/" (htmlView Index.layout)
    get "/index.html" (redirectTo false "/")
    get "/default.html" (redirectTo false "/")
    get "/app/login" (htmlView Login.layout)
    get "/app/signin-github" (redirectTo false "/app/flashcard")
}

// let logout : HttpHandler =
//     signOut "Cookies"
//     >=> redirectTo false "/"

let loggedInView = router {
    pipe_through loggedIn

    // get "" (htmlView UserPage.layout)
    // get "/" (htmlView UserPage.layout)
    get "/flashcard" (htmlView UserPage.layout)
    get "/admin" (isAdmin >=> htmlView AdminPage.layout)
    // get "/logout" (logout)
}

let loggedInApi = router {
    pipe_through loggedIn

    get "/hello" (fun next ctx -> json (FlashCards.getRandom()) next ctx)
    get "/init" (fun next ctx -> 
        task { 
            let! h = getInitCounter()
            return! json h next ctx
        })
}

let browserRouter = router {
    not_found_handler (htmlView NotFound.layout) //Use the default 404 webpage
    pipe_through browser //Use the default browser pipeline
    forward "" defaultView //Use the default view
    forward "/app" loggedInView
    forward "/api" loggedInApi
}

let appRouter = router {
    forward "" browserRouter
}