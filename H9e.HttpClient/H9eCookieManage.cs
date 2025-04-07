using System;
using System.Collections.Generic;
using System.Linq;

namespace H9e.HttpClient {
    public class H9eCookieManage : Dictionary<string, string> {

        private readonly HashSet<string> UniqueCookieList = new HashSet<string>();

        public string[] GetAllCookieList() {
            return UniqueCookieList.ToArray();
        }

        public string GetByName(string key) {
            return ContainsKey(key) ? this[key] : "";
        }

        public H9eCookieManage Set(string key, string value) {
            this[key] = value;
            return this;
        }

        public H9eCookieManage Set(H9eCookieManage cookies) {
            if (cookies != null) {
                foreach (var kv in cookies) {
                    this[kv.Key] = kv.Value;
                }
            }
            return this;
        }

        public void Set(string responseHeader) {
            string[] headers = responseHeader.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in headers) {
                if (line.StartsWith("Set-Cookie:", StringComparison.OrdinalIgnoreCase)) {
                    string cookie = line.Split(':')[1].Trim();
                    UniqueCookieList.Add(cookie);
                    string[] kv = cookie.Split(';');
                    int pos = kv[0].IndexOf('=');
                    if (pos > 0) {
                        this[kv[0].Substring(0, pos)] = kv[0].Substring(pos + 1);
                    }
                }
            }
        }

        public override string ToString() {
            return string.Join("; ", this.Select(kv => $"{kv.Key}={kv.Value}"));
        }
    }
}
