using System.Collections.Generic;

namespace DNRD_Service.Dto
{
    public class AccelaRestDto
    {
        public class RestRequestStandardObject
        {
            public RestRequestStandardObject()
            {
                header = new Dictionary<string, string>();
                body = new Dictionary<string, object>();
            }
            public Dictionary<string, string> header { get; set; }
            public Dictionary<string, object> body { get; set; }
        }



        // Data Objects used for response from REST
        public class RestResponseObject<T>
        {
            public int status { get; set; }
            public string code { get; set; }
            public string message { get; set; }
            public string traceId { get; set; }
            public T result { get; set; }
        }


        // Data Objects used for response from REST calling EMSE
        public class EmseResultObject<T>
        {
            public bool success { get; set; }
            public string message { get; set; }
            public string traceId { get; set; }
            public T result { get; set; }
        }

        public class TransactionLanguageCodeResponse
        {
            public string lang { get; set; }
        }

        public class TransactionsRecordsResponse
        {
            public string data { get; set; }
        }
    }
}

