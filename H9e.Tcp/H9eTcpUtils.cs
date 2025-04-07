using H9e.Tcp.Packet;

namespace H9e.Tcp {
    public class H9eTcpUtils {

        public delegate void TcpClientExitDelegate(string guid);
        public delegate void TcpPacketMessageDelegate(H9eTcpClient client, IH9eTcpPacket packet);

        public static void Init() {
            System.Net.ServicePointManager.Expect100Continue = false;
            System.Net.ServicePointManager.DefaultConnectionLimit = 500;
            System.Net.ServicePointManager.UseNagleAlgorithm = false;
            H9eTcpPacket.RegisterPacket<FileTcpPacket>("file");
            H9eTcpPacket.RegisterPacket<SystemTcpPacket>("system");
        }
    }
}
