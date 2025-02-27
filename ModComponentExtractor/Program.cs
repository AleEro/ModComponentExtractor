﻿using ICSharpCode.SharpZipLib.Zip;

namespace ModComponentExtractor
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            while (true)
            {
                string console_input = string.Empty;

                if ((args.Length != 1) == (console_input == string.Empty))
                {
                    Console.WriteLine("\tProgram takes exactly one argument");
                    Console.WriteLine("\tOn Windows, either:");
                    Console.WriteLine("\tDrop a modcomponent file to extract it into a folder");
                    Console.WriteLine("\tDrop a folder to compress it into a modcomponent file");
                    Console.WriteLine("\tOn Mac / Linux, either:");
                    Console.WriteLine("\tPass a modcomponent file path as an argument in the terminal to extract it into a folder");
                    Console.WriteLine("\tPass a folder path as an argument in the terminal to compress it into a modcomponent file");
                    Console.WriteLine("\tPress enter to quit this application and try again");
                    Console.WriteLine("\tInput q button and press enter to quit this application");

                    console_input = Console.ReadLine().ToString();
                }
                if (console_input.ToLower() == "q")
                {
                    Console.WriteLine("\tEXITIG...");
                    break;
                }
                if (console_input != "")
                {
                    ProcessArgs(console_input);
                    console_input = string.Empty;
                }
                if (args.Length == 1)
                {
                    ProcessArgs(args[0]);
                    args = null;
                }
            }

        }
        private static void ProcessArgs(string arg)
        {
            try
            {
                ProcessPath(arg);
                Console.WriteLine("Succeeded");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine("Failed");
            }
        }
        private static void ProcessPath(string path)
        {
            if (Directory.Exists(path))
            {
                ProcessDirectory(path);
            }
            else if (File.Exists(path))
            {
                ProcessFile(path);
            }
            else
            {
                throw new FileNotFoundException("Could not find file or folder", path);
            }
        }

        private static void ProcessDirectory(string path)
        {
            string name = Path.GetFileName(path);
            if (name.EndsWith('\\') || name.EndsWith('/'))
                name = name.Substring(0, name.Length - 1);
            if (name == "blueprints" || name == "auto-mapped" || name == "gear-spawns")
                throw new Exception($"{name} cannot be used as the name of an item pack. Place this folder into a new empty folder, and use that folder instead");
            string parentDirectory = Path.GetDirectoryName(Path.GetFullPath(path)) ?? throw new Exception("Could not get parent directory path");
            string outputPath = Path.Combine(parentDirectory, $"{name}.modcomponent");
            FastZip fastZip = new FastZip();
            fastZip.CreateZip(outputPath, path, true, "");
        }

        private static void ProcessFile(string path)
        {
            string extension = Path.GetExtension(path);
            if (extension != ".modcomponent" && extension != ".zip")
            {
                throw new Exception($"Program cannot be used on files with extension '{extension}'");
            }
            string fileName = Path.GetFileNameWithoutExtension(path);
            string parentDirectory = Path.GetDirectoryName(Path.GetFullPath(path)) ?? throw new Exception("Could not get parent directory path");
            string outputDirectory = Path.Combine(parentDirectory, fileName);

            if (Directory.Exists(outputDirectory))
            {
                Directory.Delete(outputDirectory, true);
            }
            Directory.CreateDirectory(outputDirectory);

            FastZip fastZip = new FastZip();
            fastZip.ExtractZip(path, outputDirectory, "");
        }
    }
}