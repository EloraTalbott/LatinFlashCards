open System.IO
open System.Threading.Tasks

open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Giraffe
open Saturn
open Shared
open FSharp.Data

open Giraffe.Serialization

let publicPath = Path.GetFullPath "../Client/public"
let port = 8085us


let allFlashCards = 
  [
    {
        Front = {Nominative = "nauta"; Genitive = "nautae" }
        Back = 
          {
            Declension = Declension.First
            Lesson = 1
            EnglishTranslation = "sailor"
            EnglishDerivatives = ["nautical"; "nautilus";]
            Gender = Gender.Masculine
          }
    }
    {
        Front = {Nominative = "terra"; Genitive = "terrae" }
        Back = 
          {
            Declension = Declension.First
            Lesson = 1
            EnglishTranslation = "earth, land"
            EnglishDerivatives = ["How am I supposed to know";]
            Gender = Gender.Feminine
          }
    }
  ]  

let getRandomFlashCard () =  
  let rnd = System.Random()  
  allFlashCards |> List.item (rnd.Next(allFlashCards.Length))

let getInitCounter() : Task<FlashCardData> = task { return (getRandomFlashCard ()) }
let getHello() : Task<string> = task { return "Hello" }

let webApp = router {
    get "/api/init" (fun next ctx ->
        task {
            let! counter = getInitCounter()
            return! Successful.OK counter next ctx
        })
    get "/api/hello" (fun next ctx -> 
        task { 
            let! h = getHello()
            return! Successful.OK h next ctx
        })
}

let configureSerialization (services:IServiceCollection) =
    let fableJsonSettings = Newtonsoft.Json.JsonSerializerSettings()
    fableJsonSettings.Converters.Add(Fable.JsonConverter())
    services.AddSingleton<IJsonSerializer>(NewtonsoftJsonSerializer fableJsonSettings)

let app = application {
    url ("http://0.0.0.0:" + port.ToString() + "/")
    use_router webApp
    memory_cache
    use_static publicPath
    service_config configureSerialization
    use_gzip
}

run app
