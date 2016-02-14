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

module LazyModule =
  open System
  open Suave.Filters
  open Suave.Successful
  open Suave.Operators

  let app =
    choose [
      path "/eager" >=> OK (sprintf "%O" DateTimeOffset.Now)
      path "/lazy" >=> context (fun _ -> OK (sprintf "%O" DateTimeOffset.Now))

      path "/challenge" >=> context (fun _ ->
        let now = DateTimeOffset.Now
        if now.Second % 2 = 0 then OK "hi" else OK "bye"
      )
    ]

module IndexModule =
  open Suave
  open Suave.Successful
  open Suave.RequestErrors
  open Suave.Operators
  open Suave.Filters

  let app =
    choose [
      GET >=> path "/" >=> Files.browseFileHome "index.html"
      GET >=> path "/index.html" >=> Files.browseFileHome "index.html"
      POST >=> request (fun r -> 
        match r.formData "name" with
        | Choice1Of2 name when name.Length <> 0 ->
          OK (sprintf "Got your message, %s" name)

        | Choice1Of2 _ ->
          BAD_REQUEST "Please provide your name"

        | Choice2Of2 error ->
          BAD_REQUEST error)

      Writers.setStatus HTTP_404 >=> Files.browseFileHome "not_found.html"
    ]

[<EntryPoint>]
let main argv =
  startWebServer defaultConfig IndexModule.app
  0
