using DNRD_Service.Services.IServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace DNRD_Service.Services
{
    public class WebRequestHandler : IWebRequestHandler
    {
        public string SendPost(string url, string body, Dictionary<string, string> headers = null)
        {
            if (headers != null)
                return Send(url, body, "POST", headers);
            else
                return Send(url, body, "POST");
        }

        public string SendGet(string url, Dictionary<string, string> headers = null)
        {
            if (headers != null)
                return Send(url, "", "GET", headers);
            else
                return Send(url, "", "GET");
        }

        public string Send(string url, string body, string method, Dictionary<string, string> headers = null, string contentType = null)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentException("Request URL is required");
            }
            if (string.IsNullOrWhiteSpace(method))
            {
                throw new ArgumentException("Request Method is required");
            }
            var data = new byte[0];
            if (method.ToUpper().Equals("POST") || method.ToUpper().Equals("PUT"))
            {
                data = Encoding.UTF8.GetBytes(body);
                if (data.Length == 0)
                {
                    throw new ArgumentException("Request Body is required");
                }
            }
            string responseString;
            try
            {
                var requestUri = new Uri(url);
                var request = (HttpWebRequest)WebRequest.Create(requestUri);
                request.Method = method;
                request.ContentType = "application/json";
                if (!string.IsNullOrWhiteSpace(contentType))
                    request.ContentType = contentType;
                request.ContentLength = data.Length;
                request.AllowAutoRedirect = false;
                request.Timeout = 300000;

                // Headers
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        request.Headers[header.Key] = header.Value;
                    }
                }

                // Ignore Certificate Validation
                ServicePointManager.ServerCertificateValidationCallback = (obj, certificate, chain, errors) => true;
                if (method.ToUpper().Equals("POST") || method.ToUpper().Equals("PUT"))
                {
                    request.ContentLength = data.Length;
                    using (var stream = request.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                }
                var response = (HttpWebResponse)GetResponseWithoutException(request);
                using (var stream = response.GetResponseStream())
                {
                    if (stream == null)
                    {
                        throw new Exception("Unknown error has occured", new Exception($"{method} request to {url} with data {body} didn't return a response"));
                    }
                    var reader = new StreamReader(stream, Encoding.UTF8);
                    responseString = reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, new Exception($"An exception occured while trying to send a {method} request to {url} with data {body}. Exception: {e.Message}"));
            }
            return responseString;
        }

        public WebResponse GetResponseWithoutException(WebRequest request)
        {
            try
            {
                return request.GetResponse();
            }
            catch (WebException e)
            {
                if (e.Response == null)
                {
                    throw;
                }
                return e.Response;
            }
        }
    }
}
