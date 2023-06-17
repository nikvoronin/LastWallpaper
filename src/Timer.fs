module Timer

open System.Threading
open System
let create (interval: TimeSpan) action =
    let period = (int) interval.TotalMilliseconds
    new Timer
        ( TimerCallback (fun _ -> action ()), 0
        , period, period
        )

/// Creates then immediately starts the timer
let startImmediate (interval: TimeSpan) action =
    let period = (int) interval.TotalMilliseconds
    new Timer
        ( TimerCallback (fun _ -> action ()), 0
        , 0 // to start the timer immediately
        , period
        )

let change (interval: TimeSpan) (timer: Timer) =
    let period = (int) interval.TotalMilliseconds
    timer.Change (period, period)
    |> ignore

let stop (timer: Timer) =
    timer.Change (Timeout.Infinite, Timeout.Infinite) |> ignore
    timer.Dispose ()
    |> ignore
