using System;
using System.Collections.Generic;
using System.Linq;

namespace H9e.HttpClient {

    public enum H9eHttpHeader {
        UserAgent,
        Accept,
        AcceptLanguage,
        AcceptEncoding,
        CacheControl,
        Pragma,
        Priority,
        SecCHUA,
        SecCHUAMobile,
        SecCHUAPlatform,
        SecFetchDest,
        SecFetchMode,
        SecFetchSite,
        SecFetchUser,
        UpgradeInsecureRequests,
        Referer,
        Origin,
        Cookie,
        ContentType,
    }

    public class H9eHeaderManage : Dictionary<string, string> {
        private readonly static Dictionary<H9eHttpHeader, string> HttpHeaderDict = new Dictionary<H9eHttpHeader, string> {
            [H9eHttpHeader.UserAgent] = "User-Agent",
            [H9eHttpHeader.Accept] = "Accept",
            [H9eHttpHeader.AcceptLanguage] = "Accept-Language",
            [H9eHttpHeader.AcceptEncoding] = "Accept-Encoding",
            [H9eHttpHeader.CacheControl] = "Cache-Control",
            [H9eHttpHeader.Pragma] = "Pragma",
            [H9eHttpHeader.Priority] = "Priority",
            [H9eHttpHeader.SecCHUA] = "Sec-CH-UA",
            [H9eHttpHeader.SecCHUAMobile] = "Sec-CH-UA-Mobile",
            [H9eHttpHeader.SecCHUAPlatform] = "Sec-CH-UA-Platform",
            [H9eHttpHeader.SecFetchDest] = "Sec-Fetch-Dest",
            [H9eHttpHeader.SecFetchMode] = "Sec-Fetch-Mode",
            [H9eHttpHeader.SecFetchSite] = "Sec-Fetch-Site",
            [H9eHttpHeader.SecFetchUser] = "Sec-Fetch-User",
            [H9eHttpHeader.UpgradeInsecureRequests] = "Upgrade-Insecure-Requests",
            [H9eHttpHeader.Referer] = "Referer",
            [H9eHttpHeader.Origin] = "Origin",
            [H9eHttpHeader.Cookie] = "Cookie",
            [H9eHttpHeader.ContentType] = "Content-Type",
        };

        public H9eHeaderManage Set(H9eHttpHeader key, string value) {
            AddHeader(key, value);
            return this;
        }

        public H9eHeaderManage Set(string key, string value) {
            AddHeader(key, value);
            return this;
        }

        public new void Add(string key, string value) {
            AddHeader(key, value);
        }

        public H9eHeaderManage AddHeader(H9eHttpHeader key, string value) {
            this[HttpHeaderDict[key]] = value;
            return this;
        }

        public H9eHeaderManage AddHeader(string key, string value) {
            foreach (string old in Keys) {
                if (old.Equals(key, StringComparison.CurrentCultureIgnoreCase)) {
                    key = old;
                    break;
                }
            }
            this[key] = value;
            return this;
        }

        public string ToHeaderString(H9eHeaderManage header) {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            foreach (var item in this) {
                string key = item.Key;
                bool add = true;
                foreach (string old in headers.Keys) {
                    if (old.Equals(key, StringComparison.CurrentCultureIgnoreCase)) {
                        add = false;
                        break;
                    }
                }
                if (add) {
                    headers[key] = item.Value;
                }
            }

            if (header != null) {
                foreach (var item in header) {
                    string key = item.Key;
                    foreach (string old in headers.Keys) {
                        if (old.Equals(key, StringComparison.CurrentCultureIgnoreCase)) {
                            key = old;
                            break;
                        }
                    }
                    headers[key] = item.Value;
                }
            }

            return string.Join("\r\n", headers.Select(kv => $"{kv.Key}: {kv.Value}"));
        }

        public H9eHeaderManage() { }

    }
}
