using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platforma.Application.Files
{
    public class FilesUtils
    {
        public static void DeleteEmptyDirectoriesUpwards(string path)
        {
            var dir = Path.GetDirectoryName(path);
            while (!string.IsNullOrEmpty(dir) && Directory.Exists(dir))
            {
                // If directory is empty, delete it and move up
                if (Directory.GetFileSystemEntries(dir).Length == 0)
                {
                    Directory.Delete(dir);
                    dir = Path.GetDirectoryName(dir);
                }
                else
                {
                    // Stop if directory is not empty
                    break;
                }
            }
        }

        // Rollback file upload if error occurs
        public static void RollbackFileUpload(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                    DeleteEmptyDirectoriesUpwards(path);
                }
            }
            catch
            {
                Console.WriteLine($"Error deleting file during rollback");
            }
        }
    }
}
