using H9e.Tcp.Packet;

namespace H9e.Tcp {
    public class H9eTcpUtils {

        public static void Init() {
            System.Net.ServicePointManager.Expect100Continue = false;
            System.Net.ServicePointManager.DefaultConnectionLimit = 500;
            H9eTcpPacket.RegisterPacket<FileTcpPacket>("file");
        }


    }
}
