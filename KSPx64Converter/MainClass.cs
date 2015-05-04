using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Resources;

namespace KSPx64Converter
{
    public class MainClass
    {
        private static string currentPath = Path.GetDirectoryName(typeof(MainClass).Assembly.Location);
        private const string SUPPORTED_HASH = "76be8d3761acb6afa2a51c1b666b032d3a0277ae0f15d596f3c648fee2017204";

        public static void Main()
        {
            if (!CheckDirectory())
            {
                Console.WriteLine("Please place this program next to KSP.exe");
                Console.ReadKey();
                return;
            }
            if (!CheckVersion())
            {
                Console.WriteLine("Incompatible version detected. This program only supports KSP 1.0.x");
                Console.ReadKey();
                return;
            }
            if (!CheckDisclaimer())
            {

                Console.Write("You did ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("not");
                Console.ResetColor();
                Console.WriteLine(" accept the disclaimer. Goodbye!");
                Console.ReadKey();
                return;
            }
            Console.WriteLine("Copying KSP_Data to KSP_x64_Data");
            CopyKSPData();
            Console.WriteLine("Installing 64bit files");
            Install64Files();
            Console.WriteLine("KSP 64bit installation complete!");
            Console.ReadKey();
        }


        private static bool CheckDirectory()
        {
            if (!File.Exists(Path.Combine(currentPath, "KSP.exe")))
            {
                return false;
            }
            if (!Directory.Exists(Path.Combine(currentPath, "KSP_Data")))
            {
                return false;
            }
            return true;
        }

        private static bool CheckDisclaimer()
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write("WARNING: The 64 bit windows version of KSP is ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("unstable");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("!");
            Console.ResetColor();
            Console.Write("It is ");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write("highly recommended ");
            Console.ResetColor();
            Console.Write("to first try ");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write("-force-opengl");
            Console.ResetColor();
            Console.WriteLine(" with the 32bit version of KSP before attempting to use the 64bit version.");
            Console.WriteLine();
            Console.WriteLine("You may only use this program with the following restrictions:");
            Console.WriteLine("1. You will not ask squad for support.");
            Console.WriteLine("2. You will not ask modders for support.");
            Console.WriteLine();
            Console.ResetColor();
            Console.Write("If you accept these conditions, type ");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write("yes");
            Console.ResetColor();
            Console.WriteLine(" and then press enter");
            return (Console.ReadLine().ToLower() == "yes");
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDirName);
            }

            // If the destination directory doesn't exist, create it. 
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location. 
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        private static bool CopyKSPData()
        {
            string srcPath = Path.Combine(currentPath, "KSP_Data");
            string dstPath = Path.Combine(currentPath, "KSP_x64_Data");
            if (Directory.Exists(dstPath))
            {
                return true;
            }
            DirectoryCopy(srcPath, dstPath, true);
            return true;
        }

        private static bool Install64Files()
        {
            string kspPath = Path.Combine(currentPath, "KSP_x64.exe");
            string monoPath = Path.Combine(currentPath, "KSP_x64_Data", "Mono", "mono.dll");
            byte[] kspBytes = GetStreamBytes("KSPx64Converter.KSP.exe.gz");
            byte[] monoBytes = GetStreamBytes("KSPx64Converter.mono.dll.gz");
            if (!File.Exists(kspPath))
            {
                File.WriteAllBytes(kspPath, kspBytes);
            }
            File.Delete(monoPath);
            File.WriteAllBytes(monoPath, monoBytes);
            return true;
        }

        private static byte[] GetStreamBytes(string resourceName)
        {
            byte[] retBytes = null;
            Stream compressedStream = typeof(MainClass).Assembly.GetManifestResourceStream(resourceName);
            using (MemoryStream ms = new MemoryStream())
            {
                using (GZipStream gzs = new GZipStream(compressedStream, CompressionMode.Decompress))
                {

                    gzs.CopyTo(ms);
                }
                retBytes = ms.ToArray();
            }
            return retBytes;
        }

        private static bool CheckVersion()
        {
            string monoPath = Path.Combine(currentPath, "KSP_Data", "Mono", "mono.dll");
            string fileHash = null;
            using (SHA256Managed sha = new SHA256Managed())
            {
                using (FileStream fs = new FileStream(monoPath, FileMode.Open))
                {
                    byte[] hash = sha.ComputeHash(fs);
                    foreach (byte b in hash)
                    {
                        fileHash += b.ToString("X2").ToLower();
                    }

                }
            }
            return fileHash == SUPPORTED_HASH;
        }
    }
}

