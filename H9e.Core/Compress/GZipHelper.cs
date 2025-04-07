using System.IO;
using System.IO.Compression;

namespace H9e.Core.Compress {
    public class GZipHelper : ICompress {

        private readonly static object Lock = new object();
        private static GZipHelper Instance = null;
        public static GZipHelper GetInstance() {
            if (Instance == null) {
                lock (Lock) {
                    if (Instance == null) {
                        Instance = new GZipHelper();
                    }
                }
            }
            return Instance;
        }


        public byte[] Compress(byte[] data) {
            using (MemoryStream outputStream = new MemoryStream()) {
                using (GZipStream gzipStream = new GZipStream(outputStream, CompressionMode.Compress, true)) {
                    gzipStream.Write(data, 0, data.Length);
                }
                return outputStream.ToArray();
            }
        }

        public byte[] Decompress(byte[] data) {
            using (MemoryStream inputStream = new MemoryStream(data)) {
                using (GZipStream gzipStream = new GZipStream(inputStream, CompressionMode.Decompress)) {
                    using (MemoryStream outputStream = new MemoryStream()) {
                        gzipStream.CopyTo(outputStream);
                        return outputStream.ToArray();
                    }
                }
            }
        }

        
    }
}
