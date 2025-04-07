namespace H9e.Tcp.Packet {
    public class SystemTcpPacket : H9eTcpPacket {

        public string Key { get; private set; }
        public string Value { get; private set; }

        public SystemTcpPacket() : base("system") {
            
        }

        public static SystemTcpPacket Build(string type, object value) {
            return new SystemTcpPacket {
                Key = type,
                Value = value?.ToString()
            };
        }

        public static SystemTcpPacket Guid(string id) {
            return Build("guid", id);
        }
    }
}
