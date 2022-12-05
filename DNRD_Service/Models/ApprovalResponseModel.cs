using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DNRD_Service.Models
{
    public class ApprovalResponseModel
    {
        public string EventId { get; set; }
        public string TransactionKey { get; set; }
        public long? ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        [JsonIgnore]
        public List<ContactModel> Contacts { get; set; }

        [JsonIgnore]
        public bool isAllContactsRejected { get; set; }
    }
}
