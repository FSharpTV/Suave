// Learn more about F# at https://fsharp.tv

open Suave

[<EntryPoint>]
let main argv =
  startWebServer defaultConfig (Successful.OK "Hello World!")
  0