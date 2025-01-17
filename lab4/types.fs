module Types

open Consts
open Spectre.Console

// Типы данных
type Position = { X: int; Y: int }

type Direction =
    | Up
    | Down
    | Left
    | Right
    | RightDown
    | LeftDown

type Projectile =
    { Speed: int
      Direction: Direction
      Position: Position
      Color: Color }

type Ship =
    { Position: Position
      Color: Color
      HP: int }

type EnemyPattern =
    { MoveDirection: Direction
      ShootDirection: Direction
      ShootInterval: int
      MoveInterval: int
      ShootSpeed: int }

type Enemy =
    { Pattern: EnemyPattern
      Ship: Ship
      HP: int }


type GameState =
    { Player: Ship
      Enemies: Enemy list
      Projectiles: Projectile list
      Score: int
      Widht: int
      Height: int
      Tick: int }


let createShip (x: int) (y: int) (color: Color) (hp: int) : Ship =
    { Position = { X = x; Y = y }
      Color = color
      HP = hp }

let movePosition position direction =
    match direction with
    | Left -> { position with X = position.X - 1 }
    | Right -> { position with X = position.X + 1 }
    | Up -> { position with Y = position.Y - 1 }
    | Down -> { position with Y = position.Y + 1 }
    | RightDown ->
        { X = position.X + 1
          Y = position.Y + 1 }
    | LeftDown ->
        { X = position.X - 1
          Y = position.Y + 1 }

let moveShip (ship: Ship) (direction: Direction) : Ship =
    { ship with
        Position = movePosition ship.Position direction }

let moveProjectile (projectile: Projectile) =
    { projectile with
        Position = movePosition projectile.Position projectile.Direction }

let shootProjectile (position: Position) (direction: Direction) speed : Projectile =
    { Speed = speed
      Direction = direction
      Position = { X = position.X; Y = position.Y }
      Color = if direction = Up then Color.Aqua else EnemyProjColor }
    |> moveProjectile


let moveProjectiles (projectiles: Projectile list) : Projectile list =
    projectiles
    |> List.map (fun p ->
        match p.Direction with
        | Up ->
            { p with
                Position = { p.Position with Y = p.Position.Y - 1 } }
        | Down ->
            { p with
                Position = { p.Position with Y = p.Position.Y + 1 } }
        | Left ->
            { p with
                Position = { p.Position with X = p.Position.X - 1 } }
        | Right ->
            { p with
                Position = { p.Position with X = p.Position.X + 1 } }
        | _ -> p)

let ReduceEnemyHpConditional (list: Enemy list) (flag: bool) : Enemy list =
    match list with
    | [] -> []
    | head :: tail ->
        tail
        @ (if flag then
               [ { head with HP = head.HP - 1 } ]
           else
               [ head ])

let detectCollisions (projectiles: Projectile list) (enemies: Enemy list) : Enemy list * Projectile list =
    let rec detectHit (enemies: Enemy list) (projectiles: Projectile list) (count: int) =
        match count with
        | 0 -> (enemies, projectiles)
        | count ->
            let enemy = enemies.Head

            detectHit
                (ReduceEnemyHpConditional
                    enemies
                    (List.exists (fun (proj: Projectile) -> proj.Position = enemy.Ship.Position && proj.Color<>EnemyProjColor) projectiles))
                (projectiles |> List.filter (fun p -> p.Position <> enemy.Ship.Position || (p.Color = EnemyProjColor && p.Position = enemy.Ship.Position)))
                (count - 1)

    detectHit enemies projectiles enemies.Length


let drawBorders  (width: int) (heignt: int) (canvas: Canvas) : Canvas =
    for x in 0 .. (width - 1) do
        canvas.SetPixel(x, 0, Color.Red) |> ignore
        canvas.SetPixel(x, heignt - 1, Color.Red) |> ignore

    for y in 0 .. (heignt - 1) do
        canvas.SetPixel(0, y, Color.Green) |> ignore
        canvas.SetPixel(width - 1, y, Color.Green) |> ignore

    canvas

let positionInBorders position width height =
    position.X < width-1 && position.X > 0 && position.Y > 0 && position.Y < height-1
