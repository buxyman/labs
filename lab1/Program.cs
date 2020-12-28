using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;

namespace Lab1
{
    [Serializable]
    class Person
    {
        public static int currentId = 0;

        public string Name { get; private set; }
        public int Age { get; private set; }
        public int Id { get; private set; }

        public Person(string Name, int Age, int Id)
        {
            this.Name = Name;
            this.Age = Age;
            this.Id = Id;
        }
    }

    class Command
    {
        public string FullPath { get; private set; }

        string prevValidPath;

        List<Person> list;

        public static List<string> log;

        public string CommandText { get; private set; }

        public Command(string path, List<Person> list)
        {
            FullPath = path;
            prevValidPath = path;
            this.list = list;
        }

        void Parse()
        {
            if (CommandText == "exit") return;

            if (CommandText == "help")
            {

                Console.WriteLine("1. cd - move to directory");
                Console.WriteLine("2. . - root directory");
                Console.WriteLine("3. .. - parent directory");
                Console.WriteLine("4. createdir");
                Console.WriteLine("5. deldir");
                Console.WriteLine("6. renamedir");
                Console.WriteLine("7. createfile");
                Console.WriteLine("8. delfile");
                Console.WriteLine("9. renamefile");
                Console.WriteLine("10. movefile");
                Console.WriteLine("11. copyfile");

                Console.WriteLine("12. addcol");
                Console.WriteLine("13. delcol");
                Console.WriteLine("14. clrcol");
                Console.WriteLine("15. showcol");
                Console.WriteLine("16. loadcoltxt");
                Console.WriteLine("17. savecoltxt");
                Console.WriteLine("18. loadcolbin");
                Console.WriteLine("19. savecolbin");


                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();

                return;
            }

            if (CommandText == "..")
            {
                string newPath = Directory.GetParent(FullPath)?.FullName;

                if (newPath == null)
                    throw new InvalidOperationException("No parent root");

                FullPath = newPath;

                return;
            }

            if (CommandText == ".")
            {
                string newPath = Directory.GetDirectoryRoot(FullPath);

                if (newPath == null)
                    throw new InvalidOperationException("No root directory");

                FullPath = newPath;

                return;
            }

            if (CommandText == "cd")
            {
                Console.Write("Enter the name of directory to move: ");
                string dirName = Console.ReadLine();

                dirName = dirName.Trim('/');
                dirName = dirName.Trim('\\');

                Console.WriteLine(dirName.Contains(':'));

                string newPath = dirName.Contains(':') ? dirName : Path.Combine(FullPath, dirName);

                if (!Directory.Exists(newPath))
                    throw new DirectoryNotFoundException("Direcory does not exist");

                string[] dirs = Directory.GetDirectories(newPath);
                string[] files = Directory.GetFiles(newPath);

                foreach (string s in dirs)
                    if (!Directory.Exists(s))
                        throw new DirectoryNotFoundException("Invalid path");

                foreach (string s in files)
                    if (!File.Exists(s))
                        throw new DirectoryNotFoundException("Invalid path");

                FullPath = newPath;

                return;
            }

            if (CommandText == "createdir")
            {
                Console.Write("Enter the name of directory to create: ");
                string dirName = Console.ReadLine();

                string newPath = dirName.Contains(':') ? dirName : Path.Combine(FullPath, dirName);

                if (Directory.Exists(newPath))
                    throw new InvalidOperationException("Direcory already exists");

                Directory.CreateDirectory(newPath);

                return;
            }

            if (CommandText == "deldir")
            {
                Console.Write("Enter the name of directory to delete: ");
                string dirName = Console.ReadLine();

                string newPath = dirName.Contains(':') ? dirName : Path.Combine(FullPath, dirName);

                if (!Directory.Exists(newPath))
                    throw new InvalidOperationException("Direcory does not exist");

                Directory.Delete(newPath, true);

                return;
            }

            if (CommandText == "renamedir")
            {
                Console.Write("Enter the name of directory to rename: ");
                string dirName = Console.ReadLine();

                string newPath1 = dirName.Contains(':') ? dirName : Path.Combine(FullPath, dirName);

                if (!Directory.Exists(newPath1))
                    throw new InvalidOperationException("Direcory does not exist");

                Console.Write("Enter new name: ");
                string dirNewName = Console.ReadLine();

                string newPath2 = (dirNewName.Contains(':')) ? dirNewName : Path.Combine(FullPath, dirNewName);

                Directory.Move(newPath1, newPath2);

                return;
            }

            if (CommandText == "createfile")
            {
                Console.Write("Enter the name of file to create: ");
                string fileName = Console.ReadLine();

                string newPath = fileName.Contains(':') ? fileName : Path.Combine(FullPath, fileName);

                if (File.Exists(newPath))
                    throw new InvalidOperationException("File already exists");

                using (File.Create(newPath)) { }

                return;
            }

            if (CommandText == "delfile")
            {
                Console.Write("Enter the name of file to delete: ");
                string fileName = Console.ReadLine();

                string newPath = fileName.Contains(':') ? fileName : Path.Combine(FullPath, fileName);

                if (!File.Exists(newPath))
                    throw new InvalidOperationException("File does not exist");

                File.Delete(newPath);

                return;
            }

            if (CommandText == "renamefile")
            {
                Console.Write("Enter the name of file to rename: ");
                string fileName = Console.ReadLine();

                string newPath1 = fileName.Contains(':') ? fileName : Path.Combine(FullPath, fileName);

                if (!File.Exists(newPath1))
                    throw new InvalidOperationException("File does not exist");

                Console.Write("Enter new name: ");
                string fileNewName = Console.ReadLine();

                string newPath2 = fileNewName.Contains(':') ? fileNewName : Path.Combine(FullPath, fileNewName);

                File.Move(newPath1, newPath2);

                return;
            }

            if (CommandText == "movefile")
            {
                Console.Write("Enter the name of file to move: ");
                string fileName = Console.ReadLine();

                string newPath1 = fileName.Contains(':') ? fileName : Path.Combine(FullPath, fileName);

                if (!File.Exists(newPath1))
                    throw new InvalidOperationException("File does not exist");

                Console.Write("Enter new location: ");
                string dirName = Console.ReadLine();

                string newPath2 = dirName.Contains(':') ? Path.Combine(dirName, fileName) : Path.Combine(FullPath, dirName, fileName);

                File.Move(newPath1, newPath2);

                return;
            }

            if (CommandText == "copyfile")
            {
                Console.Write("Enter the name of file to copy: ");
                string fileName = Console.ReadLine();

                string newPath1 = fileName.Contains(':') ? fileName : Path.Combine(FullPath, fileName);

                if (!File.Exists(newPath1))
                    throw new InvalidOperationException("File does not exist");

                Console.Write("Enter new location: ");
                string dirName = Console.ReadLine();

                string newPath2 = dirName.Contains(':') ? Path.Combine(dirName, fileName) : Path.Combine(FullPath, dirName, fileName);

                File.Copy(newPath1, newPath2);

                return;
            }

            if (CommandText == "addcol")
            {
                Console.Write("Enter the name: ");
                string name = Console.ReadLine();

                Console.Write("Enter the age: ");
                int age = int.Parse(Console.ReadLine());

                if (age < 0 || age > 150)
                    throw new FormatException("Invalid age value");

                list.Add(new Person(name, age, ++Person.currentId));

                log.Add($"The element was successfully added to the collection");

                return;
            }

            if (CommandText == "delcol")
            {
                Console.Write("Enter ID of person to delete: ");

                int id = int.Parse(Console.ReadLine());

                if (id < 1)
                    throw new FormatException("Invalid ID value");

                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Id == id)
                    {
                        list.RemoveAt(i);

                        log.Add($"The element was successfully removed from the collection");

                        return;
                    }
                }

