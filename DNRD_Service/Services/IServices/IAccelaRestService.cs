using DNRD_Service.Dto;

namespace DNRD_Service.Services.IServices
{
    public interface IAccelaRestService
    {
        AccelaRestDto.EmseResultObject<T> CallAdapterScript<T>(object request,string token);
    }
}