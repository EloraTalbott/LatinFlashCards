module Users

open Saturn
open Giraffe
open System.Security.Claims

let matchUpUsers : HttpHandler = fun next ctx ->
    // A real implementation would match up user identities with something stored in a database
    ctx.User.Claims |> Seq.iter (fun claim -> printfn "AUTH INFO: Claim issuer %s claim value %s" claim.Issuer claim.Value)
    let isAdmin =
        ctx.User.Claims |> Seq.exists (fun claim ->
            claim.Issuer = "GitHub" && claim.Type = ClaimTypes.Name && claim.Value = "Elora Talbott")
    if isAdmin then
        ctx.User.AddIdentity(new ClaimsIdentity([Claim(ClaimTypes.Role, "Admin", ClaimValueTypes.String, "MyApplication")]))
    next ctx

let loggedIn = pipeline {
    requires_authentication (Giraffe.Auth.challenge "GitHub")
    plug matchUpUsers
}

let error: HttpHandler = RequestErrors.forbidden (text "Must be admin")

let isAdmin = pipeline {
    plug loggedIn
    requires_role "Admin" error
}