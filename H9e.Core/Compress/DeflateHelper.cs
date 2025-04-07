using System.IO;
using System.IO.Compression;

namespace H9e.Core.Compress {
    public class DeflateHelper : ICompress {

        private readonly static object Lock = new object();
        private static DeflateHelper Instance = null;
        public static DeflateHelper GetInstance() {
            if (Instance == null) {
                lock (Lock) {
                    if (Instance == null) {
                        Instance = new DeflateHelper();
                    }
                }
            }
            return Instance;
        }

        public byte[] Compress(byte[] data) {
            using (MemoryStream outputStream = new MemoryStream()) {
                using (DeflateStream deflateStream = new DeflateStream(outputStream, CompressionLevel.Optimal)) {
                    deflateStream.Write(data, 0, data.Length);
                }
                return outputStream.ToArray();
            }
        }

        public byte[] Decompress(byte[] data) {
            using (MemoryStream inputStream = new MemoryStream(data)) {
                using (DeflateStream deflateStream = new DeflateStream(inputStream, CompressionMode.Decompress)) {
                    using (MemoryStream outputStream = new MemoryStream()) {
                        deflateStream.CopyTo(outputStream);
                        return outputStream.ToArray();
                    }
                }
            }
        }
    }
}
