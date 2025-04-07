namespace H9e.Core.Compress {
    public interface ICompress {
        byte[] Compress(byte[] data);
        byte[] Decompress(byte[] data);
    }

    public class BaseCompress : ICompress {
        private readonly static object Lock = new object();
        private static BaseCompress Instance = null;
        public static BaseCompress GetInstance() {
            if (Instance == null) {
                lock (Lock) {
                    if (Instance == null) {
                        Instance = new BaseCompress();
                    }
                }
            }
            return Instance;
        }
        public byte[] Compress(byte[] data) {
            return data;
        }

        public byte[] Decompress(byte[] data) {
            return data;
        }

    }

}
