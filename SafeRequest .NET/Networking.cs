﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SafeRequest.NET {

    public class Networking {

        private const string USER_AGENT = "SafeRequest.NET";

        public static Response Request(string url, RequestType type, NameValueCollection values = null) {
            Response response = new Response();
            try {
                WebClient web = GetClient();
                string raw = string.Empty;
                switch (type) {
                    case RequestType.GET:
                        raw = web.DownloadString(url);
                        break;
                    case RequestType.POST:
                        if (values == null)
                            throw new Exception("Missing POST values.");
                        byte[] rawBytes = web.UploadValues(url, values);
                        raw = Encoding.Default.GetString(rawBytes);
                        break;
                }
                response.Initialize(raw);
            } catch (Exception ex) {
                response.status = false;
                response.message = ex.Message;
            }
            return response;
        }

        public static WebClient GetClient(string userAgent = USER_AGENT) {
            WebClient web = new WebClient();
            web.Headers.Add(HttpRequestHeader.UserAgent, userAgent);
            web.Proxy = null;
            return web;
        }

    }

    public struct Response {
        public bool status;
        public string message;
        public string raw;
        private Dictionary<string, object> data;
        public T GetData<T>(string key) { return (T)data[key]; }
        public bool DataExists(string key) { return data.ContainsKey(key); }
        public void Initialize(string _raw) {
            raw = _raw;
            data = JsonConvert.DeserializeObject<Dictionary<string, object>>(raw);
            if (DataExists("status") && DataExists("message")) {
                status = GetData<bool>("status");
                message = GetData<string>("message");
            }
            else throw new Exception("Response is missing required data.");
        }
    }

    public enum RequestType { NONE, GET, POST }

}
