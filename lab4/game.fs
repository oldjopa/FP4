module game

open System
open Spectre.Console
open Types
open Static
open Consts
open System.Threading


// Инициализация игры
let initGame () : GameState =
    let player = createShip 10 35 Color.Cyan1 3

    let enemies = []

    let projectiles = []
    let score = 0

    { Player = player
      Enemies = enemies
      Projectiles = projectiles
      Score = score
      Widht = WIDTH
      Height = HEIGHT
      Tick = 0 }

let processInput (input: ConsoleKey) (state: GameState) : GameState =
    match input with
    | ConsoleKey.LeftArrow ->
        { state with
            Player = moveShip state.Player Left }
    | ConsoleKey.RightArrow ->
        { state with
            Player = moveShip state.Player Right }
    | ConsoleKey.Spacebar ->
        let newProjectile = shootProjectile state.Player.Position Up 2

        { state with
            Projectiles = newProjectile :: state.Projectiles }
    | _ -> state

let updateGameState (state: GameState) : GameState =
    match state.Player.HP with
    | hp when hp <= 0 -> state
    | _ ->
        let updatedProjectiles =
            state.Projectiles
            |> List.map (fun proj ->
                if state.Tick % proj.Speed = 0 then
                    moveProjectile proj
                else
                    proj)
            |> List.filter (fun proj -> positionInBorders proj.Position state.Widht state.Height)

        let (remainingEnemies: Enemy list), (remainingProjectiles: Projectile list) =
            detectCollisions updatedProjectiles state.Enemies

        let updatedEnemies =
            // List.filter (fun enemy -> not (List.contains enemy hitEnemies)) state.Enemies
            remainingEnemies
            |> List.filter (fun enemy -> positionInBorders enemy.Ship.Position state.Widht state.Height)
            |> List.filter (fun enemy -> enemy.HP > 0)
            |> List.map (fun enemy ->
                if state.Tick % enemy.Pattern.MoveInterval = 0 then
                    { enemy with
                        Ship = moveShip enemy.Ship enemy.Pattern.MoveDirection }
                else
                    enemy)

        let updatedPlayer =
            { state.Player with
                HP =
                    state.Player.HP
                    - (updatedProjectiles
                       |> List.filter (fun p -> p.Position = state.Player.Position)
                       |> List.length) }

        let newProjectiles =
            updatedEnemies
            |> List.fold
                (fun (acc: Projectile list) enemy ->
                    if state.Tick % enemy.Pattern.ShootInterval = 0 then
                        acc
                        @ [ shootProjectile enemy.Ship.Position enemy.Pattern.ShootDirection enemy.Pattern.ShootSpeed ]
                    else
                        acc)
                remainingProjectiles
            |> List.filter (fun p -> p.Position <> state.Player.Position)

        let updatedScore =
            state.Score + List.length state.Enemies - List.length updatedEnemies

        { state with
            Player = updatedPlayer
            Enemies =
                updatedEnemies
                @ (enemyMap
                   |> List.filter (fun x -> state.Tick % (fst x) = 0)
                   |> List.map (fun x -> snd x)
                   |> List.fold (@) [])
            Projectiles = newProjectiles
            Score = updatedScore
            Tick = state.Tick + 1 }



let renderGameState (state: GameState) : Canvas =
    let canvas =
        new Canvas(state.Widht, state.Height + 6)
        |> drawBorders state.Widht state.Height

    canvas.SetPixel(state.Player.Position.X, state.Player.Position.Y, state.Player.Color)
    |> ignore

    state.Enemies
    |> List.iter (fun enemy ->
        if positionInBorders enemy.Ship.Position state.Widht state.Height then
            canvas.SetPixel(enemy.Ship.Position.X, enemy.Ship.Position.Y, enemy.Ship.Color)
            |> ignore)

    state.Projectiles
    |> List.iter (fun proj ->
        if positionInBorders proj.Position state.Widht state.Height then
            canvas.SetPixel(proj.Position.X, proj.Position.Y, proj.Color) |> ignore)

    for i in [ 0 .. state.Player.HP - 1 ] do
        drawHeart (8 * i) (state.Height + 1)
        |> List.iter (fun p -> canvas.SetPixel((fst p), (snd p), Color.Red) |> ignore)

    canvas

let rec gameLoop (state: GameState) : unit =
    let canvas = renderGameState state
    AnsiConsole.Cursor.SetPosition(0, 0)
    AnsiConsole.Profile.Height = 100 |> ignore // убрать потом, не понятно что делает

    AnsiConsole.Write(canvas)

    if state.Player.HP <= 0 then
        AnsiConsole.WriteLine()
        AnsiConsole.Write("\t\t\tGAME OVER")
        AnsiConsole.WriteLine()
        AnsiConsole.Write(sprintf "\t\t\tSCORE: %i" state.Score)



    let input =
        if Console.KeyAvailable then
            Console.ReadKey(true).Key
        else
            ConsoleKey.None
    // let input = Console.ReadKey(true)
    let newState = state |> processInput input |> updateGameState

    // let updatedState = updateGameState newState

    Thread.Sleep(10)

    gameLoop newState
