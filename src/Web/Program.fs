open Suave

module HelloWorldModule =
  let okRes = Successful.OK "Hello World!"

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
 

[<EntryPoint>]
let main argv =
  startWebServer defaultConfig FilesModule.cats
  0
