using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace DNRD_Service.Models
{
    public class AuthenticationRequestModel
    {
        [JsonProperty("agency"),Required]
        public string agency { get; set; }

        [JsonProperty("userId"), Required]
        public string userId { get; set; }

        [JsonProperty("password"), Required]
        public string password { get; set; }
    }
}
