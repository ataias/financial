module Tests

open System
open Xunit
open Interest

let lowValue = -0.001
let highValue = 0.001

[<Fact>]
let ``Test interest for one month without delta`` () =
    let rate = 0.1 / 100.0
    let amount = 10000.
    let delta = 0.
    let months = 1
    let newAmount = Interest.interest rate amount delta months
    Assert.Equal(amount * (1. + rate) ** 1., newAmount)

[<Fact>]
let ``Test interest for 5 months without delta`` () =
    let rate = 0.1 / 100.0
    let amount = 10000.
    let delta = 0.
    let months = 5
    let newAmount = Interest.interest rate amount delta months
    // comparison of doubles is tricky, so accepting range here
    Assert.InRange(amount * (1. + rate) ** 5. - newAmount, lowValue, highValue)
