using H9e.Tcp.Packet;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace H9e.Tcp {
    public class H9eTcpClient {

        public TcpClient Client { get; private set; }
        public H9eTcpServer Server { get; }
        public string ID { get; private set; } = null;
        public object Tag { get; set; } = null;
        public bool IsRunning { get; private set; } = true;

        private readonly IPAddress IP;
        private readonly int Port;
        private readonly object StopLock = new object();
        private readonly object SendLock = new object();

        private Thread ClientPacketThread;

        public event H9eTcpUtils.TcpPacketMessageDelegate OnPacketMessage;
        public event H9eTcpUtils.TcpClientExitDelegate OnDisconnect;

        public H9eTcpClient(string guid, H9eTcpServer server, TcpClient client) {
            Server = server;
            Client = client;
            ID = guid;
            Init();
        }

        public H9eTcpClient(string ip, int port) {
            IP = IPAddress.Parse(ip);
            Port = port;
        }

        public bool Connect() {
            try {
                Client = new TcpClient();
                Client.Connect(IP, Port);
                Init();
                return true;
            } catch (Exception) {
                return false;
            }
        }

        public void AutoConnect(int delay = 1000) {
            while (!Connect()) {
                if (!IsRunning) {
                    break;
                }
                Task.Delay(delay).Wait();
            }
        }

        private void Init() {
            IsRunning = true;
            ClientPacketThread = new Thread(TcpClientPacketReader) {
                IsBackground = true
            };
            ClientPacketThread.Start();
            if (Server != null) {
                if (ID != null) {
                    Send(SystemTcpPacket.Guid(ID));
                }
            }
        }

        private void TcpClientPacketReader() {
            try {
                while (IsRunning) {
                    int length = ReadInt();
                    if (length == -1) {
                        break;
                    }
                    byte[] buffer = ReadBuffer(length);
                    if (buffer == null) {
                        break;
                    }
                    var result = H9eTcpPacket.ToInstance(buffer);
                    if (result == null) {
                        break;
                    }
                    if (result is SystemTcpPacket system) {
                        if (system.Key == "guid") {
                            ID = system.Value;
                            continue;
                        }
                    }
                    OnPacketMessage?.Invoke(this, result);
                }
            } catch (Exception) {
                Stop(false);
            }
        }

        public void Send(IH9eTcpPacket packet) {
            lock (SendLock) {
                try {
                    if (IsRunning) {
                        byte[] buffer = packet.ToBytes();
                        Client.GetStream().Write(buffer, 0, buffer.Length);
                    }
                } catch (Exception) {
                    Stop(false);
                }
            }
        }

        public void SendPacket(IH9eTcpPacket captcha) {
            Send(captcha);
        }

        private int ReadInt() {
            byte[] buffer = ReadBuffer(4);
            if (buffer == null) {
                return -1;
            }
            return BitConverter.ToInt32(buffer, 0);
        }

        private byte[] ReadBuffer(int length) {
            byte[] buffer = new byte[length];
            int offset = 0;
            try {
                while (offset < length) {
                    int read = Client.GetStream().Read(buffer, offset, length - offset);
                    if (read == 0) {
                        Stop(false);
                        return null;
                    }
                    offset += read;
                }
                return buffer;
            } catch (Exception) {
                Stop(false);
                return null;
            }
        }

        public void Stop(bool join = true) {
            lock (StopLock) {
                if (IsRunning) {
                    IsRunning = false;
                    Client?.Close();
                    OnDisconnect?.Invoke(ID);
                }
            }
            if (ClientPacketThread != null) {
                if (join) {
                    ClientPacketThread.Join();
                }
                ClientPacketThread = null;
            }
        }
    }
}
