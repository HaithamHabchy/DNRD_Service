using DNRD_Service.Models;
using DNRD_Service.Services;
using DNRD_Service.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace DNRD_Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApprovalController : ControllerBase
    {
        private readonly ILogger<ApprovalController> _logger;
        private readonly IApprovalService _approvalService;

        public ApprovalController(ILogger<ApprovalController> logger,IApprovalService approvalService)
        {
            _logger = logger;
            _approvalService = approvalService;
        }

        [HttpPost("")]
        public IActionResult ProcessActions([FromBody] ApprovalRequestModel iReq)
        {
            var responseModel = new ApprovalResponseModel();
            var token = Request.Headers["Authorization"];
            try
            {
                responseModel = _approvalService.ProcessApprovalActions(iReq, token);
                return Ok(responseModel);
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case HANDLED_EXCEPTION:
                        responseModel = (ApprovalResponseModel)ex.Data["exception"];
                        _logger.LogError("HANDLED_EXCEPTION:" + responseModel.ErrorMessage);
                        return Ok(responseModel);
                    case BAD_REQUEST_EXCEPTION:
                        responseModel = (ApprovalResponseModel)ex.Data["exception"];
                        _logger.LogError("BAD_REQUEST_EXCEPTION:" + responseModel.ErrorMessage);
                        return BadRequest(responseModel);
                    case UNAUTHORIZED_EXCEPTION:
                        responseModel = (ApprovalResponseModel)ex.Data["exception"];
                        if(responseModel != null)
                        {
                            _logger.LogError("UNAUTHORIZED_EXCEPTION:" + responseModel.ErrorMessage);
                        }
                        return Unauthorized(responseModel);
                    default:
                        _logger.LogError("EXCEPTION:" + responseModel.ErrorMessage);
                        return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                }
            }
        }
    }
}
