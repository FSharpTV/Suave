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
open Suave.Operators
open Suave.Filters
open Suave.Successful
open Suave.RequestErrors

// type WebPart = HttpContext -> Async<HttpContext option>

let routes : WebPart =
  // choose calls each WebPart in turn until one returns `Some httpContext`.
  choose [
    // >>= means; first try '/hi' and if that matches, try the `hi` from
    // the ``Creating your first Suave webpage`` module (which always succeeds,
    // i.e. returns `Some httpContext`).
    path "/hi" >=> hi

    path "/bye" >=> bye

    path "/cute-cat" >=> Files.file (Path.Combine(rootPath, "3.jpg"))

    // path may match, but `never` makes it fail
    path "/never" >=> never

    // this is actually the implementation
    path "/never-impl" >=> fun ctx -> fail

    // and `fail = async.Return None`

    // you can inspect values from the request using the `request` applicative:
    path "/tell-me" >=> request (fun r -> OK (sprintf "You requested path: %s" r.url.AbsolutePath))

    // another way to write it (remember the signature of WebPart):
    // and that `val OK : string -> WebPart`.
    path "/tell-me" >=> request (fun r -> fun httpContext -> async {
      return! OK (sprintf "You requested path: %s" r.url.AbsolutePath) httpContext
    })

    // yet another way, fully expanded (behind the scenes):
    path "/tell-me" >=> request (fun r -> fun httpContext -> async {
      let buf = Text.Encoding.UTF8.GetBytes (sprintf "You requested path: %s" r.url.AbsolutePath)

      // -> HttpContext option
      return Some
        // extending the record with new values, to create a new value
        { httpContext with
            response =
              { httpContext.response with
                  // Bytes is the normal in-memory result value
                  content = Bytes buf } }
    })

    // if all the above web parts fail to resolve, serve a cat instead:
    servedCats

    // and if the path is not e.g. "/1.jpg", say so:
    NOT_FOUND "The path did neither match greetings nor find any cats."
  ]


let userDataAndCallbacks =
  // this will work, because the HttpContext value from `setUserData` is passed
  // alone as long as `Some ctx` is returned
  path "/callbacks" >=> Writers.setUserData "myKey" "catscatscats" >=> warbler (fun ctx ->
    OK (sprintf "Your key's value is: %s" (ctx.userState |> Map.find "myKey" |> unbox))
  )

let userDataAndCallbacks2 =
  // this will won't work, will crash in `Map.find`.
  choose [
    path "/callbacks" >=> Writers.setUserData "myKey" "catscatscats" >=> never

    // choose continues downward:
    path "/callbacks" >=> warbler (fun ctx ->
      OK (sprintf "Your key's value is: %s" (ctx.userState |> Map.find "myKey" |> unbox))
    ) // => 500 Internal Server Error: The given key was not present in the dictionary.
  ]

// OR you can let a custom web part call your callback:
let userDataAndCallbacks3 : WebPart =

  let myApp (callback : string -> WebPart) =
    let haveSomeCats = "catscatscats"
    callback haveSomeCats

  choose [
    path "/callbacks" >=> myApp (fun myValue ->
      OK (sprintf "Your key's value is: %s" myValue)
    )
  ]

let main argv =
  startWebServer defaultConfig userDataAndCallbacks3
  0