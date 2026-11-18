using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReduxHelper.Factory
{
    public static class DirectoryFactory
    {
        public static bool TryCreateDirectoriesIfNotExists(string path, params string[] dirs)
        {
            bool directoriesIfNotExists = true;
            if ((dirs != null ? (!Enumerable.Any<string>(dirs) ? 1 : 0) : 1) != 0)
                return directoriesIfNotExists;
            foreach (string dir in dirs ?? new string[0])
            {
                if (!TryCreateDirectoryIfNotExist(path, dir))
                    directoriesIfNotExists = false;
            }
            return directoriesIfNotExists;
        }

        public static bool TryCreateDirectoryIfNotExist(string path, string dir)
        {
            string path1 = Path.Combine(path, dir);
            if (!Directory.Exists(path1))
            {
                try
                {
                    Directory.CreateDirectory(path1);
                    if (!Directory.Exists(path1))
                        return false;
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }
    }
}
