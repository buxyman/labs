using System.ServiceProcess;
using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

namespace lab3
{
    static class Program
    {
        
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new Service1()
            };

            string DirectoryPath = @"D:\labs\lab3";
            try
            {
                ServiceBase.Run(ServicesToRun);
            }
            catch (Exception e)
            {
                using (StreamWriter sw = new StreamWriter(Path.Combine(DirectoryPath, "Exceptions.txt"), true))
                {
                    sw.WriteLine($"Exception: ( {e.Message} ) ---------- {DateTime.Now: dd/MM/yyyy HH:mm:ss}");
                }
            }

            ServiceBase.Run(ServicesToRun);
        }





        internal static string targetDirectoryPath = @"D:\labs\lab3\out";
        internal static string sourceDirectoryPath = @"D:\labs\in";

        static void DecryptFile(string inputFile, string outputFile, string skey)
        {
            try
            {
                using (RijndaelManaged aes = new RijndaelManaged())
                {
                    byte[] key = ASCIIEncoding.UTF8.GetBytes(skey);

                    byte[] IV = ASCIIEncoding.UTF8.GetBytes(skey);

                    using (FileStream fsCrypt = new FileStream(inputFile, FileMode.Open))
                    {
                        using (FileStream fsOut = new FileStream(outputFile, FileMode.Create))
                        {
                            using (ICryptoTransform decryptor = aes.CreateDecryptor(key, IV))
                            {
                                using (CryptoStream cs = new CryptoStream(fsCrypt, decryptor, CryptoStreamMode.Read))
                                {
                                    int data;
                                    while ((data = cs.ReadByte()) != -1)
                                    {
                                        fsOut.WriteByte((byte)data);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {


            }
        }
        static void EncryptFile(string inputFile, string outputFile, string skey)
        {
            RijndaelManaged aes = new RijndaelManaged();
            try
            {
                byte[] key = ASCIIEncoding.UTF8.GetBytes(skey);
                using (FileStream fsCrypt = new FileStream(outputFile, FileMode.Create))
                {
                    using (CryptoStream cs = new CryptoStream(fsCrypt, aes.CreateEncryptor(key, key), CryptoStreamMode.Write))
                    {
                        using (FileStream fsIn = new FileStream(inputFile, FileMode.Open))
                        {
                            int data;
                            while ((data = fsIn.ReadByte()) != -1)
                            {
                                cs.WriteByte((byte)data);
                            }

                            aes.Clear();

                        }
                    }
                }

            }
            catch (Exception e)
            {
                aes.Clear();
            }
        }

        static void FileToArchive(string directoryPath, string archivePath)
        {
            ZipFile.CreateFromDirectory(directoryPath, archivePath);
        }

        static void ArchieveToFile(string archivePath, string exDirectoryPath)
        {
            ZipFile.ExtractToDirectory(archivePath, exDirectoryPath);
        }

        static void watch(string path, bool includeSubDirs)
        {
            using (var watcher = new FileSystemWatcher(path))
            {
                watcher.Created += FileCreatedChangedDeleted;
                watcher.Changed += FileCreatedChangedDeleted;
                watcher.Deleted += FileCreatedChangedDeleted;
                watcher.Renamed += FileRenamed;
                watcher.Error += FileError;

                watcher.IncludeSubdirectories = includeSubDirs;
                watcher.EnableRaisingEvents = true;
                Console.WriteLine("Просмотр событий. Завершение по нажатию Enter");
                Console.ReadLine();
            }
        }
        static void FileCreatedChangedDeleted(object o, FileSystemEventArgs e)
        {
            Console.WriteLine("Файл {0} был {1}", e.FullPath, e.ChangeType);
            string chType = "";
            chType += e.ChangeType;

            if (chType == "Created")
            {
                string fName = e.FullPath.Substring(19);
                string aName = e.FullPath.Substring(19).Substring(0, fName.Length - 3) + "zip";
                string dName = e.FullPath.Substring(19).Substring(0, fName.Length - 3);

                EncryptFile(e.FullPath, @"D:\\nw.txt", "5547645789675435");
                File.Copy(@"D:\\nw.txt", e.FullPath, true);
                Console.WriteLine("Файл был зашифрован");

                FileToArchive(@"D:\\SourceDirectory", @"D:\\TargetDirectory\" + aName);
                Console.WriteLine("Файл был архивирован");
                File.Delete(e.FullPath);

                ArchieveToFile(@"D:\\TargetDirectory\" + aName, @"D:\\TargetDirectory");
                Console.WriteLine("Файл был извлечен");
                File.Delete(@"D:\\TargetDirectory\" + aName);

                DecryptFile(@"D:\\TargetDirectory\" + fName, @"D:\\TargetDirectory\Nun.txt", "5547645789675435");
                Console.WriteLine("Файл был дешифрован");
                File.Copy(@"D:\\TargetDirectory\Nun.txt", @"D:\\TargetDirectory\" + fName, true);
                File.Delete(@"D:\\TargetDirectory\Nun.txt");

                Directory.CreateDirectory(@"D:\\TargetDirectory\archive\" + dName);
                File.Move(@"D:\\TargetDirectory\" + fName, @"D:\\TargetDirectory\archive\" + dName + "\\" + fName);
            }
        }

        static void FileRenamed(object o, RenamedEventArgs e)
         => Console.WriteLine("Переименован: {0} в {1}", e.OldFullPath, e.FullPath);
        static void FileError(object o, ErrorEventArgs e)
         => Console.WriteLine("Ошибка: ", e.GetException().Message);
    }
}
