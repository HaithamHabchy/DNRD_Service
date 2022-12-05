using System.Collections.Generic;
using System.Net;

namespace DNRD_Service.Services.IServices
{
    public interface IWebRequestHandler
    {
        WebResponse GetResponseWithoutException(WebRequest request);
        string Send(string url, string body, string method, Dictionary<string, string> headers = null, string contentType = null);
        string SendGet(string url, Dictionary<string, string> headers = null);
        string SendPost(string url, string body, Dictionary<string, string> headers = null);
    }
}