module ``Creating your first Suave webpage``

open Suave
open Suave.Successful

let hi : WebPart = OK "Hello World"

let bye : WebPart = OK "Goodbye World"

let main argv =
  startWebServer defaultConfig hi // => "Hello World"
  0
