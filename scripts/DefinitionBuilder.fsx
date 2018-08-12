#I "../packages/script"
#r "FSharp.Data/lib/net45/FSharp.Data.dll"

open FSharp.Data

let foo = "blah"
foo
type LatinDefinition = HtmlProvider<"http://www.latin-dictionary.net/search/english/sailor">

let def = LatinDefinition.GetSample()
