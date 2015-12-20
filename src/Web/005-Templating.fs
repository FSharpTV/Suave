module ``Using a server side template``

open System
open System.IO
open System.Reflection

// you can copy-and-paste this:
let rootPath =
  Assembly.GetExecutingAssembly().CodeBase
  |> fun s -> (Uri s).AbsolutePath
  |> Path.GetDirectoryName

open System
open Suave
open Suave.Successful
open Suave.Filters
open Suave.RequestErrors
open Suave.DotLiquid
open Suave.Utils // for ThreadSafeRandom
open Suave.Operators

DotLiquid.setTemplatesDir (__SOURCE_DIRECTORY__ + "/templates")

type Image =
  { href : string
    title : string }

let staticPictures =
  choose [
    // this will first serve matching cat pictures
    ``Browsing for files``.servedCats

    // falling through to serving the index page:
    page "image-of-the-day.liquid" { href = "/1.jpg"; title = "The best cat ever" }
  ]

let pictureOfTheDay =

  let catFolder =
    Path.Combine(rootPath, "cats")

  let (files : string []), (fileToDescription : Map<string, string>) =
    File.ReadAllLines(Path.Combine(catFolder, "descriptions.tsv"))
    |> Array.map (fun s -> s.Split('\t'))
    // transform the array of two items into a F# tuple; we know its
    // two-values-shape, so ignore the warning:
    |> Array.map (function
      | [| fileName; description; _ |] -> fileName, description
      | _ -> failwith "data received is not matching expectation of descriptions.tsv")
    // do something of the sequence of FileName * Description
    |> fun values -> 
      values |> Array.map fst, // make an array of all file names (not paths)
      Map.ofSeq values // make a mapping between file name and their descriptions

  choose [
    ``Browsing for files``.servedCats

    // needs to be Lazy, so we use warbler:
    warbler (fun _ ->
      // remember; in Suave, you may have multiple concurrent requests going:
      let index = ThreadSafeRandom.next 0 files.Length

      // first find the image for the found index:
      let imageFile = files.[index]

      // the find the description for that file:
      let title = fileToDescription.[imageFile]

      // serve the page with this image:
      page "image-of-the-day.liquid" { href = sprintf "/%s" imageFile; title = title }
    )
  ]

let main argv =
  startWebServer defaultConfig pictureOfTheDay
  0
