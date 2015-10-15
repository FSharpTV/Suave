module ``Creating your first Suave webpage``

open Suave
open Suave.Types
open Suave.Http
open Suave.Web

open Suave.Http.Successful

let hi : WebPart = OK "Hello World"

let bye : WebPart = OK "Goodbye World"

let main argv =
  startWebServer defaultConfig hi // => "Hello World"
  0
