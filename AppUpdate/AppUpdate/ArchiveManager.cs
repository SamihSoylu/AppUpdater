/*
 * Author: Samih Soylu.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppUpdate
{
    internal static class ZipArchiveEntryExtensions
    {
        public static bool IsFolder(this ZipArchiveEntry entry)
        {
            return entry.FullName.EndsWith("/");
        }
    }

    internal class ArchiveManager
    {
        public bool decompress(string zipPath, string extractPath = "")
        {
            try
            {
                using (ZipArchive archive = ZipFile.OpenRead(zipPath))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        try
                        {
                            if (entry.IsFolder())
                            {
                                Directory.CreateDirectory(Path.Combine(extractPath, entry.FullName));
                                continue;
                            }

                            entry.ExtractToFile(Path.Combine(extractPath, entry.FullName), true);

                            Console.WriteLine(" FILE " + entry.FullName + " [SUCCESS]");
                        }
                        catch (DirectoryNotFoundException)
                        {
                            Console.WriteLine(" DIR " + entry.FullName + " [FAIL]");
                            continue;
                        }
                        catch (FileNotFoundException)
                        {
                            Console.WriteLine(" FILE " + entry.FullName + " [FAIL]");
                            continue;
                        }
                    }
                }
            }
            catch (InvalidDataException)
            {
                Console.WriteLine(" ARCHIVE IS CORRUPT");
                return false;
            }

            Console.WriteLine("DECOMPRESS [SUCCESS]");
            return true;
        }
    }
}