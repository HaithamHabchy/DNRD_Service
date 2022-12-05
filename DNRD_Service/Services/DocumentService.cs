using DNRD_Service.Models;
using DNRD_Service.Services.IServices;
using DNRD_Service.Utilities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DNRD_Service.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IConfiguration _configuration;
        private readonly IAccelaRestService _accelaRestService;
        private AdapterConfigModel _adapterConfigModel;
        public DocumentService(IConfiguration configuration, IAccelaRestService accelaRestService)
        {
            _configuration = configuration;
            _accelaRestService = accelaRestService;
            _adapterConfigModel = new AdapterConfigModel();
            configuration.GetSection("AdapterConfig").Bind(_adapterConfigModel);
        }
        public async Task<DocumentResponseModel> GetDocumentById(long documentId, string token)
        {
            if (token != null)
            {
                token = token.Replace("Bearer ", "");

            }

            //Validate token
            var result = validateToken(token);
            if (result != null)
            {
                var responseModel = new DocumentResponseModel();
                responseModel.errorCode = Convert.ToInt32(result.Split("|")[0]);
                responseModel.errorMessage = result.Split("|")[1];
                var exception = new UNAUTHORIZED_EXCEPTION();
                exception.Data.Add("exception", responseModel);
                throw exception;
            }

            var response = new DocumentResponseModel();
            token = token.Replace("Bearer ", "");
            Uri u = new Uri(Path.Combine(_adapterConfigModel.BizUrl + "/apis/v4/documents/" + documentId + "/download?token=" + token));
            var httpResponse = await Task.Run(() => GetURI(u));
            if (httpResponse != null)
            {
                if(httpResponse.StatusCode == System.Net.HttpStatusCode.Forbidden){
                    var exception = new UNAUTHORIZED_EXCEPTION();
                    throw exception;
                }

                if (httpResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Stream stream = httpResponse.Content.ReadAsStreamAsync().Result;
                    byte[] bytes;
                    using (var memoryStream = new MemoryStream())
                    {
                        stream.CopyTo(memoryStream);
                        bytes = memoryStream.ToArray();
                    }
                    string base64 = Convert.ToBase64String(bytes);
                    response.result = base64;
                }
                else if (httpResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    var responseModel = new DocumentResponseModel();
                    responseModel.errorCode = 59;
                    responseModel.errorMessage = "Document not found";
                    var exception = new NO_DATA_FOUND_EXECEPTION();
                    exception.Data.Add("exception", responseModel);
                    throw exception;
                }
                else
                {
                    throw new Exception();
                }
            }
            else
            {
                throw new Exception();
            }

            return response;
        }

        private async Task<HttpResponseMessage> GetURI(Uri u)
        {
            HttpResponseMessage result = new HttpResponseMessage();
            using (var client = new HttpClient())
            {
                result = await client.GetAsync(u);
            }
            return result;
        }

        private string validateToken(string token)
        {
            var request = new AccelaRestRequestModel();
            request.header.action = "validateAccessToken";
            request.header.lang = "en_US";
            request.body = JsonConvert.SerializeObject(new { Token = token });
            var response = _accelaRestService.CallAdapterScript<dynamic>(request,token);
            return response.result;
        }
    }
}
