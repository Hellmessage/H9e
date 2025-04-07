using System;
using System.Text;

namespace H9e.HttpClient {
    public class H9eHttpMessage {
        public string Url { get; set; }
        public string RequestHeader { get; set; }
        public int StatusCode { get; set; } = -1;
        public string StatusDescription { get; set; }

        #region 响应头
        private string _responseHeader = null;
        public string[] ResponseHeaders { get; private set; } = null;
        public string ResponseHeader {
            get {
                return _responseHeader;
            }
            set {
                _responseHeader = value;
                if (value != null) {
                    ResponseHeaders = value.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    if (ResponseHeaders.Length > 0) {
                        StatusCode = int.Parse(ResponseHeaders[0].Split(' ')[1]);
                        StatusDescription = ResponseHeaders[0].Split(' ')[2];
                    }
                }
            }
        }
        #endregion

        #region 响应体
        public byte[] ResponseBuffer { get; set; }
        private string _responseBodyString = null;
        public string ResponseBody {
            get {
                if (_responseBodyString == null) {
                    if (ResponseBuffer != null && ResponseBuffer.Length > 0) {
                        _responseBodyString = Encoding.UTF8.GetString(ResponseBuffer);
                    }
                }
                return _responseBodyString;
            }
        }
        #endregion

        public Exception Error { get; set; } = new Exception("Unknow");

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("----------------------------------------------------------------------------------------------------------------");
            sb.Append($"请求URL:").AppendLine(Url).AppendLine();
            sb.Append($"请求头:").AppendLine(RequestHeader).AppendLine();
            sb.AppendLine().AppendLine();
            sb.Append($"响应码:").AppendLine($"{StatusCode} {StatusDescription}").AppendLine();
            sb.Append($"响应头:").AppendLine(ResponseHeader).AppendLine();
            sb.Append($"响应体:").AppendLine(ResponseBody).AppendLine();
            sb.AppendLine().AppendLine();
            sb.Append($"错误信息:").AppendLine(Error.Message).AppendLine();
            sb.AppendLine("----------------------------------------------------------------------------------------------------------------");
            sb.AppendLine().AppendLine();
            return sb.ToString();
        }

    }

}
