module ``Calling a server-side API``

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
open Suave.Types
open Suave.Web
open Suave.Http
open Suave.Http.Successful
open Suave.Http.Applicatives
open Suave.Http.RequestErrors
open Suave.Utils

module QOTD =

  let private quotes =
    [| "What a lovely day!"
       "There are 10 types of people, those who get binary and those who don't."
    |]

  let quoteInner : WebPart =
    // instead of using warbler, you can create a lambda taking the context:
    fun (ctx : HttpContext) ->
      let composed =
        OK quotes.[ThreadSafeRandom.next 0 quotes.Length]
        >>= Writers.setMimeType "text/plain; charset=utf-8"
      composed ctx


let api =
  choose [
    // browsing the quote of the day web part:
    path "/qotf" >>= QOTD.quoteInner

    // let's see if we can't find the cats also (this time only by their numbers
    // and not with their suffixed (.jpg)-string)
    pathScan "/cats/%d" (fun number ->
      Files.browseFile rootPath (sprintf "%d.jpg" number)
    )
  ]

// same pattern as in the other modules:
let main argv =
  // we let WebPack place its files in /src/build/public as seen from the root
  // of the git repository:
  let publicDirSimple =
    // suffixed "simple", because what source directory this file is in is only
    // usable for as long as you're running on your own computer, but not when
    // the site has been deployed. 'rootPath' is better for that, and even
    // better is finding the root path in main and passing it into any function
    // that needs to look at the file system.
    Path.GetFullPath (Path.Combine(__SOURCE_DIRECTORY__, "../../build/public"))

  // configure suave to default to serving files from the build/public folder
  let suaveConfig = { defaultConfig with homeFolder = Some publicDirSimple }
  startWebServer suaveConfig <|
    choose [
      // first, try to match the API paths:
      api

      // otherwise, see if a file is present for the path
      Files.browseHome 

      // because web browsers using the History WebApi may make requests to the
      // currently showing path in the address bar, let's serve index.html
      // by default and let javascript take care of deep-linking/showing the
      // right page for the request
      Files.browseFileHome "index.html"

      // you'll reach this case if you've forgot to run 'npm install' before
      // starting the web server
      ServerErrors.INTERNAL_ERROR (sprintf "Please place your index.html in %s"
                                           (suaveConfig.homeFolder |> Option.fold (fun s t -> t) "[not configured]"))
    ]
  0