                throw new Exception("ID is not found");
            }

            if (CommandText == "clrcol")
            {
                list.Clear();

                log.Add($"The collection was successfully cleared");

                return;
            }

            if (CommandText == "showcol")
            {
                Console.ForegroundColor = ConsoleColor.Green;

                if (list.Count == 0)
                    Console.WriteLine("None");
                else
                    foreach (var person in list)
                        Console.WriteLine($"ID: {person.Id,-5} Name: {person.Name,-20} Age: {person.Age}");

                Console.ResetColor();

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
                return;
            }

            if (CommandText == "loadcoltxt")
            {
                List<Person> newList = new List<Person>();
                int maxId = 0;

                Console.Write("Enter the name of file to load: ");
                string fileName = Console.ReadLine();

                Regex regTxt = new Regex(@"\.txt$");
                Regex regGz = new Regex(@"\.gz$");

                if (!regTxt.IsMatch(fileName) && !regGz.IsMatch(fileName))
                    throw new Exception("Invalid file extension");

                string newPath = fileName.Contains(':') ? fileName : Path.Combine(FullPath, fileName);

                if (!File.Exists(newPath))
                    throw new InvalidOperationException("File does not exist");

                string decompressedPath = null;

                if (regGz.IsMatch(newPath))
                {
                    Regex reg1 = new Regex(@"\\?.+$");

                    StringBuilder sb = new StringBuilder(newPath);

                    if (reg1.Match(newPath).ToString().Contains('.'))
                    {
                        while (sb[sb.Length - 1] != '.') sb.Remove(sb.Length - 1, 1);
                        sb.Remove(sb.Length - 1, 1);
                    }

                    sb.Append(".txt");

                    decompressedPath = sb.ToString();

                    Decompress(newPath, decompressedPath);
                }

                if (decompressedPath != null)
                    newPath = decompressedPath;

                using (StreamReader sr = new StreamReader(newPath))
                {
                    while (!sr.EndOfStream)
                    {
                        string name = sr.ReadLine();
                        int age = int.Parse(sr.ReadLine());
                        int id = int.Parse(sr.ReadLine());

                        if (age < 0 || age > 150)
                            throw new FormatException("Invalid age value");

                        if (id < 0)
                            throw new FormatException("Invalid ID value");

                        if (id > maxId) maxId = id;

                        newList.Add(new Person(name, age, id));
                    }
                }

                list = newList;
                Person.currentId = maxId;

                log.Add($"The collection was successfully loaded");

                return;
            }

