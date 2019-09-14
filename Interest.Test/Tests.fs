module Tests

open System
open System.IO
open Xunit
open Interest

let lowValue = -0.001
let highValue = 0.001

let data =
  @"
  ""TIPO DE INVESTIMENTO"";""DESCRIÇÃO"";""VENCIMENTO"";""TAXA NEGOCIADA"";""QUANTIDADE"";""VALOR APLICADO"";""VALOR BRUTO"";""VALOR LÍQUIDO""
  ""CDB"";""BANCO MAXIMA"";31/07/2020;""124% do CDI"";4,0;R$4.000,00;R$4.350,94;R$4.289,53
  ""CDB"";""BANCO LUSO BRASILEIRO"";29/11/2019;""119% do CDI"";3,0;R$3.000,00;R$3.420,57;R$3.346,97
  ""CDB"";""BANCO MAXIMA"";04/08/2025;""126% do CDI"";2,0;R$2.000,00;R$2.004,01;R$2.001,15
  ""CDB"";""BANCO MAXIMA"";23/11/2020;""125% do CDI"";3,0;R$3.000,00;R$3.439,70;R$3.362,75
  ""CDB"";""BANCO BMG"";08/12/2022;""IPC-A + 6,55%"";3000,0;R$3.000,00;R$3.565,21;R$3.466,30
  ""CDB"";""BANCO MAXIMA"";23/11/2020;""125% do CDI"";1,0;R$1.000,00;R$1.146,56;R$1.120,91
  ""LC"";""SOCINAL"";14/03/2025;""127% do CDI"";2,0;R$2.000,00;R$2.066,46;R$2.051,51
  ""Tesouro Direto"";""Tesouro Selic 2023"";01/03/2023;""SELIC"";0,66;R$5.961,82;R$6.806,11;R$6.675,98
  ""Tesouro Direto"";""Tesouro IPCA+ 2024"";15/08/2024;""IPCA"";1,64;R$3.382,71;R$4.589,87;R$4.405,82
  ""Tesouro Direto"";""Tesouro Prefixado 2025"";01/01/2025;""PREFIXADO"";0,55;R$345,57;R$383,20;R$374,55
  ""Tesouro Direto"";""Tesouro IPCA+ com Juros Semestrais 2035"";15/05/2035;""IPCA"";1,10;R$3.648,25;R$4.616,28;R$4.444,49
  ""Tesouro Direto"";""Tesouro IPCA+ 2035"";15/05/2035;""IPCA"";2,0;R$2.615,30;R$3.636,08;R$3.455,57
  ""Ações"";""SPXI11"";;"""";20,0;;R$2.571,60;
  "

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
    let parsed = Interest.read (Interest.EasyInvest) "/Users/ataias/easyinvest-2019-no-sep.csv"
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
