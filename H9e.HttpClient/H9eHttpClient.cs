using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace H9e.HttpClient {

    public class H9eHttpClient {
        private readonly static List<int> RedirectCode = new List<int> { 301, 302, 307 };
        public static bool IsDebug { get; set; } = false;
        public static bool IsSaveLog { get; set; } = false;
        private readonly List<string> AllLogs = new List<string>();

        #region 请求头配置
        public readonly H9eHeaderManage HeaderManage = new H9eHeaderManage();
        public H9eHttpClient SetHeader(string key, string value) {
            HeaderManage.Set(key, value);
            return this;
        }
        public H9eHttpClient Set(H9eHttpHeader key, string value) {
            HeaderManage.Set(key, value);
            return this;
        }
        public H9eHttpClient SetHeader(H9eHttpHeader key, string value) {
            HeaderManage.Set(key, value);
            return this;
        }
        public H9eHttpClient SetUserAgent(string value) {
            HeaderManage.Set(H9eHttpHeader.UserAgent, value);
            return this;
        }
        public H9eHttpClient SetAccept(string value) {
            HeaderManage.Set(H9eHttpHeader.Accept, value);
            return this;
        }
        public H9eHttpClient SetAcceptLanguage(string value) {
            HeaderManage.Set(H9eHttpHeader.AcceptLanguage, value);
            return this;
        }
        public H9eHttpClient SetAcceptEncoding(string value) {
            HeaderManage.Set(H9eHttpHeader.AcceptEncoding, value);
            return this;
        }
        public H9eHttpClient SetCacheControl(string value) {
            HeaderManage.Set(H9eHttpHeader.CacheControl, value);
            return this;
        }
        public H9eHttpClient SetPragma(string value) {
            HeaderManage.Set(H9eHttpHeader.Pragma, value);
            return this;
        }
        public H9eHttpClient SetPriority(string value) {
            HeaderManage.Set(H9eHttpHeader.Priority, value);
            return this;
        }
        public H9eHttpClient SetSecCHUA(string value) {
            HeaderManage.Set(H9eHttpHeader.SecCHUA, value);
            return this;
        }
        public H9eHttpClient SetSecCHUAMobile(string value) {
            HeaderManage.Set(H9eHttpHeader.SecCHUAMobile, value);
            return this;
        }
        public H9eHttpClient SetSecCHUAPlatform(string value) {
            HeaderManage.Set(H9eHttpHeader.SecCHUAPlatform, value);
            return this;
        }
        public H9eHttpClient SetSecFetchDest(string value) {
            HeaderManage.Set(H9eHttpHeader.SecFetchDest, value);
            return this;
        }
        public H9eHttpClient SetSecFetchMode(string value) {
            HeaderManage.Set(H9eHttpHeader.SecFetchMode, value);
            return this;
        }
        public H9eHttpClient SetSecFetchSite(string value) {
            HeaderManage.Set(H9eHttpHeader.SecFetchSite, value);
            return this;
        }
        public H9eHttpClient SetSecFetchUser(string value) {
            HeaderManage.Set(H9eHttpHeader.SecFetchUser, value);
            return this;
        }
        public H9eHttpClient SetUpgradeInsecureRequests(string value) {
            HeaderManage.Set(H9eHttpHeader.UpgradeInsecureRequests, value);
            return this;
        }
        public H9eHttpClient SetReferer(string value) {
            HeaderManage.Set(H9eHttpHeader.Referer, value);
            return this;
        }
        public H9eHttpClient SetOrigin(string value) {
            HeaderManage.Set(H9eHttpHeader.Origin, value);
            return this;
        }
        public H9eHttpClient SetCookie(string value) {
            HeaderManage.Set(H9eHttpHeader.Cookie, value);
            return this;
        }
        public H9eHttpClient SetContentType(string value) {
            HeaderManage.Set(H9eHttpHeader.ContentType, value);
            return this;
        }

        #endregion

        #region Cookie配置
        public readonly H9eCookieManage CookieManage = new H9eCookieManage();
        public H9eHttpClient SetCookie(string key, string value) {
            CookieManage.Set(key, value);
            return this;
        }
        #endregion

        #region 代理配置
        private bool UseProxy { get; set; } = false;
        private H9eHttpClientProxy Proxy { get; set; }
        public void SetProxy(string host, int port, string user = null, string pass = null) {
            UseProxy = true;
            Proxy = H9eHttpClientProxy.Build(host, port, user, pass);
        }
        public void SetProxy(string address, string user = null, string pass = null) {
            UseProxy = true;
            Proxy = H9eHttpClientProxy.Build(address, user, pass);
        }
        public void SetProxy(H9eHttpClientProxy proxy) {
            if (proxy != null) {
                UseProxy = true;
                Proxy = proxy;
            }
        }
        #endregion

        #region 请求方法

        public H9eHttpMessage Get(string url, H9eHeaderManage header = null, int timeout = 20000, bool readBody = true, bool redirect = true) {
            return Request("GET", url, null, header, timeout, readBody, redirect).Result;
        }

        public H9eHttpMessage Post(string url, string body, H9eHeaderManage header = null, int timeout = 20000, bool readBody = true, bool redirect = true) {
            return Request("POST", url, body, header, timeout, readBody, redirect).Result;
        }

        public H9eHttpMessage Head(string url, H9eHeaderManage header = null, int timeout = 20000, bool readBody = true, bool redirect = true) {
            return Request("HEAD", url, null, header, timeout, readBody, redirect).Result;
        }

        public H9eHttpMessage Put(string url, string body, H9eHeaderManage header = null, int timeout = 20000, bool readBody = true, bool redirect = true) {
            return Request("PUT", url, body, header, timeout, readBody, redirect).Result;
        }

        public H9eHttpMessage Delete(string url, H9eHeaderManage header = null, int timeout = 20000, bool readBody = true, bool redirect = true) {
            return Request("DELETE", url, null, header, timeout, readBody, redirect).Result;
        }

        public H9eHttpMessage Options(string url, H9eHeaderManage header = null, int timeout = 20000, bool readBody = true, bool redirect = true) {
            return Request("OPTIONS", url, null, header, timeout, readBody, redirect).Result;
        }

        public H9eHttpMessage Trace(string url, H9eHeaderManage header = null, int timeout = 20000, bool readBody = true, bool redirect = true) {
            return Request("TRACE", url, null, header, timeout, readBody, redirect).Result;
        }

        public H9eHttpMessage Connect(string url, H9eHeaderManage header = null, int timeout = 20000, bool readBody = true, bool redirect = true) {
            return Request("CONNECT", url, null, header, timeout, readBody, redirect).Result;
        }

        public H9eHttpMessage Patch(string url, string body, H9eHeaderManage header = null, int timeout = 20000, bool readBody = true, bool redirect = true) {
            return Request("PATCH", url, body, header, timeout, readBody, redirect).Result;
        }

        #endregion

        #region 异步请求方法

        public async Task<H9eHttpMessage> GetAsync(string url, H9eHeaderManage header = null, int timeout = 20000, bool readBody = true, bool redirect = true) {
            return await Request("GET", url, null, header, timeout, readBody, redirect);
        }

        public async Task<H9eHttpMessage> PostAsync(string url, string body, H9eHeaderManage header = null, int timeout = 20000, bool readBody = true, bool redirect = true) {
            return await Request("POST", url, body, header, timeout, readBody, redirect);
        }

        public async Task<H9eHttpMessage> HeadAsync(string url, H9eHeaderManage header = null, int timeout = 20000, bool readBody = true, bool redirect = true) {
            return await Request("HEAD", url, null, header, timeout, readBody, redirect);
        }

        public async Task<H9eHttpMessage> PutAsync(string url, string body, H9eHeaderManage header = null, int timeout = 20000, bool readBody = true, bool redirect = true) {
            return await Request("PUT", url, body, header, timeout, readBody, redirect);
        }

        public async Task<H9eHttpMessage> DeleteAsync(string url, H9eHeaderManage header = null, int timeout = 20000, bool readBody = true, bool redirect = true) {
            return await Request("DELETE", url, null, header, timeout, readBody, redirect);
        }

        public async Task<H9eHttpMessage> OptionsAsync(string url, H9eHeaderManage header = null, int timeout = 20000, bool readBody = true, bool redirect = true) {
            return await Request("OPTIONS", url, null, header, timeout, readBody, redirect);
        }

        public async Task<H9eHttpMessage> TraceAsync(string url, H9eHeaderManage header = null, int timeout = 20000, bool readBody = true, bool redirect = true) {
            return await Request("TRACE", url, null, header, timeout, readBody, redirect);
        }

        public async Task<H9eHttpMessage> ConnectAsync(string url, H9eHeaderManage header = null, int timeout = 20000, bool readBody = true, bool redirect = true) {
            return await Request("CONNECT", url, null, header, timeout, readBody, redirect);
        }

        public async Task<H9eHttpMessage> PatchAsync(string url, string body, H9eHeaderManage header = null, int timeout = 20000, bool readBody = true, bool redirect = true) {
            return await Request("PATCH", url, body, header, timeout, readBody, redirect);
        }

        #endregion

        #region 请求核心

        private async Task<H9eHttpMessage> Request(string method, string url, string body = null, H9eHeaderManage header = null, int timeout = 20000, bool readBody = true, bool redirect = true) {
            H9eHttpMessage message = new H9eHttpMessage() {
                Url = url,
            };
            try {
                DebugLog("请求地址:", url);
                string userAgent = GetUserAgent(header);
                string headerString = HeaderManage.ToHeaderString(header);
                string totalUrl = url;
                while (true) {
                    Uri uri = new Uri(totalUrl);
                    using (TcpClient client = await ConnectToServer(uri, userAgent, timeout)) {
                        if (client == null) {
                            throw new TimeoutException("连接服务器超时");
                        }
                        using (Stream stream = await AuthenticateSslStream(client, uri)) {
                            message.RequestHeader = BuildRequest(method, uri, headerString, body);
                            byte[] requestBytes = Encoding.UTF8.GetBytes(message.RequestHeader);
                            DebugLog("请求头:", message.RequestHeader);
                            await stream.WriteAsync(requestBytes, 0, requestBytes.Length);
                            byte[] responseHeaderBytes = await ReadResponseHeader(stream, timeout);
                            message.ResponseHeader = Encoding.UTF8.GetString(responseHeaderBytes);
                            DebugLog("响应头:", message.ResponseHeader);
                            if (redirect) {
                                if (RedirectCode.IndexOf(message.StatusCode) != -1) {
                                    if (IsSaveLog) {
                                        AllLogs.Add(message.ToString());
                                    }
                                    string locationHeader = Array.Find(message.ResponseHeaders, line => line.StartsWith("Location:", StringComparison.OrdinalIgnoreCase));
                                    if (locationHeader != null) {
                                        string newUrl = locationHeader.Split(':')[1].Trim();
                                        if (!newUrl.StartsWith("http")) {
                                            newUrl = new Uri(uri, newUrl).ToString();
                                        }
                                        totalUrl = newUrl;
                                        DebugLog($"重定向到: {totalUrl}");
                                        continue;
                                    }
                                }
                            }
                            if (!readBody) {
                                return message;
                            }
                            (int contentLength, bool isChunked) = ParseResponseHeaders(message.ResponseHeader);
                            message.ResponseBuffer = await ReadResponseBody(stream, timeout, contentLength, isChunked);
                            DebugLog("响应体:", message.ResponseBuffer.Length);
                            DebugLog("响应体:", message.ResponseBody);
                            return message;
                        }
                    }
                }
            } catch (Exception ex) {
                message.StatusCode = -1;
                message.StatusDescription = ex.Message;
                message.Error = ex;
            } finally {
                if (IsSaveLog) {
                    AllLogs.Add(message.ToString());
                }
            }
            return message;
        }


        private async Task<TcpClient> ConnectToServer(Uri uri, string userAgent, int timeout) {
            Task<TcpClient> task = Task.Run(async () => {
                TcpClient client;
                int port = uri.Port;
                if (UseProxy) {
                    client = new TcpClient(Proxy.Host, Proxy.Port);
                    NetworkStream stream = client.GetStream();
                    string proxyRequest = $"CONNECT {uri.Host}:{port} HTTP/1.1\r\n" +
                                          $"Host: {uri.Host}\r\n" +
                                          $"User-Agent: {userAgent}\r\n";
                    if (!string.IsNullOrWhiteSpace(Proxy.User)) {
                        string authInfo = $"{Proxy.User}:{Proxy.Pass}";
                        authInfo = Convert.ToBase64String(Encoding.UTF8.GetBytes(authInfo));
                        proxyRequest += $"Proxy-Authorization: Basic {authInfo}\r\n";
                    }
                    proxyRequest += "\r\n";
                    byte[] proxyRequestBytes = Encoding.UTF8.GetBytes(proxyRequest);
                    await stream.WriteAsync(proxyRequestBytes, 0, proxyRequestBytes.Length);
                    byte[] buffer = new byte[1024];
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    string proxyResponse = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    if (!proxyResponse.ToLower().Contains("200 Connection established".ToLower())) {
                        client.Close();
                        throw new Exception("Proxy connection failed");
                    }
                } else {
                    client = new TcpClient(uri.Host, port);
                }
                return client;
            });
            if (await Task.WhenAny(task, Task.Delay(timeout)) == task) {
                return await task;
            }
            return null;
        }

        private async Task<Stream> AuthenticateSslStream(TcpClient client, Uri uri) {
            if (uri.Scheme == "https") {
                SslStream ssl = new SslStream(client.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
                await ssl.AuthenticateAsClientAsync(uri.Host, null, SslProtocols.Tls12, false);
                return ssl;
            } else {
                return client.GetStream();
            }
        }

        private static string BuildRequest(string method, Uri uri, string headers, string body) {
            string request = $"{method} {uri.PathAndQuery} HTTP/1.1\r\n" +
                             $"Host: {uri.Host}\r\n" +
                             $"{headers}\r\n";
            if (method == "POST" || method == "PUT" || method == "PATCH") {
                if (body == null) {
                    body = "";
                }
                request += $"Content-Length: {body.Length}\r\n\r\n" +
                           $"{body}";
            } else {
                request += "\r\n";
            }
            return request;
        }

        private static async Task<byte[]> ReadResponseHeader(Stream stream, int timeout) {
            using (MemoryStream headerStream = new MemoryStream()) {
                byte[] headerBuffer = new byte[1];
                while (true) {
                    var readTask = stream.ReadAsync(headerBuffer, 0, 1);
                    if (await Task.WhenAny(readTask, Task.Delay(timeout)) == readTask) {
                        int bytesRead = await readTask;
                        if (bytesRead == 0) {
                            break;
                        }
                        headerStream.Write(headerBuffer, 0, bytesRead);
                        if (headerStream.Length >= 4 && Encoding.UTF8.GetString(headerStream.GetBuffer(), (int)headerStream.Length - 4, 4) == "\r\n\r\n") {
                            break;
                        }
                    } else {
                        throw new TimeoutException("读取响应头超时");
                    }
                }
                return headerStream.ToArray();
            }  
        }

        private static (int contentLength, bool isChunked) ParseResponseHeaders(string responseHeader) {
            int contentLength = 0;
            bool isChunked = false;
            string[] headerLines = responseHeader.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in headerLines) {
                if (line.StartsWith("Content-Length:", StringComparison.OrdinalIgnoreCase)) {
                    string lengthValue = line.Split(':')[1].Trim();
                    if (int.TryParse(lengthValue, out contentLength)) {
                        break;
                    }
                } else if (line.StartsWith("Transfer-Encoding:", StringComparison.OrdinalIgnoreCase)) {
                    string encodingValue = line.Split(':')[1].Trim();
                    if (encodingValue.Equals("chunked", StringComparison.OrdinalIgnoreCase)) {
                        isChunked = true;
                    }
                }
            }
            return (contentLength, isChunked);
        }

        private static async Task<byte[]> ReadResponseBody(Stream stream, int timeout, int contentLength, bool isChunked) {
            using (MemoryStream responseBodyStream = new MemoryStream()) {
                if (isChunked) {
                    while (true) {
                        byte[] chunkSizeBytes = await ReadChunkSize(stream, timeout);
                        if (chunkSizeBytes.Length == 0) {
                            break;
                        }
                        string chunkSizeStr = Encoding.UTF8.GetString(chunkSizeBytes).Trim();
                        int chunkSize = int.Parse(chunkSizeStr, System.Globalization.NumberStyles.HexNumber);
                        if (chunkSize == 0) {
                            await ReadEndOfChunk(stream, timeout);
                            break;
                        }
                        byte[] chunkBuffer = new byte[chunkSize];
                        int totalBytesRead = await ReadChunkContent(stream, timeout, chunkBuffer, chunkSize);
                        responseBodyStream.Write(chunkBuffer, 0, totalBytesRead);
                        await ReadChunkSeparator(stream, timeout);
                    }
                } else {
                    byte[] bodyBuffer = new byte[contentLength];
                    int totalBytesRead = 0;
                    while (totalBytesRead < contentLength) {
                        var readTask = stream.ReadAsync(bodyBuffer, totalBytesRead, contentLength - totalBytesRead);
                        if (await Task.WhenAny(readTask, Task.Delay(timeout)) == readTask) {
                            int bytesRead = await readTask;
                            if (bytesRead == 0) {
                                break;
                            }
                            totalBytesRead += bytesRead;
                        } else {
                            throw new TimeoutException("读取响应体超时");
                        }
                    }
                    responseBodyStream.Write(bodyBuffer, 0, totalBytesRead);
                }
                return responseBodyStream.ToArray();
            }
        }

        private static async Task<byte[]> ReadChunkSize(Stream stream, int timeout) {
            using (MemoryStream chunkSizeStream = new MemoryStream()) {
                byte[] chunkSizeBuffer = new byte[1];
                while (true) {
                    var readTask = stream.ReadAsync(chunkSizeBuffer, 0, 1);
                    if (await Task.WhenAny(readTask, Task.Delay(timeout)) == readTask) {
                        int bytesRead = await readTask;
                        if (bytesRead == 0) {
                            break;
                        }
                        char c = Encoding.UTF8.GetString(chunkSizeBuffer, 0, bytesRead)[0];
                        if (c == '\r') {
                            continue;
                        }
                        if (c == '\n') {
                            break;
                        }
                        chunkSizeStream.Write(chunkSizeBuffer, 0, bytesRead);
                    } else {
                        throw new TimeoutException("读取块大小超时");
                    }
                }
                return chunkSizeStream.ToArray();
            }  
        }

        private static async Task ReadEndOfChunk(Stream stream, int timeout) {
            byte[] endBuffer = new byte[2];
            var readTask = stream.ReadAsync(endBuffer, 0, 2);
            if (await Task.WhenAny(readTask, Task.Delay(timeout)) != readTask) {
                throw new TimeoutException("读取块结束符超时");
            }
        }

        private static async Task<int> ReadChunkContent(Stream stream, int timeout, byte[] chunkBuffer, int chunkSize) {
            int totalBytesRead = 0;
            while (totalBytesRead < chunkSize) {
                var readTask = stream.ReadAsync(chunkBuffer, totalBytesRead, chunkSize - totalBytesRead);
                if (await Task.WhenAny(readTask, Task.Delay(timeout)) == readTask) {
                    int bytesRead = await readTask;
                    if (bytesRead == 0) {
                        break;
                    }
                    totalBytesRead += bytesRead;
                } else {
                    throw new TimeoutException("读取块内容超时");
                }
            }
            return totalBytesRead;
        }

        private static async Task ReadChunkSeparator(Stream stream, int timeout) {
            byte[] separatorBuffer = new byte[2];
            var readTask = stream.ReadAsync(separatorBuffer, 0, 2);
            if (await Task.WhenAny(readTask, Task.Delay(timeout)) != readTask) {
                throw new TimeoutException("读取块分隔符超时");
            }
        }

        private string GetUserAgent(H9eHeaderManage header) {
            string userAgent = HeaderManage["User-Agent"];
            if (string.IsNullOrEmpty(userAgent)) {
                userAgent = header?["User-Agent"];
                if (string.IsNullOrEmpty(userAgent)) {
                    userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/134.0.0.0 Safari/537.36";
                }
            }
            return userAgent;
        }

        private bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
            return true;
        }

        #endregion

        #region Curl请求

        public static H9eHttpMessage Curl_POST(string url, string body, H9eHttpClientProxy proxy = null) {
            H9eHttpClient client = new H9eHttpClient();
            client.SetUserAgent("curl");
            client.SetProxy(proxy);
            return client.Post(url, body);
        }

        public static H9eHttpMessage Curl_GET(string url, H9eHttpClientProxy proxy = null) {
            H9eHttpClient client = new H9eHttpClient();
            client.SetUserAgent("curl");
            client.SetProxy(proxy);
            return client.Get(url);
        }

        #endregion


        public void SaveLoggerToFile(string file) {
            try {
                using (StreamWriter writer = new StreamWriter(file)) {
                    writer.WriteLine(string.Join("\r\n", AllLogs));
                    writer.Flush();
                }
            } catch (Exception) {

            }
        }

        private static void DebugLog(params object[] argv) {
            if (IsDebug) {
                Console.WriteLine("------------------------------------------------------------");
                Console.WriteLine(string.Join("\r\n", argv));
            }
        }
    }

}
