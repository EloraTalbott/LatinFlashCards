open System.IO
open System.Threading.Tasks

open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Giraffe
open Saturn
open Shared
open FSharp.Data

open Giraffe.Serialization
open Microsoft.WindowsAzure.Storage

let tryGetEnv = System.Environment.GetEnvironmentVariable >> function null | "" -> None | x -> Some x

let publicPath = tryGetEnv "public_path" |> Option.defaultValue "../Client/public" |> Path.GetFullPath
let port = 8085us


let allFlashCards = 
  [
    {
        Front = {Nominative = [LatinText.Normal "nauta"]; Genitive = [LatinText.Normal "nautae";] }
        Back = 
          {
            Declension = Declension.First
            Lesson = 1
            EnglishTranslation = "sailor"
            EnglishDerivatives = ["nautical"; "nautilus";]
            Gender = Gender.Masculine
          }
    }
//2    
    {
        Front = {Nominative = [LatinText.Normal "terra"]; Genitive = [LatinText.Normal "terrae"] }
        Back = 
          {
            Declension = Declension.First
            Lesson = 1
            EnglishTranslation = "earth, land"
            EnglishDerivatives = ["How am I supposed to know";]
            Gender = Gender.Feminine
          }
    }
//3
    {
        Front = {Nominative = [LatinText.Normal "porta";]; Genitive = [LatinText.Normal "portae";] }
        Back = 
          {
            Declension = Declension.First
            Lesson = 1
            EnglishTranslation = "gate"
            EnglishDerivatives = ["portal, airport, portico";]
            Gender = Gender.Feminine
          }
    }  
//4
    {
        Front = {Nominative = [LatinText.Normal "silva";]; Genitive = [LatinText.Normal "silvae";] }
        Back = 
          {
            Declension = Declension.First
            Lesson = 1
            EnglishTranslation = "forest"
            EnglishDerivatives = ["silviculture";]
            Gender = Gender.Feminine
          }
    }
//5
    {
        Front = {Nominative = [LatinText.Normal "gladius";]; Genitive = [LatinText.Normal "gladii";] }
        Back = 
          {
            Declension = Declension.Second
            Lesson = 0
            EnglishTranslation = "sword"
            EnglishDerivatives = ["How am I supposed to know";]
            Gender = Gender.Masculine
          }
    }
//6
    {
        Front = {Nominative = [LatinText.Normal "servus";]; Genitive = [LatinText.Normal "servi";] }
        Back = 
          {
            Declension = Declension.Second
            Lesson = 0
            EnglishTranslation = "slave"
            EnglishDerivatives = ["How am I suposed to know";]
            Gender = Gender.Masculine
          }
    }
//7
    {
        Front = {Nominative = [LatinText.Normal "caelum";]; Genitive = [LatinText.Normal "caeli";] }
        Back = 
          {
            Declension = Declension.Second
            Lesson = 0
            EnglishTranslation = "sky,heaven"
            EnglishDerivatives = ["How am I supposed to know";]
            Gender = Gender.Neuter
          }
    }
//8
    {
        Front = {Nominative = [LatinText.Normal "fillius";]; Genitive = [LatinText.Normal "filii";] }
        Back = 
          {
            Declension = Declension.First
            Lesson = 0
            EnglishTranslation = "son"
            EnglishDerivatives = ["How am I supposed to know";]
            Gender = Gender.Masculine
          }
    }
//9
    {
        Front = {Nominative = [LatinText.Normal "amicus";]; Genitive = [LatinText.Normal "amici";] }
        Back = 
          {
            Declension = Declension.Second
            Lesson = 0
            EnglishTranslation = "friend"
            EnglishDerivatives = ["How am I supposed to know";]
            Gender = Gender.Masculine
          }
    }
//10
    {
        Front = {Nominative = [LatinText.Normal "romanus";]; Genitive = [LatinText.Normal "romani";] }
        Back = 
          {
            Declension = Declension.Second
            Lesson = 0
            EnglishTranslation = "roman"
            EnglishDerivatives = ["How am I supposed to know";]
            Gender = Gender.Masculine
          }
    }
//11
    {
        Front = {Nominative = [LatinText.Normal "dux";]; Genitive = [LatinText.Normal "ducis";] }
        Back = 
          {
            Declension = Declension.First
            Lesson = 0
            EnglishTranslation = "leader"
            EnglishDerivatives = ["How am I supposed to know";]
            Gender = Gender.Masculine
          }
    }
//12
//need macrons
    {
        Front = {Nominative = [LatinText.Normal "Mar"; LatinText.Macron 'i'; LatinText.Normal "a";]; Genitive = [LatinText.Normal "Mar"; LatinText.Macron 'i'; LatinText.Normal "ae";] }
        Back = 
          {
            Declension = Declension.First
            Lesson = 1
            EnglishTranslation = "Mary"
            EnglishDerivatives = ["How am I supposed to know";]
            Gender = Gender.Feminine
          }
    }
//13
//need macrons
    {
        Front = {Nominative = [LatinText.Normal "gl";LatinText.Macron 'o'; LatinText.Normal "ria";]; Genitive = [LatinText.Normal "gl";LatinText.Macron 'o'; LatinText.Normal "riae";] }
        Back = 
          {
            Declension = Declension.First
            Lesson = 1
            EnglishTranslation = "fame, glory"
            EnglishDerivatives = ["glorious, glory";]
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

let configureAzure (services:IServiceCollection) =
    tryGetEnv "APPINSIGHTS_INSTRUMENTATIONKEY"
    |> Option.map services.AddApplicationInsightsTelemetry
    |> Option.defaultValue services

let app = application {
    url ("http://0.0.0.0:" + port.ToString() + "/")
    use_router webApp
    memory_cache
    use_static publicPath
    service_config configureSerialization
    service_config configureAzure
    use_gzip
}

run app
