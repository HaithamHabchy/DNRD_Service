using Microsoft.AspNetCore.Http;
using System.IO;

namespace DNRD_Service.Models
{
    public class DocumentResponseModel
    {
        public string result { get; set; }
        public int? errorCode { get; set; }
        public string errorMessage { get; set; }
    }
}
