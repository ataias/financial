namespace Interest
open System
open FSharp.Data

module Interest =

  type Header = {
      Value: float
      Symbol: string
  }

  type Broker =
      | EasyInvest

  type InvestmentInfo = {
      Broker: Broker
      Qty: decimal
      Investment: string
      Description: string
      DueDate: string //DateTime option
      AgreedRate: string //float option
      InitialAmount: string //decimal
      GrossAmount: string //decimal
      NetAmount: string
  }


  type EasyInvestTable = CsvProvider<"../data/easyinvest-2019-no-sep.csv", ";">

  let rec public interest rate amount delta n =
      match n with
          | 0 -> amount
          | _ ->
              interest rate (amount*(1.+rate) + delta) delta (n-1)


  /// <summary>
  /// Simulates evolution of capital for different interest rates and
  /// different injections of money per month for a given amount of years
  /// </summary>
  let public simulate nYears =
      let rates = [for i in 1..15 -> 0.1*(float i)/100.0]
      let n = rates.Length
      let initialAmount = 120000.0
      let delta = [for i in 10..50 -> float (i * 100)]
      let m = delta.Length
      let months = 12 * nYears
      let getAmount i j =
        let amount = interest rates.[i] initialAmount delta.[j] months
        let nextInterest = amount * rates.[i]
        (amount, nextInterest)
      let matrix = Array2D.init n m getAmount
      let topHeader = List.map (fun x -> { Value = x * 100.0; Symbol = "%" }) rates
      let leftHeader = List.map (fun x -> { Value = x; Symbol = "R$" }) delta
      (topHeader, leftHeader, matrix)

  let public printArray (arr: string [,]): unit =
      for line = 0 to Array2D.length1 arr - 1 do
          if line = 1 then
              printfn "|-"
          for column = 0 to Array2D.length2 arr - 1 do
              // printfn "(%d, %d)" line column
              printf "| %s " arr.[line, column]
          printfn "|"

  let public createTable title (data: float[,]) (top:Header list) (left: Header list) : string[,] =
      let genElement i j =
          match (i, j) with
              | 0, j when j > 0 ->
                  let h = top.[j-1]
                  sprintf "%.2f%s" h.Value h.Symbol
              | i, 0 when i > 0 ->
                  let h = left.[i-1]
                  sprintf "%s %.2f" h.Symbol h.Value
              | i, j when i > 0 && j > 0 ->
                  data.[j-1, i-1].ToString("#,##0.00")

              | _, _ -> title
      let n = left.Length + 1
      let m = top.Length + 1
      Array2D.init n m genElement

  let public read (broker:Broker) (uri:string) : InvestmentInfo[] =
      let convert (e:EasyInvestTable.Row) : InvestmentInfo =
          { Broker=EasyInvest
            Qty=e.``QUANTIDADE``
            Investment=e.``TIPO DE INVESTIMENTO``
            Description=e.``DESCRIÇÃO``
            DueDate=e.``VENCIMENTO``
            AgreedRate=e.``TAXA NEGOCIADA``
            InitialAmount=e.``VALOR APLICADO``
            GrossAmount=e.``VALOR BRUTO``
            NetAmount=e.``VALOR LÍQUIDO``}
      // let convert (e:EasyInvestTable.Row) : InvestmentInfo =
      //     { Broker=EasyInvest
      //       Qty=4.
      //       Investment="2"
      //       Description="2"
      //       DueDate="2"
      //       AgreedRate="2"
      //       InitialAmount="2"
      //       GrossAmount="2"
      //       NetAmount="2" }

      match broker with
          | EasyInvest ->
              let data = EasyInvestTable.Load(uri)
              Seq.map convert data.Rows
              |> Array.ofSeq
