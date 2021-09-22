module WikiRaceBot.Program

open System
open System.Threading.Tasks
open Discord
open Discord.WebSocket
open WikiRaceBot._WikiParser

// helper func for awaiting System.Threading.Task objects
let await (task: Task) = task |> Async.AwaitTask |> Async.RunSynchronously


let log (msg: LogMessage) =
    
    printfn $"%s{msg.ToString()}"
    
    Task.CompletedTask

let msgReceived (client: DiscordSocketClient) (msg: SocketMessage) =
    
    if msg.Author.Id <> client.CurrentUser.Id then
        if msg.Content.StartsWith "%genroute" then
            let cmdArgs = msg.Content.[9..].Trim().Split(' ') // trim command off the start
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
            let joined = route |> List.fold (fun current next -> $"%s{current}\n<%s{next}>") ""
            await <| msg.Channel.SendMessageAsync ($"Generated route with depth %i{depth} in %f{sw.Elapsed.TotalSeconds}s:" + joined)
    
    Task.CompletedTask

let ready (client: DiscordSocketClient) () =
    
    client.SetGameAsync("WikiRace", ``type`` = ActivityType.Playing) |> await
    
    Task.CompletedTask

let botMain token = async {
        
        let client = new DiscordSocketClient()
        
        client.add_Log (Func<LogMessage,Task> log)
        client.add_MessageReceived (Func<SocketMessage, Task> (msgReceived client))
        client.add_Ready (Func<Task> (ready client))
        
        await <| client.LoginAsync(TokenType.Bot, token)
        
        await <| client.StartAsync()
        
        // idk why i need this but the discord.net example has it so sure
        await <| Task.Delay(-1)
    }


[<EntryPoint>]
let main _ =
    
    let token = Environment.GetEnvironmentVariable("WIKIRACE_BOT_TOKEN")
    
    botMain token |> Async.RunSynchronously
    
    0