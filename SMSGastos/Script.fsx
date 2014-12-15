// This file is a script that can be executed with the F# Interactive.  
// It can be used to explore and test the library project.
// Note that script files will not be part of the project build.

(*
#I @"C:\Users\fabio.marreco\Dropbox\Projetos\SMSGastos\SMSGastos"
#load @"C:\utilities\startupsimple.fsx"
open Visualize
*)

#load "SMSParser.fs"
open SMSParser
open System
open System.IO


let arquivoBanco = (string (fsi.CommandLineArgs.GetValue(1)))
let dir_saida = (string (fsi.CommandLineArgs.GetValue(2)))



(*
let arquivoBanco = @"C:\Users\fabio.marreco\Dropbox\sync phone\Banco.txt";;
let dir_saida = @"C:\Users\fabio.marreco\Desktop\YNAB";;
*)
//AREA DE TESTES
let sms = File.ReadAllText (arquivoBanco, System.Text.Encoding.Default)
let msgs = carregaMensagens sms 

//msgs|> Visualize.gridview

//let dir_saida = @"C:\Users\fabio.marreco\Desktop\YNAB";

let (|Conta|_|) conta = 
    match conta with
    | ParseRegex @".*?final (\d+)" [ "8051" ] -> Some("ITAUVS.csv")
    | ParseRegex @".*?final (\d+)" [ "5109" ] -> Some("ITAUVS.csv")
    | ParseRegex @".*?final (\d+)" [ "9411" ] -> Some("ITAUMC.csv")
    | ParseRegex @".*?final (\d+)" [ "4860" ] -> Some("SANTANDERVS.csv")
    | ParseRegex @".*?final (\d+)" [ "3633" ] -> Some("SANTANDERVS.csv")
    | ParseRegex @".*?final (\d+)" [ "3470" ] -> Some("SANTANDERVS.csv")
    | "ITAU"-> Some("ITAU.csv")
    | "Santander" -> Some("SANTANDER.csv")
    | _ -> None


let atualiza_arquivo arquivo date payee (value:float) = 
    if not (System.IO.File.Exists (arquivo)) then do System.IO.File.WriteAllLines (arquivo,["Date,Payee,Category,Memo,Outflow,Inflow"])
    let line = System.String.Format (@"{0:dd/MM/yy},{1},,,{2},", date, payee, System.Xml.XmlConvert.ToString(value))
    System.IO.File.AppendAllLines (arquivo,[line])


msgs |> Seq.iter ( 
    fun msg ->
        match msg with 
        | (Conta arq, TipoEntrada.Compra, value, date, payee) -> 
              let path = System.IO.Path.Combine(dir_saida, arq)
              do atualiza_arquivo path date payee value
        | _ -> printfn "ERR: %A" msg)
        
let caminho = Path.GetDirectoryName(arquivoBanco);
let nomeArq = Path.GetFileNameWithoutExtension(arquivoBanco);
let novoNome = Path.Combine (caminho, nomeArq + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm") + ".txt")
do System.IO.File.Move (arquivoBanco, novoNome)
