using DNRD_Service.Models;
using System.Threading.Tasks;

namespace DNRD_Service.Services.IServices
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResponseModel> GetToken(AuthenticationRequestModel model);
    }
}