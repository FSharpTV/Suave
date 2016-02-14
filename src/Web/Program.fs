open Suave

module HelloWorldModule =
  let okRes = Successful.OK "Hello World!"
  let bye = Successful.OK "Goodbye World!"

module FilesModule =
  open System
  open System.IO
  open System.Reflection

  // you can copy-and-paste this:
  let rootPath =
    Assembly.GetExecutingAssembly().CodeBase
    |> fun s -> (Uri s).AbsolutePath
    |> Path.GetDirectoryName

  let cats =
    Files.browse (Path.Combine (rootPath, "cats"))

  let app =
    Files.browseFile rootPath "files/hello.txt"


module RoutingModule =
  open System.IO
  open Suave.Filters
  open Suave.Successful
  open Suave.Operators

  let private aCat fileName =
    Path.Combine (FilesModule.rootPath, "cats", fileName)

  let app : WebPart =
    choose [
      path "/hi" >=> HelloWorldModule.okRes
      path "/bye" >=> HelloWorldModule.bye
      path "/never" >=> never >=> OK "DID RETURN?"
      path "/never2" >=> (fun ctx -> fail) >=> OK "DID RETURN?"
      path "/never2" >=> (fun ctx -> async.Return(None : HttpContext option)) >=> OK "DID RETURN?"
      path "/cat" >=> Files.file (aCat "3.jpg")
      pathScan "/cats/%i/image" (
        sprintf "%i.jpg"
        >> aCat
        >> Files.file)
    ]

[<EntryPoint>]
let main argv =
  startWebServer defaultConfig RoutingModule.app
  0
