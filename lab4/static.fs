module Static

open Types
open Spectre.Console


let WIDTH = 40
let HEIGHT = 40

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
    [ (100,
       [ { Pattern =
             { MoveDirection = RightDown
               ShootDirection = Down
               ShootInterval = 60
               MoveInterval = 30
               ShootSpeed = 10 }
           Ship = createShip (WIDTH * 2 / 3) 5 Color.Red 1
           HP = 1 }
         { Pattern =
             { MoveDirection = Down
               ShootDirection = Down
               ShootInterval = 60
               MoveInterval = 30
               ShootSpeed = 10 }
           Ship = createShip (WIDTH / 2) 5 Color.Red 1
           HP = 1 }
         { Pattern =
             { MoveDirection = LeftDown
               ShootDirection = Down
               ShootInterval = 60
               MoveInterval = 30
               ShootSpeed = 10 }
           Ship = createShip (WIDTH / 3) 5 Color.Red 1
           HP = 1 } ])
      (300,
       [ { Pattern =
             { MoveDirection = RightDown
               ShootDirection = Down
               ShootInterval = 60
               MoveInterval = 30
               ShootSpeed = 10 }
           Ship = createShip (WIDTH * 2 / 3) 5 Color.Red 1
           HP = 1 }
         { Pattern =
             { MoveDirection = Down
               ShootDirection = Down
               ShootInterval = 60
               MoveInterval = 30
               ShootSpeed = 10 }
           Ship = createShip (WIDTH / 2) 5 Color.Red 1
           HP = 1 }
         { Pattern =
             { MoveDirection = LeftDown
               ShootDirection = Down
               ShootInterval = 60
               MoveInterval = 30
               ShootSpeed = 10 }
           Ship = createShip (WIDTH / 3) 5 Color.Red 1
           HP = 1 } ])

      ]
