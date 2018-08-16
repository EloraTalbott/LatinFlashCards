open System.IO
open System.Threading.Tasks

open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Giraffe
open Saturn
open Shared
open Giraffe.Serialization

let publicPath = Path.GetFullPath "../Client/public"
let port = 8085us

let configureSerialization (services:IServiceCollection) =
    let fableJsonSettings = Newtonsoft.Json.JsonSerializerSettings()
    fableJsonSettings.Converters.Add(Fable.JsonConverter())
    services.AddSingleton<IJsonSerializer>(NewtonsoftJsonSerializer fableJsonSettings)

let app = application {
    url ("http://0.0.0.0:" + port.ToString() + "/")
    use_router Router.appRouter
    memory_cache
    use_static publicPath

    use_github_oauth "id" "key" "/app/cards" [("login", "githubUsername"); ("name", "fullName")]
    service_config configureSerialization
    use_gzip
}

run app
