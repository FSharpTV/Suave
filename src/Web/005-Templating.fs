module ``Using a server side template``

open System
open Suave
open Suave.Types
open Suave.Web
open Suave.Http
open Suave.Http.Successful
open Suave.Http.Applicatives
open Suave.Http.RequestErrors
open Suave.DotLiquid

DotLiquid.setTemplatesDir (__SOURCE_DIRECTORY__ + "/templates")

type Image =
  { href : string
    title : string }

let app =
  choose [
    // this will first serve matching cat pictures
    ``Browsing for files``.servedCats

    // falling through to serving the index page:
    page "image-of-the-day.liquid" { href = "/1.jpg"; title = "The best cat ever" }
  ]

let main argv =
  startWebServer defaultConfig app
  0
