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
        static List<MonthYear> monthYears = new List<MonthYear>();

        static string ToSize(long source)
        {
            const int byteConversion = 1024;
            double bytes = Convert.ToDouble(source);

            if (bytes >= Math.Pow(byteConversion, 3)) //GB Range
            {
                return string.Concat(Math.Round(bytes / Math.Pow(byteConversion, 3), 2), " GB");
            }
            else if (bytes >= Math.Pow(byteConversion, 2)) //MB Range
            {
                return string.Concat(Math.Round(bytes / Math.Pow(byteConversion, 2), 2), " MB");
            }
            else if (bytes >= byteConversion) //KB Range
            {
                return string.Concat(Math.Round(bytes / byteConversion, 2), " KB");
            }
            else //Bytes
            {
                return string.Concat(bytes, " Bytes");
            }
        }
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


        static void AddMonthYear(MonthYear monthYear)
        {
            if (!monthYears.Any(x => x.Month == monthYear.Month && x.Year == monthYear.Year))
            {
                Log("Mês para verificar:" + monthYear.ToString());
                monthYears.Add(monthYear);
            }
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
            Mailer mailer = new Mailer();
            long totalSize = 0;

            try
            {
                if (ArgExists("-testmail", args))
                {
                    try
                    {
                        Log("Teste de envio de email");
                        mailer.Send("Teste de email", "DeleteOlders, teste de email");
                        Log("Email enviado");
                    }
                    catch (Exception er)
                    {
                        Log("Email não enviado:" + er.Message);
                    }

                    return;
                }

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
                            totalSize = totalSize + dirToDelete.EnumerateFiles().Sum(file => file.Length);

                            Directory.Delete(dirToDelete.FullName, true);
                        }

                    }
                }

                List<DirectoryInfo> oldMonthDirectories = directoryInfo.GetDirectories()
                        .Where(x => (x.CreationTime < new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1))).ToList();

                int oldFoldersCount = oldMonthDirectories.Count;

                if (ArgExists("-oldmax", args))
                {
                    foreach (DirectoryInfo oldDi in oldMonthDirectories)
                    {
                        MonthYear monthYear = new MonthYear(oldDi.CreationTime.Month, oldDi.CreationTime.Year);
                        AddMonthYear(monthYear);
                    }

                    foreach (MonthYear monthYear in monthYears)
                    {
                        List<DirectoryInfo> oldDiMonthYear = directoryInfo.GetDirectories()
                        .Where(x => (x.CreationTime.Month == monthYear.Month) && (x.CreationTime.Year == monthYear.Year)).ToList();

                        int oldMonthYearCount = oldDiMonthYear.Count;
                        int oldMonthYearFirstCount = oldMonthYearCount - oldMax;

                        if (oldMonthYearCount > oldMax)
                        {
                            List<DirectoryInfo> oldMonthYearToDelete = oldDiMonthYear.OrderBy(x => x.CreationTime).Take(oldMonthYearFirstCount).ToList();

                            foreach (DirectoryInfo oldMonthYearDelete in oldMonthYearToDelete)
                            {
                                Log("Excluindo diretório anterior:" + oldMonthYearDelete.FullName);
                                totalSize = totalSize + oldMonthYearDelete.EnumerateFiles().Sum(file => file.Length);
                                Directory.Delete(oldMonthYearDelete.FullName, true);
                            }
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
                            Log("Excluindo todos diretórios antigos:" + allOldDirectoriesToDelete.FullName);
                            totalSize = totalSize + allOldDirectoriesToDelete.EnumerateFiles().Sum(file => file.Length);
                            Directory.Delete(allOldDirectoriesToDelete.FullName, true);
                        }
                    }
                }

                Log("Fim da remoção");
                Log("Tamanho total removido foi de " + ToSize(totalSize));

                try
                {
                    mailer.Send("Remoção de backups", log);
                }
                catch (Exception)
                {

                    throw;
                }
                

            }
            catch (Exception er)
            {
                Log("Ocorreu um erro:" + er.Message);

                try
                {
                    mailer.Send("ERRO:Remoção de backups", log);
                }
                catch (Exception)
                {

                    throw;
                }
            }

        }

    }
}
