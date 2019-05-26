using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DeleteOlders
{
    class Program
    {
        static string log = "";

        static void Log(string text)
        {
            log += DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + " - " + text + Environment.NewLine;
            Console.WriteLine(text);
        }
        static string GetArg(string param, string[] args)
        {
            foreach (string arg in args)
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

            int thisMonth = DateTime.Now.Month;
            int max = GetArg("-max", args) == null ? -1 : Convert.ToInt32(GetArg("-max", args));
            int oldMax = GetArg("-oldmax", args) == null ? -1 : Convert.ToInt32(GetArg("-oldmax", args));

            if (max != -1)
            {
                Log("Excluindo diretórios de " + DateTime.Now.Month + "/" + DateTime.Now.Year + " mantendo último(s) " + max + " dia(s)");
                List<DirectoryInfo> monthDirectories = directoryInfo.GetDirectories()
                    .Where(x => (x.CreationTime.Month == DateTime.Now.Month) && (x.CreationTime.Year == DateTime.Now.Year)).ToList();

                int foldersCount = monthDirectories.Count;
                int firstCount = foldersCount - max;

                if (foldersCount > max)
                {
                    List<DirectoryInfo> foldersToDelete = monthDirectories.OrderBy(x => x.CreationTime).Take(firstCount).ToList();

                    foreach (DirectoryInfo dirToDelete in foldersToDelete)
                    {
                        Log("Excluindo diretório:" + dirToDelete.FullName);
                        //Directory.Delete(dirToDelete.FullName);
                    }

                }
            }

            List<DirectoryInfo> oldMonthDirectories = directoryInfo.GetDirectories()
                    .Where(x => (x.CreationTime.Month != DateTime.Now.Month && x.CreationTime.Year != DateTime.Now.Year)).ToList();

            int oldFoldersCount = oldMonthDirectories.Count;
            if (ArgExists("-oldmax", args))
            {                
                int oldFirstCount = oldFoldersCount - oldMax;

                if (oldFoldersCount > oldMax)
                {
                    Log("Excluindo meses anteriores mantendo último(s) " + oldMax + " dia(s) de cada mês");
                    List<DirectoryInfo> oldFoldersToDelete = oldMonthDirectories.OrderBy(x => x.CreationTime).Take(oldFirstCount).ToList();

                    foreach (DirectoryInfo oldDirToDelete in oldFoldersToDelete)
                    {
                        Log("Excluindo diretório antigo:" + oldDirToDelete.Name);
                        //Directory.Delete(dirToDelete.FullName);
                    }

                }
            }
            else
            {
                if (oldFoldersCount > 0)
                {
                    Log("Excluindo todos os meses mais antigos");
                    foreach (DirectoryInfo allOldDirectoriesToDelete in oldMonthDirectories)
                    {
                        Log("Excluindo todos diretórios antigos:" + allOldDirectoriesToDelete.Name);
                    }
                }
            }

        }

    }
}