            if (CommandText == "savecoltxt")
            {
                Console.Write("Enter the name of file to save: ");
                string fileName = Console.ReadLine();

                Regex reg = new Regex(@"\.txt$");

                if (!reg.IsMatch(fileName))
                    throw new Exception("Invalid file extension");

                string newPath = fileName.Contains(':') ? fileName : Path.Combine(FullPath, fileName);

                using (StreamWriter sw = new StreamWriter(newPath))
                {
                    foreach (Person person in list)
                    {
                        sw.WriteLine(person.Name);
                        sw.WriteLine(person.Age);
                        sw.WriteLine(person.Id);
                    }
                }

                Console.Write("Compress file? (yes/no) : ");

                string s = Console.ReadLine();

                if (s == "yes")
                {
                    Regex reg1 = new Regex(@"\\?.+$");

                    StringBuilder sb = new StringBuilder(newPath);

                    if (reg1.Match(newPath).ToString().Contains('.'))
                    {
                        while (sb[sb.Length - 1] != '.') sb.Remove(sb.Length - 1, 1);
                        sb.Remove(sb.Length - 1, 1);
                    }

                    sb.Append(".gz");

                    string compressedFilePath = sb.ToString();

                    Compress(newPath, compressedFilePath);

                    File.Delete(newPath);

                    log.Add($"The collection was successfully saved");

                    return;
                }
                else if (s == "no")
                {
                    log.Add($"The collection was successfully saved");

                    return;
                }
                else
                    throw new Exception("Invalid command");
            }

            if (CommandText == "loadcolbin")
            {
                List<Person> newList = new List<Person>();

                Console.Write("Enter the name of file to load: ");
                string fileName = Console.ReadLine();

                Regex regDat = new Regex(@"\.dat$");
                Regex regGz = new Regex(@"\.gz$");

                if (!regDat.IsMatch(fileName) && !regGz.IsMatch(fileName))
                    throw new Exception("Invalid file extension");

                string newPath = fileName.Contains(':') ? fileName : Path.Combine(FullPath, fileName);

                if (!File.Exists(newPath))
                    throw new InvalidOperationException("File does not exist");

                string decompressedPath = null;

                if (regGz.IsMatch(newPath))
                {
                    Regex reg1 = new Regex(@"\\?.+$");

                    StringBuilder sb = new StringBuilder(newPath);

                    if (reg1.Match(newPath).ToString().Contains('.'))
                    {
                        while (sb[sb.Length - 1] != '.') sb.Remove(sb.Length - 1, 1);
                        sb.Remove(sb.Length - 1, 1);
                    }

                    sb.Append(".dat");

                    decompressedPath = sb.ToString();

                    Decompress(newPath, decompressedPath);
                }

                if (decompressedPath != null)
                    newPath = decompressedPath;

                using (FileStream fs = new FileStream(newPath, FileMode.Open))
                {
                    newList = (List<Person>)(new BinaryFormatter()).Deserialize(fs);
                }

                list = newList;

                int maxId = 0;

                for (int i = 0; i < list.Count; i++)
                    if (list[i].Id > maxId)
                        maxId = list[i].Id;

                Person.currentId = maxId;

                log.Add($"The collection was successfully loaded");

                return;
            }

            if (CommandText == "savecolbin")
            {
                Console.Write("Enter the name of file to save: ");
                string fileName = Console.ReadLine();

                Regex reg = new Regex(@"\.dat$");

                if (!reg.IsMatch(fileName))
                    throw new Exception("Invalid file extension");

                string newPath = fileName.Contains(':') ? fileName : Path.Combine(FullPath, fileName);

                using (FileStream fs = new FileStream(newPath, FileMode.OpenOrCreate))
                {
                    (new BinaryFormatter()).Serialize(fs, list);
                }

                Console.Write("Compress file? (yes/no) : ");

                string s = Console.ReadLine();

                if (s == "yes")
                {
                    Regex reg1 = new Regex(@"\\?.+$");

                    StringBuilder sb = new StringBuilder(newPath);

                    if (reg1.Match(newPath).ToString().Contains('.'))
                    {
                        while (sb[sb.Length - 1] != '.') sb.Remove(sb.Length - 1, 1);
                        sb.Remove(sb.Length - 1, 1);
                    }

                    sb.Append(".gz");

                    string compressedFilePath = sb.ToString();

                    Compress(newPath, compressedFilePath);

                    File.Delete(newPath);

                    log.Add($"The collection was successfully saved");

                    return;
                }
                else if (s == "no")
                {
                    log.Add($"The collection was successfully saved");

                    return;
                }
                else
                    throw new Exception("Invalid command");
            }

