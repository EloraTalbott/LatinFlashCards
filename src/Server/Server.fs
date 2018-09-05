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
            Ending = EndingType.Declension Declension.First
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
            Ending = EndingType.Declension Declension.First
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
            Ending = EndingType.Declension Declension.First
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
            Ending = EndingType.Declension Declension.First
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
            Ending = EndingType.Declension Declension.Second
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
            Ending = EndingType.Declension Declension.Second
            Lesson = 0
            EnglishTranslation = "slave, servant"
            EnglishDerivatives = ["How am I suposed to know";]
            Gender = Gender.Masculine
          }
    }
//7
    {
        Front = {Nominative = [LatinText.Normal "caelum";]; Genitive = [LatinText.Normal "caeli";] }
        Back = 
          {
            Ending = EndingType.Declension Declension.Second
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
            Ending = EndingType.Declension Declension.First
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
            Ending = EndingType.Declension Declension.Second
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
            Ending = EndingType.Declension Declension.Second
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
            Ending = EndingType.Declension Declension.First
            Lesson = 0
            EnglishTranslation = "leader"
            EnglishDerivatives = ["How am I supposed to know";]
            Gender = Gender.Masculine
          }
    }
//12
    {
        Front = {Nominative = [LatinText.Normal "Mar"; LatinText.Macron 'i'; LatinText.Normal "a";]; Genitive = [LatinText.Normal "Mar"; LatinText.Macron 'i'; LatinText.Normal "ae";] }
        Back = 
          {
            Ending = EndingType.Declension Declension.First
            Lesson = 1
            EnglishTranslation = "Mary"
            EnglishDerivatives = ["How am I supposed to know";]
            Gender = Gender.Feminine
          }
    }
//13
    {
        Front = {Nominative = [LatinText.Normal "gl";LatinText.Macron 'o'; LatinText.Normal "ria";]; Genitive = [LatinText.Normal "gl";LatinText.Macron 'o'; LatinText.Normal "riae";] }
        Back = 
          {
            Ending = EndingType.Declension Declension.First
            Lesson = 1
            EnglishTranslation = "fame, glory"
            EnglishDerivatives = ["glorious, glory";]
            Gender = Gender.Feminine
          }
    }
//14
    {
        Front = {Nominative = [LatinText.Normal "v";LatinText.Macron 'i';LatinText.Normal "cerunt";]; Genitive = [] }
        Back = 
          {
            Ending = EndingType.Conjugation Conjugation.First
            Lesson = 0
            EnglishTranslation = "they conquered"
            EnglishDerivatives = ["Umm...";]
            Gender = Gender.Feminine
          }
    }
//15
    {
        Front = {Nominative = [LatinText.Normal "vid";LatinText.Macron 'e';LatinText.Normal "tis";]; Genitive = [] }
        Back = 
          {
            Ending = EndingType.Conjugation Conjugation.First
            Lesson = 0
            EnglishTranslation = "you see (plural)"
            EnglishDerivatives = ["";]
            Gender = Gender.Feminine
          }
    }
//16
    {
        Front = {Nominative = [LatinText.Normal "incolunt";]; Genitive = [LatinText.Normal "";] }
        Back = 
          {
            Ending = EndingType.Conjugation Conjugation.First
            Lesson = 0
            EnglishTranslation = "they inhabit"
            EnglishDerivatives = ["";]
            Gender = Gender.Feminine
          }
    }
//17
    {
        Front = {Nominative = [LatinText.Normal "vict"; LatinText.Macron 'o'; LatinText.Normal "ria";]; Genitive = [LatinText.Normal "vict"; LatinText.Macron 'o'; LatinText.Normal "riae";] }
        Back = 
          {
            Ending = EndingType.Declension Declension.First
            Lesson = 0
            EnglishTranslation = "victory"
            EnglishDerivatives = ["victory";]
            Gender = Gender.Feminine
          }
    }
//18
    {
        Front = {Nominative = [LatinText.Normal "propter";]; Genitive = [LatinText.Normal "";] }
        Back = 
          {
            Ending = EndingType.NotApplicable
            Lesson = 0
            EnglishTranslation = "on account of"
            EnglishDerivatives = ["Umm...";]
            Gender = Gender.Feminine
          }
    }
//19
    {
        Front = {Nominative = [LatinText.Normal "R";LatinText.Macron 'o';LatinText.Normal "ma";]; Genitive = [LatinText.Normal "R";LatinText.Macron 'o';LatinText.Normal "mae";] }
        Back = 
          {
            Ending = EndingType.Declension Declension.First
            Lesson = 0
            EnglishTranslation = "Rome"
            EnglishDerivatives = ["Rome";]
            Gender = Gender.Feminine
          }
    }
//20
//need macron in i
    {
        Front = {Nominative = [LatinText.Normal "Gallus";]; Genitive = [LatinText.Normal "Gall";LatinText.Macron 'i';] }
        Back = 
          {
            Ending = EndingType.Declension Declension.Second
            Lesson = 0
            EnglishTranslation = "a Gaul"
            EnglishDerivatives = ["Umm...";]
            Gender = Gender.Feminine
          }
    }
