module Static

open Consts
open Types
open Spectre.Console


let defaultEnemyPattern =
    { MoveDirection = RightDown
      ShootDirection = Down
      ShootInterval = 60
      MoveInterval = 30
      ShootSpeed = 10 }

let defaultEnemy =
    { Pattern = defaultEnemyPattern
      Ship = createShip (WIDTH * 2 / 3) 1 Color.Red 1
      HP = 1 }

let drawHeart (x: int) (y: int) : (int * int) list =
    [ (x + 1, y)
      (x + 2, y)
      (x + 4, y)
      (x + 5, y)
      (x, y + 1)
      (x + 1, y + 1)
      (x + 2, y + 1)
      (x + 3, y + 1)
      (x + 4, y + 1)
      (x + 5, y + 1)
      (x + 6, y + 1)
      (x + 1, y + 2)
      (x + 2, y + 2)
      (x + 3, y + 2)
      (x + 4, y + 2)
      (x + 5, y + 2)
      (x + 2, y + 3)
      (x + 3, y + 3)
      (x + 4, y + 3)
      (x + 3, y + 4) ]


let enemyMap =
    [
    (300,
       [    { defaultEnemy with
                Pattern.MoveDirection = RightDown 
                Ship = {defaultEnemy.Ship with Ship.Position.X = (WIDTH * 2 / 3)}} 
            { defaultEnemy with
                Pattern.MoveDirection = Down 
                Ship = {defaultEnemy.Ship with Ship.Position.X = (WIDTH / 2)}}
            { defaultEnemy with
                Pattern.MoveDirection = LeftDown 
                Ship = {defaultEnemy.Ship with Ship.Position.X = (WIDTH  / 3)}}
        ]) 
    (550,
       [    { defaultEnemy with
                Pattern.MoveDirection = LeftDown 
                Ship = {defaultEnemy.Ship with Ship.Position.X = (WIDTH * 2 / 3)}} 
            { defaultEnemy with
                Pattern.MoveDirection = RightDown 
                Ship = {defaultEnemy.Ship with Ship.Position.X = (WIDTH  / 3)}}
        ])
    (1050,
       [    { defaultEnemy with
                Pattern.MoveDirection = RightDown 
                Pattern.MoveInterval = 20
                Ship = {defaultEnemy.Ship with Ship.Position.X = (WIDTH * 2 / 3)}} 
            { defaultEnemy with
                Pattern.MoveDirection = RightDown 
                Pattern.MoveInterval = 20
                Ship = {defaultEnemy.Ship with Ship.Position.X = (WIDTH  / 3)}}
        ])
    ]
