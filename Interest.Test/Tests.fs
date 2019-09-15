module Tests

open System
open System.IO
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

[<Fact>]
let ``Finance should be able to import EasyInvest CSV data into a format parsable later`` () =
    // let file = "../data/easyinvest-2019-09-14.csv"

    // let data = File.ReadAllLines file
    // let (data:string) =
    //     data.Split([|'\n'|])
    //     |> fun x -> Array.sub x 0 1 // only first elements to make it easier for now
    //     |> String.concat "\n"
    let parsed = Interest.read (Interest.EasyInvest) "../../../../data/easyinvest-2019-no-sep.csv"
    let (expected:Interest.InvestmentInfo[]) = [|
          { Broker=Interest.EasyInvest
            Qty=4.0M
            Investment="CDB"
            Description="BANCO MAXIMA"
            DueDate="31/07/2020"
            AgreedRate="124% do CDI"
            InitialAmount="R$4.000,00"
            GrossAmount="R$4.350,94"
            NetAmount="R$4.289,53"}
        |]
    Assert.Equal<Interest.InvestmentInfo[]>(expected, parsed)