//21
    {
        Front = {Nominative = [LatinText.Normal "Gallia";]; Genitive = [LatinText.Normal "Galliae";] }
        Back = 
          {
            Ending = EndingType.Declension Declension.First
            Lesson = 0
            EnglishTranslation = "Gaul"
            EnglishDerivatives = ["Umm...";]
            Gender = Gender.Feminine
          }
    }
//22
    {
        Front = {Nominative = [LatinText.Normal "oppidum";]; Genitive = [LatinText.Normal "oppid";LatinText.Macron 'i';] }
        Back = 
          {
            Ending = EndingType.Declension Declension.Second
            Lesson = 0
            EnglishTranslation = "town"
            EnglishDerivatives = ["Umm...";]
            Gender = Gender.Feminine
          }
    }
//23
    {
        Front = {Nominative = [LatinText.Normal "Chr";LatinText.Macron 'i';LatinText.Normal "stus";]; Genitive = [LatinText.Normal "Chr";LatinText.Macron 'i';LatinText.Normal "st";LatinText.Macron 'i';] }
        Back = 
          {
            Ending = EndingType.Declension Declension.Second
            Lesson = 0
            EnglishTranslation = "Christ"
            EnglishDerivatives = ["Umm...";]
            Gender = Gender.Feminine
          }
    }
//24
    {
        Front = {Nominative = [LatinText.Normal "per";LatinText.Macron 'i';LatinText.Normal "culum";]; Genitive = [LatinText.Normal "per";LatinText.Macron 'i';LatinText.Normal "cul";LatinText.Macron 'i';] }
        Back = 
          {
            Ending = EndingType.Declension Declension.Second
            Lesson = 0
            EnglishTranslation = "danger"
            EnglishDerivatives = ["Umm...";]
            Gender = Gender.Feminine
          }
    }
//25
    {
        Front = {Nominative = [LatinText.Normal "praemium";]; Genitive = [LatinText.Normal "praeni";LatinText.Macron 'i'] }
        Back = 
          {
            Ending = EndingType.Declension Declension.Second
            Lesson = 0
            EnglishTranslation = "reward"
            EnglishDerivatives = ["Umm...";]
            Gender = Gender.Feminine
          }
    }
//26
    {
        Front = {Nominative = [LatinText.Normal "r";LatinText.Macron 'e';LatinText.Normal "gnum";]; Genitive = [LatinText.Normal "r";LatinText.Macron 'e';LatinText.Normal "gn";LatinText.Macron 'i';] }
        Back = 
          {
            Ending = EndingType.Declension Declension.Second
            Lesson = 0
            EnglishTranslation = "kingdom, royal power"
            EnglishDerivatives = ["Umm...";]
            Gender = Gender.Feminine
          }
    }
//27
    {
        Front = {Nominative = [LatinText.Normal "bellum";]; Genitive = [LatinText.Normal "bell"; LatinText.Macron 'i'] }
        Back = 
          {
            Ending = EndingType.Declension Declension.Second
            Lesson = 0
            EnglishTranslation = "war"
            EnglishDerivatives = ["Umm...";]
            Gender = Gender.Feminine
          }
    }
//28
    {
        Front = {Nominative = [LatinText.Normal "Chr";LatinText.Macron 'i';LatinText.Normal "sti";LatinText.Macron 'a';LatinText.Normal "nus";]; Genitive = [LatinText.Normal "Chr";LatinText.Macron 'i';LatinText.Normal "sti";LatinText.Macron 'a';LatinText.Normal "ni";] }
        Back = 
          {
            Ending = EndingType.Declension Declension.Second
            Lesson = 0
            EnglishTranslation = "Christian"
            EnglishDerivatives = ["Umm...";]
            Gender = Gender.Feminine
          }
    }
//29
    {
        Front = {Nominative = [LatinText.Normal "imperium";]; Genitive = [LatinText.Normal "imperi";LatinText.Macron 'i'] }
        Back = 
          {
            Ending = EndingType.Declension Declension.Second
            Lesson = 0
            EnglishTranslation = "comand, power, empire"
            EnglishDerivatives = ["Umm...";]
            Gender = Gender.Feminine
          }
    }
//30!
    {
        Front = {Nominative = [LatinText.Normal "Deus";]; Genitive = [LatinText.Normal "De";LatinText.Macron 'i'] }
        Back = 
          {
            Ending = EndingType.Declension Declension.Second
            Lesson = 0
            EnglishTranslation = "God"
            EnglishDerivatives = ["Umm...";]
            Gender = Gender.Feminine
          }
    }
  ]  

let getRandomFlashCard () =  
  let rnd = System.Random()  
  allFlashCards |> List.item (rnd.Next(allFlashCards.Length))

let getInitCard() : Task<FlashCardData> = task { return (getRandomFlashCard ()) }
let getCards() : Task<FlashCardData list> = task { return (allFlashCards) }
let getHello() : Task<string> = task { return "Hello" }

let webApp = router {
    get "/api/init" (fun next ctx ->
        task {
            let! card = getInitCard()
            return! Successful.OK card next ctx
        })
    get "/api/getAll" (fun next ctx ->
        task {
            let! cards = getCards()
            return! Successful.OK cards next ctx
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
