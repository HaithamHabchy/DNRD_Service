using DNRD_Service.Utilities;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace DNRD_Service.Models
{
    public class ApprovalRequestModel
    {
        [SwaggerIgnore]
        public string Token { get; set; }
        public string EventId { get; set; }
        public string TransactionKey { get; set; }
        public long? Action { get; set; }
        public string ActionName { get; set; }
        [JsonProperty("Comments")]
        public string Comments { get; set; }
        public List<Contact> Contacts { get; set; }
    }

    public class Contact
    {
        public string ContactRecordId { get; set; }
        public long? Action { get; set; }
        public string ActionName { get; set; }
        [JsonProperty("Comments")]
        public string Comments { get; set; }
    }
}
