using DNRD_Service.Models;

namespace DNRD_Service.Services
{
    public interface IApprovalService
    {
        ApprovalResponseModel ProcessApprovalActions(ApprovalRequestModel approvalRequestModel, string token);
    }
}