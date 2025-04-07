namespace H9e.HttpClient {
    public class H9eHttpClientProxy {
        public string Host { get; set; }
        public int Port { get; set; }
        public string User { get; set; }
        public string Pass { get; set; }

        public static H9eHttpClientProxy Build(string host, int port, string user = null, string pass = null) {
            return new H9eHttpClientProxy() {
                Host = host,
                Port = port,
                User = user,
                Pass = pass,
            };
        }

        public static H9eHttpClientProxy Build(string address, string user = null, string pass = null) {
            return new H9eHttpClientProxy() {
                Host = address.Split(':')[0],
                Port = int.Parse(address.Split(':')[1]),
                User = user,
                Pass = pass,
            };
        }
    }
}
