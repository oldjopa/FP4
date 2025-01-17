open System
open game
open Types

[<EntryPoint>]
let main argv =
    let initialState = initGame ()
    gameLoop initialState
    0