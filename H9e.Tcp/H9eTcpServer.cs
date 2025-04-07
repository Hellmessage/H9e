using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace H9e.Tcp {
    public class H9eTcpServer : IDisposable {

        private readonly IPAddress IP;
        private readonly int Port;
        private TcpListener Server;
        private Thread ServerAcceptThread;
        private bool IsRunning = false;
        private readonly object ClientLock = new object();
        private readonly Dictionary<string, H9eTcpClient> Clients = new Dictionary<string, H9eTcpClient>();
        public int GetClientCount() {
            return Clients.Count;
        }

        public event H9eTcpUtils.TcpPacketMessageDelegate OnPacketMessage;
        public event H9eTcpUtils.TcpClientExitDelegate OnClientExit;

        private void ClientDisconnect(string guid) {
            lock (ClientLock) {
                if (Clients.ContainsKey(guid)) {
                    Clients.Remove(guid);
                }
            }
            OnClientExit.Invoke(guid);
        }

        public H9eTcpServer(int port) {
            Port = port;
            IP = IPAddress.Any;
        }

        public H9eTcpServer(string ip, int port) {
            IP = IPAddress.Parse(ip);
            Port = port;
        }

        public void Start(int backlog = 100) {
            if (!IsRunning) {
                IsRunning = true;
                Server = new TcpListener(IP, Port);
                Server.Start(backlog);
                ServerAcceptThread = new Thread(HandleTcpClient) {
                    IsBackground = true
                };
                ServerAcceptThread.Start();
            }
        }

        public void Stop() {
            if (IsRunning) {
                IsRunning = false;
                if (Clients.Count > 0) {
                    lock (ClientLock) {
                        var all = Clients.ToArray();
                        foreach (var client in all) {
                            if (client.Value.IsRunning) {
                                client.Value.Stop();
                            }
                        }
                        Clients.Clear();
                    }
                }
                Server.Stop();
                ServerAcceptThread.Join();
            }
        }

        public void Dispose() {
            Stop();
        }

        private void HandleTcpClient() {
            while (IsRunning) {
                try {
                    TcpClient socket = Server.AcceptTcpClient();
                    if (socket != null) {
                        string guid = Guid.NewGuid().ToString();
                        H9eTcpClient client = new H9eTcpClient(guid, this, socket);
                        client.OnPacketMessage += OnPacketMessage;
                        client.OnDisconnect += ClientDisconnect;
                        lock (ClientLock) {
                            Clients.Add(guid, client);
                        }
                    }
                } catch (Exception) {
                    continue;
                }
            }
        }

        
    }
}
