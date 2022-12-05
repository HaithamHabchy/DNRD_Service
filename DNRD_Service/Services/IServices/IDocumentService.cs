using DNRD_Service.Models;
using System.Threading.Tasks;

namespace DNRD_Service.Services.IServices
{
    public interface IDocumentService
    {
        Task<DocumentResponseModel> GetDocumentById(long documentId, string token);
    }
}