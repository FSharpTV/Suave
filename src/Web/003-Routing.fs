module ``Introducing routing and applicatives``

open System
open System.IO
open System.Reflection

// you can copy-and-paste this:
let rootPath =
  Assembly.GetExecutingAssembly().CodeBase
  |> fun s -> (Uri s).AbsolutePath
  |> Path.GetDirectoryName

open ``Browsing for files``
open ``Creating your first Suave webpage``

open Suave
open Suave.Web
open Suave.Http
open Suave.Http.RequestErrors
open Suave.Http.Applicatives

let routes =
  choose [
    path "/hi" >>= hi
    path "/bye" >>= bye
    path "/cute-cat" >>= Files.file (Path.Combine(rootPath, "3.jpg"))
    servedCats
    NOT_FOUND "The cat was not found here"
  ]

let main argv =
  startWebServer defaultConfig routes
  0
