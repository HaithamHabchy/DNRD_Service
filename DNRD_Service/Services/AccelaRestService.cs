using DNRD_Service.Dto;
using DNRD_Service.Models;
using DNRD_Service.Services.IServices;
using DNRD_Service.Utilities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DNRD_Service.Services
{
    public class AccelaRestService : IAccelaRestService
    {
        private readonly IConfiguration _configuration;
        private readonly IWebRequestHandler _webRequestHandler;
        private readonly IUtility _utility;
        private AdapterConfigModel _adapterConfigModel;

        public AccelaRestService(IConfiguration configuration,
                                 IWebRequestHandler webRequestHandler,
                                 IUtility utility)
        {
            _configuration = configuration;
            _webRequestHandler = webRequestHandler;
            _utility = utility;
            _adapterConfigModel = new AdapterConfigModel();
            configuration.GetSection("AdapterConfig").Bind(_adapterConfigModel);
        }
        private string SendRestRequest(string url, string method, string body)
        {
            var bizServer = _adapterConfigModel.BizUrl;
            url = bizServer + url;

            var responseString = _webRequestHandler.SendPost(url, body);

            if (string.IsNullOrEmpty(responseString))
            {
                throw new Exception("", new Exception($"{method} request to {url} with data {body} didn't return a response"));
            }

            return responseString;
        }

        private AccelaRestDto.EmseResultObject<T> InvokeEmseScript<T>(string scriptName, string body, string token, bool useFreshToken)
        {
            var url = $"/apis/v4/scripts/{scriptName}?token={token}";
            var responseString = SendRestRequest(url, "POST", body);
            var restResponseObject = _utility.DeserializeFromJson<AccelaRestDto.RestResponseObject<AccelaRestDto.EmseResultObject<T>>>(responseString);
            if (restResponseObject.status == 401)
            {
                var exception = new UNAUTHORIZED_EXCEPTION();
                throw exception;
            }

            if (restResponseObject.status != 200)
            {
                var errorMessage = $"Error while invoking EMSE script {scriptName}. Status: {restResponseObject.status}, traceId: {restResponseObject.traceId}, error code: {restResponseObject.code}, message: {restResponseObject.message}";
                throw new Exception(restResponseObject.message, new Exception(errorMessage));
            }

            // If the server returned a proper JSON but without a status we'll consider the request a success
            return restResponseObject.result;
        }

        public AccelaRestDto.EmseResultObject<T> CallAdapterScript<T>(object request,string token)
        {
            var body = JsonConvert.SerializeObject(request);
            return CallAdapterScript<T>(body,token);
        }

        private AccelaRestDto.EmseResultObject<T> CallAdapterScript<T>(string body,string token)
        {
            try
            {
                return InvokeEmseScript<T>(_adapterConfigModel.AccelaEMSEScript, body,token,false);
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
