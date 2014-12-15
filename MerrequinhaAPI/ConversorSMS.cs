using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;

namespace MerrequinhaAPI
{
    public class ConversorSMS
    { 
        public IEnumerable<Registro> Executa(string nomeArquivo)
        {
            var str = File.ReadAllText(nomeArquivo, Encoding.Default);
            ProcessaDocumento(str);

            return null;// resultado.Select(ConverteMatchRegistro); 
        }

        private IEnumerable<EntradaSMS> ProcessaDocumento(string str)
        {
            //double.TryParse ("12,0", NumberStyles.Currency, CultureInfo.GetCultureInfo ("pt-br"))

            var reg = new Regex(
                "----\\s*\\n\r\n(?<sender>.*?)\\s*\\n\r\n(?<data>[^:]+):\\s*  (?<hora>.*?)\\s*\\n\r\n(?<body>.*?)\\s*\r\n--",
                RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline
                | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

            //Regex rvalor = new Regex(@"(?<=RÂ¤|R[S$]\s)[\d,\.]+");
            var ci = CultureInfo.CreateSpecificCulture("pt-br");
            return 
                from m in reg.Matches(str).Cast<Match>()
                select new EntradaSMS
                {
                    Sender = m.Groups["sender"].Value.Trim(),
                    Data = TrataHora(m.Groups["data"].Value, m.Groups["hora"].Value),
                    Conteudo = m.Groups["body"].Value
                };
        }

        private DateTime TrataHora(string dia, string hora)
        {
            var resultado = DateTime.Parse(dia);
            var m = Regex.Match(hora, @"(?<hora>\d+)\.(?<min>\d+)");
            resultado.AddHours(int.Parse(m.Groups["hora"].Value));
            resultado.AddMinutes(int.Parse(m.Groups["min"].Value));
            return resultado;


        }

        private object ProcessaConteudoSMS(string p)
        {
            throw new NotImplementedException();
        }

        private Registro ConverteMatchRegistro(Match m)
        {
            throw new NotImplementedException();
        }
    }

    public class EntradaSMS
    {
        public string Sender { get; set; }
        public DateTime Data { get; set; }
        public string Conteudo { get; set; }
    }

    public class Registro
    {
        public string Conta { get; set; }
        public DateTime Data { get; set; }
        public double Valor { get; set; }
        public TipoRegistro Tipo { get; set; }
    }

    public enum TipoRegistro
    {
        Saque, 
        Gasto, 
        Transferencia, 
        Ajuste
    }
}
