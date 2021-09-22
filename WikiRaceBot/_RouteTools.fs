module WikiRaceBot._RouteTools

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

let private strToBytes (str: string) = System.Text.Encoding.UTF8.GetBytes str

let private bytesToStr base64 = System.Convert.FromBase64String base64

let encodeRoute route =
    let routeBytes = route
                     |> List.map strToBytes
    let joinedBytes = routeBytes
                      |> List.fold (fun current next -> current @ [byte 0] @ (next |> Array.toList)) []
    
    let compressor = new ZstdNet.Compressor()
    let compressed = compressor.Wrap (joinedBytes |> List.toArray)
    System.Convert.ToBase64String compressed

let splitByNulls bytes =
    let rec splitAtIndexes indexes list =
        match indexes with
        | [] -> ()
        | _ -> ()
    
    let nullIndexes = bytes
                      |> List.mapi (fun i b -> if b = 0 then Some(i) else None)
                      |> List.choose id
    
    splitAtIndexes nullIndexes

let decodeRoute route =
    let compressed = System.Convert.FromBase64String route
    
    let decompressor = new ZstdNet.Decompressor()
    let decompressed = decompressor.Unwrap compressed
    []