            throw new InvalidOperationException("Invalid command");
        }

        public void GetCommand()
        {
            prevValidPath = FullPath;

            CommandText = Console.ReadLine();
            Parse();
        }

        public void SetPrevValidPath() => FullPath = prevValidPath;

        void Compress(string sourceFile, string compressedFile)
        {
            using (FileStream sourceStream = new FileStream(sourceFile, FileMode.Open))
            {
                using (FileStream targetStream = new FileStream(compressedFile, FileMode.OpenOrCreate))
                {
                    using (GZipStream compressionStream = new GZipStream(targetStream, CompressionMode.Compress))
                    {
                        sourceStream.CopyTo(compressionStream);
                    }
                }
            }
        }

        void Decompress(string compressedFile, string targetFile)
        {
            using (FileStream sourceStream = new FileStream(compressedFile, FileMode.Open))
            {
                using (FileStream targetStream = new FileStream(targetFile, FileMode.OpenOrCreate))
                {
                    using (GZipStream decompressionStream = new GZipStream(sourceStream, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(targetStream);
                    }
                }
            }
        }
    }

    class Program
    {
        static void OnChanged(object sender, FileSystemEventArgs e)
        {
            Command.log.Add($"{e.FullPath} {e.ChangeType}");
        }

        static void OnRenamed(object sender, RenamedEventArgs e)
        {
            Command.log.Add($"{e.OldFullPath} renamed to {e.FullPath}");
        }

        static void Main(string[] args)
        {
            //List<Person> list = new List<Person>();
            List<Person> list = new List<Person>() { new Person("Tom", 54, ++Person.currentId),
                new Person("John", 32, ++Person.currentId),
                new Person("Jack", 66, ++Person.currentId),
                new Person("Mark", 10, ++Person.currentId),
                new Person("Harry", 93, ++Person.currentId),
                new Person("Carl", 41, ++Person.currentId),
                new Person("Bill", 18, ++Person.currentId),};

            FileSystemWatcher watcher = new FileSystemWatcher();

            watcher.Changed += OnChanged;
            watcher.Created += OnChanged;
            watcher.Deleted += OnChanged;
            watcher.Renamed += OnRenamed;

            Command command = new Command(Directory.GetCurrentDirectory(), list);
            Command.log = new List<string>();

            watcher.Path = command.FullPath;
            watcher.EnableRaisingEvents = true;

            while (command.CommandText != "exit")
            {
                try
                {
                    if (Command.log.Count > 0)
                    {
                        Console.WriteLine("Log:");

                        for (int i = 0; i < Command.log.Count; i++)
                            Console.WriteLine(Command.log[i]);

                        Console.WriteLine();
                        Command.log.Clear();
                    }

                    Console.WriteLine("Current path:");
                    Console.WriteLine(command.FullPath);

                    string[] dirs = Directory.GetDirectories(command.FullPath);
                    string[] files = Directory.GetFiles(command.FullPath);

                    Console.WriteLine("\nDirectories:");

                    if (dirs.Length == 0) Console.WriteLine("None");

                    foreach (string s in dirs)
                    {
                        DirectoryInfo directoryInfo = new DirectoryInfo(s);

                        Console.WriteLine(directoryInfo.Name);
                    }

                    Console.WriteLine("\nFiles:");


                    if (files.Length == 0) Console.WriteLine("None");

                    foreach (string s in files)
                    {
                        FileInfo fileInfo = new FileInfo(s);

                        Console.WriteLine($"{fileInfo.Name,-40} Size: {fileInfo.Length.ToString() + " bytes",-15} Changed: {fileInfo.LastWriteTime}");
                    }

                    Console.Write("\n>");

                    watcher.Path = command.FullPath; // sets the path to watch

                    command.GetCommand();
                }
                catch (DirectoryNotFoundException ex)
                {
                    Console.Clear();
                    Console.WriteLine(ex.Message + "\n");

                    command.SetPrevValidPath();
                    continue;
                }
                catch (Exception ex)
                {
                    Console.Clear();
                    Console.WriteLine(ex.Message + "\n");
                    continue;
                }
                Console.Clear();
            }
        }
    }
}