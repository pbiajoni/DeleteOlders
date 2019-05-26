using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DeleteOlders
{
    class Program
    {

        static string GetArg(string param, string[] args)
        {
            foreach(string arg in args)
            {
                if (arg.Contains(param))
                {
                    return arg.Replace(param, "");
                }
            }

            return null;
        }

        static bool ArgExists(string param, string[] args)
        {
            foreach (string arg in args)
            {
                if (arg.Contains(param))
                {
                    return true;
                }
            }

            return false;
        }

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                throw new Exception("Nenhum argumento encontrado");
            }

            if (GetArg("-d", args) == null)
            {
                throw new Exception("Requer o argumento -d com o diretório a ser verificado");
            }

            string rootDirectory = GetArg("-d", args);
            DirectoryInfo directoryInfo = new DirectoryInfo(rootDirectory);

        }

    }
}
