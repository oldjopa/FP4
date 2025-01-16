module game

open System
open Spectre.Console
open Types
open System.Threading

let WIDTH = 40
let HEIGHT = 40

// Инициализация игры
let initGame () : GameState =
    let player = createShip 10 35 Color.Cyan1 5

    let enemies =
        [ { Pattern =
              { MoveDirection = RightDown
                ShootDirection = Down
                ShootInterval = 30
                MoveInterval = 20 }
            Ship = createShip 5 5 Color.Red 1
            HP = 1 }
          { Pattern =
              { MoveDirection = Down
                ShootDirection = Down
                ShootInterval = 10
                MoveInterval = 30 }
            Ship = createShip 10 5 Color.Red 1
            HP = 1 }
          { Pattern =
              { MoveDirection = Down
                ShootDirection = Down
                ShootInterval = 20
                MoveInterval = 30 }
            Ship = createShip 15 5 Color.Red 1
            HP = 1 } ]

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
        let newProjectile = shootProjectile state.Player.Position Up

        { state with
            Projectiles = newProjectile :: state.Projectiles }
    | _ -> state

let updateGameState (state: GameState) : GameState =
    let updatedProjectiles =
        moveProjectiles state.Projectiles
        |> List.filter (fun proj -> positionInBorders proj.Position state.Widht state.Height)

    let (hitEnemies: Enemy list), (remainingProjectiles: Projectile list) =
        detectCollisions updatedProjectiles state.Enemies

    let updatedEnemies =
        // List.filter (fun enemy -> not (List.contains enemy hitEnemies)) state.Enemies
        hitEnemies
        |> List.filter (fun enemy -> positionInBorders enemy.Ship.Position state.Widht state.Height)
        |> List.filter (fun enemy -> enemy.HP > 0)
        |> List.map (fun enemy ->
            if state.Tick % enemy.Pattern.MoveInterval = 0 then
                { enemy with
                    Ship = moveShip enemy.Ship enemy.Pattern.MoveDirection }
            else
                enemy)

    let newProjectiles =
        updatedEnemies
        |> List.fold
            (fun (acc: Projectile list) enemy ->
                if state.Tick % enemy.Pattern.ShootInterval = 0 then
                    acc @ [ shootProjectile enemy.Ship.Position enemy.Pattern.ShootDirection |> moveProjectile ]
                else
                    acc)
            remainingProjectiles

    let updatedScore = state.Score + List.length hitEnemies

    { state with
        Enemies = updatedEnemies
        Projectiles = newProjectiles
        Score = updatedScore 
        Tick = state.Tick + 1}


let renderGameState (state: GameState) : Canvas =
    let canvas = new Canvas(state.Widht, state.Height) |> drawBorders

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
            canvas.SetPixel(proj.Position.X, proj.Position.Y, Color.White) |> ignore)

    // let scoreText = sprintf "Score: %d" state.Score
    // canvas.SetText(1, 1, scoreText)

    canvas


let renderGame (state: GameState) : Canvas = renderGameState state

let rec gameLoop (state: GameState) : unit =
    let canvas = renderGame state
    AnsiConsole.Cursor.SetPosition(0, 0)

    AnsiConsole.Write(canvas)

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
