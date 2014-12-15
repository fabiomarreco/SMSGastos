// Learn more about F# at http://fsharp.net

module SMSParser
open System
open System.IO
open System.Text.RegularExpressions
open System.Globalization
(****************** INICIO DO PROGRAMA ***************)
let thrd (_, _, c) = c

//DEFINICOES DE Active Patterns
let (|ParseRegex|_|) regex str = 
    let m = Regex(regex).Match(str)
    if m.Success 
    then Some (List.tail [ for x in m.Groups -> x.Value] )
    else None

let (|Valor|_|) str = 
    match Double.TryParse (str, NumberStyles.Currency, CultureInfo.GetCultureInfo("pt-br")) with
    | (true, value) -> Some value
    | _ -> None;;

let (|Integer|_|) str = 
    match System.Int32.TryParse (str) with
    | (true, value) -> Some value
    | _ -> None;;

let (|DateTime|_|) str = 
    match System.DateTime.TryParse (str, CultureInfo.GetCultureInfo("pt-br"), DateTimeStyles.None) with
    | (true, value) -> Some value
    | _ -> None;;

let (|Hora|_|) str =
    match str with
    | ParseRegex @"(\d+)[h\.:](\d+)" [Integer hora; Integer min] -> Some (System.TimeSpan (hora, min, 0))
    | _ -> None

//******************** DEFINICOES ************************************************//

//Tipo de Entrada do registro
type TipoEntrada =
    | Compra        = 0
    | Transferencia = 1
    | Saldo         = 2
    | Saque         = 3



//Patterns de Bancos
let (|Santander|_|) msg = 
    match msg with
    | ("Santander", _, conteudo) -> 
        match conteudo with
        | ParseRegex @"Transacao\sCartao\s(.*?)\sde\sR(?:\$|..)\s([\d,\.]+)\saprovada\sem\s(.*?)\sas\s(.*?)\s(.*)"
                [ cartao; Valor valor; DateTime data; Hora hora; local ] -> Some ( (cartao, TipoEntrada.Compra, valor, data + hora, local ))
        | ParseRegex @"Transacao\sVisa\sElectron.*?\d+\sde\sR(?:\$|..)\s([\d,\.]+)\saprovada\sem\s(.*?)\sas\s(.*?)\s(.*)"
                [ Valor valor; DateTime data; Hora hora; local ] -> Some ( ("Santander", TipoEntrada.Compra, valor, data + hora, local ))
        | ParseRegex @"Saque\sem\sconta\scorrente\sefetuado\sem\s(.*?)\sas\s(.*?)\sno\svalor\sde\sR\$\s([\d,\.]+)"
                [ DateTime data; Hora hora; Valor valor ] -> Some ("Santander", TipoEntrada.Saque, valor, data + hora, "")
        | _ -> printfn "Erro parse SANTANDER: %A" conteudo
               None
    | _ -> None

let (|Itau|_|) msg = 
    match msg with
    | ("Itau", data, conteudo) -> 
        match conteudo with
        | ParseRegex @"Compra aprovada no (?:seu )?(.*?final \d+) - (.*?)(?:\svalor RS| - RS) ([\d,\.]+) em (.*?),? as (.*?)\."
                [ cartao; local; Valor valor; DateTime data; Hora hora ] -> Some ( (cartao, TipoEntrada.Compra, valor, data + hora, local ))
        | ParseRegex @"SAQUE APROVADO (.*?) (.*?) R\$ (.*?) Local: (.*?)\."
                [ DateTime data; Hora hora; Valor valor; local ] -> Some ( ("ITAU", TipoEntrada.Saque, valor, data + hora, local ))
        | ParseRegex @"saldo de sua conta corrente .*? e de R\$ ([\d,\.]+) em (.*?) as (.*?)\."
                [ Valor valor; DateTime data; Hora hora ] -> Some ( ("ITAU", TipoEntrada.Saldo, valor, data + hora, "" ))
        | ParseRegex @"Cartao final .*?APROVADA (.*?) (.*?) R\$ (.*?) Local: (.*?)\."
                [ DateTime data; Hora hora; Valor valor; local ] -> Some ( ("ITAU", TipoEntrada.Compra, valor, data + hora, local ))
        | _ -> printfn "Erro parse ITAU: %A" conteudo
               None
    | _ -> None


//Carrega o arquivo de mensagens
let carregaMensagens doc = 
    let reg = Regex(@"----\s*\n\s*(?<sender>.*?)\s*\n\s*(?<data>[^:\s]+):?\s*(?<hora>.*?)\s*\n\s*(?<body>.*?)\s*--",  RegexOptions.Multiline ||| RegexOptions.Singleline)
    doc |> reg.Matches
        |> Seq.cast
        |> Seq.map (fun (m:Match) -> [for x in m.Groups -> x.Value] |> List.tail)
        |> Seq.choose (fun s-> match s with
                                |[sender; DateTime data; Hora ts; content] 
                                    -> Some ((sender, data + ts, content))
                                | _ -> printfn "Erro realizando o parse de %A" s
                                       None)
        |> Seq.choose (fun m -> match m with 
                                | Santander a -> Some (a)
                                | Itau a -> Some (a)
                                | _ -> None)

