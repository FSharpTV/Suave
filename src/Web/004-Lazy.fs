module ``Understanding eager and lazy evaluation``

open Suave
open Suave.Types
open Suave.Web
open Suave.Http
open Suave.Http.Successful
open Suave.Http.Applicatives
open Suave.Http.RequestErrors

open System

let stale = OK (sprintf "%s" (DateTimeOffset.UtcNow.ToString("o")))

let fresh =
  warbler (fun _ ->
    // warbler takes a lambda, meaning it will evaluate it every request, as
    // opposed to the above 'stale' WebPart, which is evaluated when the server
    // is started, as a module-global value (static).
    OK (sprintf "%s" (DateTimeOffset.UtcNow.ToString("o")))
  )

let main argv =
  // this means "evaluate the right hand side before passing that
  // evaluated value to startWebServer", so in this case we're
  // evaluating the choose applicative before passing the WebPart
  // into startWebServer.
  startWebServer defaultConfig <| choose [
    path "/fresh" >>= fresh // always new date responded with

    path "/stale" >>= stale // always same date responded with

    NOT_FOUND "Either request /fresh or /stale."
  ]
  0
