using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace Merrequinha
{
    public class ConversorSMS
    {
        public IEnumerable<Registro> Executa(string nomeArquivo)
        {
            var str = File.ReadAllText(nomeArquivo, Encoding.Default);
            var reg = new Regex(
                "----\\s*\\n\r\n(?<sender>.*?)\\s*\\n\r\n(?<data>[^:]+):\\s*  (?<hora>.*?)\\s*\\n\r\n(?<body>.*?)\\s*\r\n--",
                RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline
                | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

            var resultado =
                from m in reg.Matches(str).Cast<Match>()
                select new
                {
                    Sender = m.Groups["sender"].Value,
                    Data = m.Groups["data"].Value,
                    Hora = m.Groups["hora"].Value,
                    Conteudo = m.Groups["body"].Value,
                };

            return null;// resultado.Select(ConverteMatchRegistro); 

        }

        private Registro ConverteMatchRegistro(Match m)
        {
            throw new NotImplementedException();
        }
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
