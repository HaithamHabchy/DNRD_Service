using DNRD_Service.Models;
using DNRD_Service.Services.IServices;
using DNRD_Service.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DNRD_Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentService;

        public DocumentController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        [HttpGet("get-document-by-id/{id}")]
        [ProducesResponseType(typeof(DocumentResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DocumentResponseModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(DocumentResponseModel), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> getDocumentByIdAsync([FromRoute] long id)
        {
            var responseModel = new DocumentResponseModel();
            try
            {
                var token = Request.Headers["Authorization"];
                if (String.IsNullOrWhiteSpace(token))
                {
                    responseModel.errorCode = 50;
                    responseModel.errorMessage = "Token required";
                    var exception = new UNAUTHORIZED_EXCEPTION();
                    exception.Data.Add("exception", responseModel);
                    throw exception;
                }

                var resp = await _documentService.GetDocumentById(id, token);

                return Ok(resp);
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case UNAUTHORIZED_EXCEPTION:
                        responseModel = (DocumentResponseModel)ex.Data["exception"];
                        return Unauthorized(responseModel);
                    case NO_DATA_FOUND_EXECEPTION:
                        responseModel = (DocumentResponseModel)ex.Data["exception"];
                        return Ok(responseModel);
                    default:
                        return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                }
            }

        }

    }
}
