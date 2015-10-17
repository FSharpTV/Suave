module ``Browsing for files``

open System
open System.IO
open System.Reflection

// you can copy-and-paste this:
let rootPath =
  Assembly.GetExecutingAssembly().CodeBase
  |> fun s -> (Uri s).AbsolutePath
  |> Path.GetDirectoryName

open Suave
open Suave.Types
open Suave.Http
open Suave.Web
open Suave.Http.Files

let filesInHome : WebPart =
  // browse the home folder
  Files.browseHome

// you can easily read the list of files in your browser:
let catDir = Path.Combine(rootPath, "cats")

// printfn "Cat dir: %s" catDir
let allCats = Files.dir catDir

// or you can 'jump into' the 'cats' folder and serve from there:
let servedCats = Files.browse catDir

let main argv =
  startWebServer defaultConfig servedCats
  0
