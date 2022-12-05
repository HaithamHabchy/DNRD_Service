using DNRD_Service.Models;
using DNRD_Service.Services.IServices;
using DNRD_Service.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DNRD_Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(IAuthenticationService authenticationService,
                                        ILogger<AuthenticationController> logger)
        {
            _logger = logger;
            _authenticationService = authenticationService;
        }

        [HttpPost("get-token")]
        [ProducesResponseType(typeof(AuthenticationResponseModel),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(AuthenticationResponseModel),StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetToken(AuthenticationRequestModel authenticationModel)
        {
            var response = new AuthenticationResponseModel();
            try
            {
                response = await _authenticationService.GetToken(authenticationModel);
                return Ok(response);
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case BAD_REQUEST_EXCEPTION:
                        response.error = ex.Data["user_message"].ToString();
                        _logger.LogError("BAD_REQUEST_EXCEPTION:" + response.error);
                        return BadRequest(response);
                    case UNAUTHORIZED_EXCEPTION:
                        response.error = ex.Data["user_message"].ToString();
                        _logger.LogError("UNAUTHORIZED_EXCEPTION:" + response.error);
                        return Unauthorized(response);
                    default:
                        _logger.LogError("EXCEPTION:" + response.error);
                        return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                }
            }
        }
    }
}