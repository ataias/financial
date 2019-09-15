// Learn more about F# at http://fsharp.org

open System
open Interest

[<EntryPoint>]
let main argv =
    let (top, left, data) = Interest.simulate 15
    let table = Interest.createTable "Amount +15y" (Array2D.map fst data) top left
    Interest.printArray table
    printf "\n"
    let table = Interest.createTable "Monthly Return +15y" (Array2D.map snd data) top left
    Interest.printArray table
    0 // return an integer exit code
