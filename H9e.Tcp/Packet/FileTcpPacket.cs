using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace H9e.Tcp.Packet {
    public class FileTcpPacket : H9eTcpPacket {
        public FileTcpPacket() : base("file") {

        }

        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public string FileHash { get; set; }
        public long FileSize { get; set; }
        public byte[] FileData { get; set; }

        public bool SaveToPath(string path, string name) {
            try {
                string currentHash = CalculateFileHash(FileData);
                if (currentHash != FileHash) {
                    return false;
                }
                if (!Directory.Exists(path)) {
                    Directory.CreateDirectory(path);
                }
                string fullPath = Path.Combine(path, $"{name}{FileExtension}");
                File.WriteAllBytes(fullPath, FileData);
                return true;
            } catch (Exception) {
                return false;
            }
        }

        public static FileTcpPacket Create(string path) {
            try {
                FileInfo info = new FileInfo(path);
                if (!info.Exists) {
                    return null;
                }
                byte[] data = File.ReadAllBytes(path);
                return new FileTcpPacket() {
                    FilePath = path,
                    FileName = info.Name,
                    FileExtension = info.Extension,
                    FileHash = CalculateFileHash(data),
                    FileSize = info.Length,
                    FileData = data
                };
            } catch (Exception) {
                return null;
            }
        }

        private static string CalculateFileHash(byte[] data) {
            using (SHA256 sha256 = SHA256.Create()) {
                byte[] hashBytes = sha256.ComputeHash(data);
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++) {
                    builder.Append(hashBytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
