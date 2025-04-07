using System;
using System.IO;

namespace H9e.Core {
    public class H9ePath {

        public static string Combine(string dir) => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dir);
        public static string Combine(params string[] dirs) {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            foreach (var dir in dirs) {
                path = Path.Combine(path, dir);
            }
            return path;
        }

    }
}
