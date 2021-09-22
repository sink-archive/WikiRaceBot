module WikiRaceBot._RouteTools

open System.Buffers.Text
open WikiRaceBot._WikiParser

let genRoute (cmdArgs: string []) =
    let url = cmdArgs.[1]
    let depth = int cmdArgs.[0]
    let unFormattedUrl =
        if url.StartsWith '<' && url.EndsWith '>' then
            url.[1..url.Length - 2]
        else
            url
    let sw = System.Diagnostics.Stopwatch.StartNew()
    let route = findRoute depth unFormattedUrl
    sw.Stop()
    
    (route, sw.Elapsed.TotalSeconds)

let private encodeUrl (url: string) =
    let plainTextBytes = System.Text.Encoding.UTF8.GetBytes url
    System.Convert.ToBase64String plainTextBytes

let private decodeUrl base64 =
    let plainTextBytes = System.Convert.FromBase64String base64
    System.Text.Encoding.UTF8.GetString plainTextBytes

let encodeRoute route =
    (route
    |> List.map encodeUrl
    |> List.fold (fun current next -> current + "_" + next) "").[1..]

let decodeRoute (route: string) =
    route.Split '_'
    |> Array.toList
    |> List.map decodeUrl