module WikiRaceBot._WikiParser
// sorry for the _ but rider was misbehaving soo

open HtmlAgilityPack

let getRawHtml (url: string) = HtmlWeb().Load(url)

let getLinks (raw: HtmlDocument) urlRoot =
    let pageContentNode = raw.DocumentNode.Descendants()
                          |> Seq.find (fun n -> n.Id = "bodyContent")
    let allLinks = pageContentNode.Descendants "a"
    
    let desiredLinks = allLinks
                       |> Seq.where (fun n ->
                           let attrs = n.Attributes
                           attrs.Contains("href")
                           && attrs.["href"].Value.StartsWith "/wiki"
                           && not (attrs.["href"].Value.Contains ':'))
                       |> Seq.map (fun n -> urlRoot + (n.Attributes.["href"].Value.Split '#').[0].ToLower()) // remove #s!
    
    desiredLinks
    |> Seq.distinct
    |> Seq.toList

let findRoute =
    let rand = System.Random()
    
    let rec findRec (pageHistory: string list) desiredClicks nextUrl =
        let urlRoot = "https://" + System.Uri(nextUrl).DnsSafeHost
        
        if pageHistory.Length = desiredClicks then
            pageHistory
        else
            let html = getRawHtml nextUrl
            let links = getLinks html urlRoot
            let randomLink = links.[rand.Next(links.Length)]
            findRec (pageHistory @ [nextUrl]) desiredClicks randomLink
    
    findRec []