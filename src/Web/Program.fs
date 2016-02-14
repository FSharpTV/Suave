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

      pathScan "/hi/%s" (fun name -> OK (sprintf "Hi, %s!" name))
      pathScan "/bye/%s" (fun name -> OK (sprintf "Good bye, %s!" name))

      never >=> request (fun r -> OK (sprintf "You requested %s" r.url.AbsolutePath))

      path "/test-context" >=> context (fun c ->
        let state =
          c.userState
          |> Map.add "message" (sprintf "You requested (ctx) %s" c.request.url.AbsolutePath |> box)
        fun ctx -> async.Return(Some { ctx with userState = state })
      ) >=> warbler (fun ctx -> OK (ctx.userState.["message"] :?> string))

      path "/responses" >=> request (fun r ->
        fun ctx -> async.Return (Some ctx)
      )
      path "/responses2" >=> request (fun r ->
        fun ctx -> 
          async {
            return Some ctx
          }
      )
      path "/responses3" >=> request (fun r ->
        let bs = System.Text.Encoding.UTF8.GetBytes "Hello"
        fun ctx -> 
          async {
            return Some
              { ctx with
                  response =
                    { ctx.response with
                        content = Bytes bs } }
          }
      )

      RequestErrors.NOT_FOUND "Not found"
    ]

[<EntryPoint>]
let main argv =
  startWebServer defaultConfig RoutingModule.app
  0
