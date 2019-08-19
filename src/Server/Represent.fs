module Server.Represent

open Newtonsoft.Json
open System
open System.IO

open Fable.Helpers.ReactServer
open Freya.Core
open Freya.Machines.Http
open Freya.Types.Http
open Freya.Optics.Http
open ServerCode.FableJson
open Freya.Machines


let jsonConverter = Fable.JsonConverter() :> JsonConverter

let json<'a> value =
    let data =
        JsonConvert.SerializeObject(value, [|jsonConverter|])
        |> Text.UTF8Encoding.UTF8.GetBytes
    let desc =
        { Encodings = None
          Charset = Some Charset.Utf8
          Languages = None
          MediaType = Some MediaType.Json }
    { Data = data
      Description = desc }

let html (value:string) =
    let data = Text.UTF8Encoding.UTF8.GetBytes value
    let desc =
        {
            Encodings = None
            Charset = Some Charset.Utf8
            Languages = None
            MediaType = Some MediaType.Html
        }

    { Data = data ; Description = desc}
    
let react htmlNode =
    renderToString htmlNode
    |> html

let readBody =
    Freya.Optic.get Request.body_
    |> Freya.map (fun body ->
        using(new StreamReader (body))(fun reader -> 
            reader.ReadToEnd ()
        )
    )

let readJson<'t> =
    readBody
    |> Freya.map (ofJson<'t>)