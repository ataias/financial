module Tests

open System
open System.IO
open Xunit
open Interest
open Interest.Interest

let lowValue = -0.001
let highValue = 0.001


[<Fact>]
let ``Test interest for one month without delta``() =
    let rate = 0.1 / 100.0
    let amount = 10000.
    let delta = 0.
    let months = 1
    let newAmount = interest rate amount delta months
    Assert.Equal(amount * (1. + rate) ** 1., newAmount)

[<Fact>]
let ``Test interest for 5 months without delta``() =
    let rate = 0.1 / 100.0
    let amount = 10000.
    let delta = 0.
    let months = 5
    let newAmount = interest rate amount delta months
    // comparison of doubles is tricky, so accepting range here
    Assert.InRange(amount * (1. + rate) ** 5. - newAmount, lowValue, highValue)

[<Fact>]
let ``Import EasyInvest CSV data into a format parsable later``() =
    let parsed =
        read EasyInvest "../../../../data/easyinvest-2019-no-sep.csv"
        |> Array.rev
        |> Array.take 3

    let (expected: InvestmentInfo []) =
        [| { Broker = EasyInvest
             Qty = 20.0M
             Investment = "Ações"
             Description = "ACTI01"
             DueDate = None
             AgreedRate = None
             InitialAmount = None
             GrossAmount = 2571.6
             NetAmount = None }
           { Broker = EasyInvest
             Qty = 2.0M
             Investment = "Tesouro Direto"
             Description = "Tesouro 5"
             DueDate = Some (DateTime(2035, 5, 15))
             AgreedRate = Some "IPCA"
             InitialAmount = Some 2615.3
             GrossAmount = 3636.08
             NetAmount = Some 3455.57 }
           { Broker = EasyInvest
             Qty = 1.10M
             Investment = "Tesouro Direto"
             Description = "Tesouro 4"
             DueDate = Some (DateTime(2035, 5, 15))
             AgreedRate = Some "IPCA"
             InitialAmount = Some 3648.25
             GrossAmount = 4616.28
             NetAmount = Some 4444.49 } |]

    Assert.Equal(expected.Length, parsed.Length)
    // Calling Assert.Equal for each element allows us to see a more
    // human-readable error message when the test fails
    for (e, p) in Array.zip expected parsed do
      Assert.Equal(e, p)
