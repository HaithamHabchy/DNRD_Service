using DNRD_Service.Models;
using DNRD_Service.Services.IServices;
using DNRD_Service.Utilities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DNRD_Service.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IConfiguration _configuration;
        private AdapterConfigModel _adapterConfigModel;
        public AuthenticationService(IConfiguration configuration)
        {
            _configuration = configuration;
            _adapterConfigModel = new AdapterConfigModel();
            configuration.GetSection("AdapterConfig").Bind(_adapterConfigModel);
        }
        public async Task<AuthenticationResponseModel> GetToken(AuthenticationRequestModel model)
        {
            var response = new AuthenticationResponseModel();
            var errorMessage = ValidateJsonAgainstSchema(model);
            if (errorMessage != String.Empty)
            {
                var exception = new BAD_REQUEST_EXCEPTION();
                exception.Data.Add("user_message", errorMessage);
                throw exception;
            }

            if(model.agency.ToLower() != "dtcm")
            {
                var exception = new UNAUTHORIZED_EXCEPTION();
                exception.Data.Add("user_message", "Invalid Credentials");
                throw exception;
            }

            Uri u = new Uri(Path.Combine(_adapterConfigModel.BizUrl + "/apis/agency/auth"));
            var payload = JsonConvert.SerializeObject(model);
            HttpContent c = new StringContent(payload, Encoding.UTF8, "application/json");
            var httpResponse = await Task.Run(() => PostURI(u, c));
            if (httpResponse != null)
            {
                if (httpResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    var exception = new UNAUTHORIZED_EXCEPTION();
                    exception.Data.Add("user_message", "Invalid Credentials");
                    throw exception;
                }
                else if (httpResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var bodyString = await httpResponse.Content.ReadAsStringAsync();
                    response = JsonConvert.DeserializeObject<AuthenticationResponseModel>(bodyString);
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

        private async Task<HttpResponseMessage> PostURI(Uri u, HttpContent c)
        {
            HttpResponseMessage result = new HttpResponseMessage();
            using (var client = new HttpClient())
            {
                result = await client.PostAsync(u, c);
            }
            return result;
        }

        private string ValidateJsonAgainstSchema(AuthenticationRequestModel model)
        {
            var errorMessage = String.Empty;
            string schemaJson = @"{
              'type': 'object',
                 'properties': {
              'agency': {'type':'string'},
              'userId': {'type':'string'},
              'password': {'type':'string'}
            },
             'required': ['agency', 'userId','password']
                }";

            JSchema schema = JSchema.Parse(schemaJson);
            JObject person = JObject.FromObject(model);
            person.IsValid(schema, out IList<string> messages);

            foreach (var item in messages)
            {
                errorMessage += item + "/";
            }
            if (errorMessage != String.Empty)
            {
                errorMessage = errorMessage.Remove(errorMessage.Length - 1);
                var exception = new BAD_REQUEST_EXCEPTION();
                exception.Data.Add("user_message", errorMessage);
                throw exception;
            }
            return String.Empty;
        }
    }
}
