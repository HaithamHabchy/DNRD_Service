using DNRD_Service.Models;
using DNRD_Service.Services.IServices;
using DNRD_Service.Utilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DNRD_Service.Services
{
    public class ApprovalService : IApprovalService
    {
        private readonly IAccelaRestService _accelaRestService;
        private readonly ILogger<ApprovalService> _logger;

        public ApprovalService(IAccelaRestService accelaRestService,
                                 ILogger<ApprovalService> logger)
        {
            _accelaRestService = accelaRestService;
            _logger = logger;
        }

        public ApprovalResponseModel ProcessApprovalActions(ApprovalRequestModel approvalRequestModel, String token)
        {
            _logger.LogInformation("ProcessApprovalActions start");
            string startExecutionTime;
            string endExecutionTime;

            startExecutionTime = DateTime.UtcNow.ToString();

            var responseModel = new ApprovalResponseModel();
            var contactResponseModel = new List<ContactModel>();

            if(token != null)
            {
                token = token.Replace("Bearer ", "");

            }

            _logger.LogInformation("Checking token...");
            if (String.IsNullOrWhiteSpace(token))
            {
                responseModel.ErrorCode = 50;
                responseModel.ErrorMessage = "Token required";
                responseModel.TransactionKey = approvalRequestModel.TransactionKey;
                responseModel.EventId = null;
                var exception = new UNAUTHORIZED_EXCEPTION();
                exception.Data.Add("exception", responseModel);
                endExecutionTime = DateTime.UtcNow.ToString();
                insertDNRDLogs(approvalRequestModel.TransactionKey, approvalRequestModel.EventId, JsonConvert.SerializeObject(approvalRequestModel), "Failure", String.Concat(responseModel.ErrorCode, "-", responseModel.ErrorMessage), startExecutionTime, endExecutionTime,token);
                throw exception;
            }

            _logger.LogInformation("Getting actions list...");
            var actionsList = getActionsList(token).ToDictionary(x => x.id, x => x.nameEn);

            //Validate token
            _logger.LogInformation("Validating token...");
            var tokenResult = validateToken(token);
            if (tokenResult != null)
            {
                responseModel.ErrorCode = Convert.ToInt64(tokenResult.Split("|")[0]);
                responseModel.ErrorMessage = tokenResult.Split("|")[1];
                var exception = new UNAUTHORIZED_EXCEPTION();
                exception.Data.Add("exception", responseModel);
                endExecutionTime = DateTime.UtcNow.ToString();
                insertDNRDLogs(approvalRequestModel.TransactionKey, approvalRequestModel.EventId, JsonConvert.SerializeObject(approvalRequestModel), "Failure", String.Concat(responseModel.ErrorCode, "-", responseModel.ErrorMessage), startExecutionTime, endExecutionTime,token);
                throw exception;
            }

            _logger.LogInformation("Verifying the action...");
            if (approvalRequestModel.Action != null)
            {
                try
                {
                    approvalRequestModel.ActionName = actionsList[approvalRequestModel.Action.Value];
                }
                catch (Exception)
                {
                    responseModel.ErrorCode = 14;
                    responseModel.ErrorMessage = "Invalid action " + approvalRequestModel.Action.Value;
                    responseModel.TransactionKey = approvalRequestModel.TransactionKey;
                    responseModel.EventId = null;
                    var exception = new BAD_REQUEST_EXCEPTION();
                    exception.Data.Add("exception", responseModel);
                    endExecutionTime = DateTime.UtcNow.ToString();
                    insertDNRDLogs(approvalRequestModel.TransactionKey, approvalRequestModel.EventId, JsonConvert.SerializeObject(approvalRequestModel), "Failure", String.Concat(responseModel.ErrorCode, "-", responseModel.ErrorMessage), startExecutionTime, endExecutionTime,token);
                    throw exception;

                }
            }

            _logger.LogInformation("Checking comments...");
            if (approvalRequestModel.Comments == null)
            {
                approvalRequestModel.Comments = String.Empty;
            }

            _logger.LogInformation("Checking transaction key...");
            if (String.IsNullOrWhiteSpace(approvalRequestModel.TransactionKey))
            {
                responseModel.ErrorCode = 60;
                responseModel.ErrorMessage = "TransactionKey is required";
                responseModel.TransactionKey = approvalRequestModel.TransactionKey;
                responseModel.EventId = null;
                var exception = new BAD_REQUEST_EXCEPTION();
                exception.Data.Add("exception", responseModel);
                endExecutionTime = DateTime.UtcNow.ToString();
                insertDNRDLogs(approvalRequestModel.TransactionKey, approvalRequestModel.EventId, JsonConvert.SerializeObject(approvalRequestModel), "Failure", String.Concat(responseModel.ErrorCode, "-", responseModel.ErrorMessage), startExecutionTime, endExecutionTime,token);
                throw exception;
            }

            _logger.LogInformation("Checking eventId...");
            if (String.IsNullOrWhiteSpace(approvalRequestModel.EventId))
            {
                responseModel.ErrorCode = 53;
                responseModel.ErrorMessage = "EventId is required";
                responseModel.TransactionKey = approvalRequestModel.TransactionKey;
                responseModel.EventId = null;
                var exception = new BAD_REQUEST_EXCEPTION();
                exception.Data.Add("exception", responseModel);
                endExecutionTime = DateTime.UtcNow.ToString();
                insertDNRDLogs(approvalRequestModel.TransactionKey, approvalRequestModel.EventId, JsonConvert.SerializeObject(approvalRequestModel), "Failure", String.Concat(responseModel.ErrorCode, "-", responseModel.ErrorMessage), startExecutionTime, endExecutionTime,token);
                throw exception;
            }

            _logger.LogInformation("Checking contacts...");
            if (approvalRequestModel.Contacts != null)
            {
                if (approvalRequestModel.Contacts.Count > 0)
                {
                    foreach (var c in approvalRequestModel.Contacts)
                    {
                        _logger.LogInformation("Checking Contact comments...");
                        if (c.Comments == null)
                        {
                            c.Comments = String.Empty;
                        }

                        _logger.LogInformation("Checking ContactRecordId...");
                        if (String.IsNullOrEmpty(c.ContactRecordId))
                        {
                            responseModel.ErrorCode = 55;
                            responseModel.ErrorMessage = "ContactRecordId is required for contacts";
                            responseModel.TransactionKey = approvalRequestModel.TransactionKey;
                            responseModel.EventId = null;
                            var exception = new BAD_REQUEST_EXCEPTION();
                            exception.Data.Add("exception", responseModel);
                            endExecutionTime = DateTime.UtcNow.ToString();
                            insertDNRDLogs(approvalRequestModel.TransactionKey, approvalRequestModel.EventId, JsonConvert.SerializeObject(approvalRequestModel), "Failure", String.Concat(responseModel.ErrorCode, "-", responseModel.ErrorMessage), startExecutionTime, endExecutionTime,token);
                            throw exception;
                        }

                        _logger.LogInformation("Checking Contact action...");
                        if (c.Action == null)
                        {
                            responseModel.ErrorCode = 54;
                            responseModel.ErrorMessage = "Action is required for contacts";
                            responseModel.TransactionKey = approvalRequestModel.TransactionKey;
                            responseModel.EventId = null;
                            var exception = new BAD_REQUEST_EXCEPTION();
                            exception.Data.Add("exception", responseModel);
                            endExecutionTime = DateTime.UtcNow.ToString();
                            insertDNRDLogs(approvalRequestModel.TransactionKey, approvalRequestModel.EventId, JsonConvert.SerializeObject(approvalRequestModel), "Failure", String.Concat(responseModel.ErrorCode, "-", responseModel.ErrorMessage), startExecutionTime, endExecutionTime, token);
                            throw exception;
                        }
                        else
                        {
                            try
                            {
                                c.ActionName = actionsList[c.Action.Value];
                            }
                            catch (Exception)
                            {
                                responseModel.ErrorCode = 14;
                                responseModel.ErrorMessage = "Invalid action " + c.Action.Value;
                                responseModel.TransactionKey = approvalRequestModel.TransactionKey;
                                responseModel.EventId = null;
                                var exception = new BAD_REQUEST_EXCEPTION();
                                exception.Data.Add("exception", responseModel);
                                endExecutionTime = DateTime.UtcNow.ToString();
                                insertDNRDLogs(approvalRequestModel.TransactionKey, approvalRequestModel.EventId, JsonConvert.SerializeObject(approvalRequestModel), "Failure", String.Concat(responseModel.ErrorCode, "-", responseModel.ErrorMessage), startExecutionTime, endExecutionTime, token);
                                throw exception;
                            }
                        }
                    }
                }
            }
            else
            {
                _logger.LogInformation("Checking event action...");
                if (String.IsNullOrEmpty(approvalRequestModel.ActionName))
                {
                    responseModel = new ApprovalResponseModel();
                    responseModel.ErrorCode = 57;
                    responseModel.ErrorMessage = "Action is required for event";
                    responseModel.TransactionKey = approvalRequestModel.TransactionKey;
                    responseModel.EventId = null;
                    var exception = new BAD_REQUEST_EXCEPTION();
                    exception.Data.Add("exception", responseModel);
                    endExecutionTime = DateTime.UtcNow.ToString();
                    insertDNRDLogs(approvalRequestModel.TransactionKey, approvalRequestModel.EventId, JsonConvert.SerializeObject(approvalRequestModel), "Failure", String.Concat(responseModel.ErrorCode, "-", responseModel.ErrorMessage), startExecutionTime, endExecutionTime, token);
                    throw exception;
                }

                _logger.LogInformation("Checking event Comments...");
                if (String.IsNullOrEmpty(approvalRequestModel.Comments))
                {
                    responseModel.ErrorCode = 58;
                    responseModel.ErrorMessage = "Comments are required for event";
                    responseModel.TransactionKey = approvalRequestModel.TransactionKey;
                    responseModel.EventId = null;
                    var exception = new BAD_REQUEST_EXCEPTION();
                    exception.Data.Add("exception", responseModel);
                    endExecutionTime = DateTime.UtcNow.ToString();
                    insertDNRDLogs(approvalRequestModel.TransactionKey, approvalRequestModel.EventId, JsonConvert.SerializeObject(approvalRequestModel), "Failure", String.Concat(responseModel.ErrorCode, "-", responseModel.ErrorMessage), startExecutionTime, endExecutionTime, token);
                    throw exception;
                }
            }

            var request = new AccelaRestRequestModel();
            request.header.action = "executeContactAction";
            request.header.lang = "en_US";
            approvalRequestModel.Token = token;
            request.body = JsonConvert.SerializeObject(approvalRequestModel);
            _logger.LogInformation("Calling executeContactAction...");
            var response = _accelaRestService.CallAdapterScript<dynamic>(request,token);
            responseModel = JsonConvert.DeserializeObject<ApprovalResponseModel>(response.result);
            _logger.LogInformation("Accela Script for executeContactAction:" + response.message);

            if (!response.success)
            {
                if (responseModel.ErrorMessage.IndexOf("401**") > -1)
                {
                    responseModel.ErrorCode = Convert.ToInt64(responseModel.ErrorMessage.Split("**")[1].Split("|")[0]);
                    responseModel.ErrorMessage = responseModel.ErrorMessage.Split("**")[1].Split("|")[1];
                    responseModel.TransactionKey = approvalRequestModel.TransactionKey;
                    responseModel.EventId = null;
                    var exception = new UNAUTHORIZED_EXCEPTION();
                    exception.Data.Add("exception", responseModel);
                    endExecutionTime = DateTime.UtcNow.ToString();
                    insertDNRDLogs(approvalRequestModel.TransactionKey, approvalRequestModel.EventId, JsonConvert.SerializeObject(approvalRequestModel), "Failure", String.Concat(responseModel.ErrorCode, "-", responseModel.ErrorMessage), startExecutionTime, endExecutionTime, token);
                    throw exception;
                }
                throw new Exception($"{response.message}");
            }
            else
            {
                if (responseModel.Contacts.Count > 0)
                {
                    foreach (var c in responseModel.Contacts)
                    {
                        if (c.ErrorMessage != "")
                        {
                            responseModel.ErrorCode = Convert.ToInt64(c.ErrorMessage.Split("|")[0]);
                            responseModel.ErrorMessage = c.ErrorMessage.Split("|")[1];
                            responseModel.TransactionKey = approvalRequestModel.TransactionKey;
                            var exception = new HANDLED_EXCEPTION();
                            exception.Data.Add("exception", responseModel);
                            endExecutionTime = DateTime.UtcNow.ToString();
                            insertDNRDLogs(approvalRequestModel.TransactionKey, approvalRequestModel.EventId, JsonConvert.SerializeObject(approvalRequestModel), "Failure", String.Concat(responseModel.ErrorCode, "-", responseModel.ErrorMessage), startExecutionTime, endExecutionTime, token);
                            throw exception;
                        }
                    }
                }

                if (responseModel.isAllContactsRejected && String.IsNullOrEmpty(approvalRequestModel.ActionName))
                {
                    approvalRequestModel.ActionName = "Reject";
                    approvalRequestModel.Comments = "Rejecting all contacts";
                }

                if (approvalRequestModel.ActionName != null)
                {
                    if (approvalRequestModel.ActionName.Trim() != "")
                    {
                        request = new AccelaRestRequestModel();
                        request.header.action = "executeEventAction";
                        request.body = JsonConvert.SerializeObject(approvalRequestModel);
                        _logger.LogInformation("Calling executeEventAction...");
                        response = _accelaRestService.CallAdapterScript<dynamic>(request,token);
                        responseModel.ErrorCode = response.result == null ? null : Convert.ToInt64(response.result.Split("|")[0]);
                        responseModel.ErrorMessage = response.result == null ? null : response.result.Split("|")[1];
                        responseModel.TransactionKey = approvalRequestModel.TransactionKey;
                        _logger.LogInformation("Accela Script for executeEventAction:" + response.message);
                        if (!response.success)
                        {
                            endExecutionTime = DateTime.UtcNow.ToString();
                            insertDNRDLogs(approvalRequestModel.TransactionKey, approvalRequestModel.EventId, JsonConvert.SerializeObject(approvalRequestModel), "Failure", String.Concat(responseModel.ErrorCode, "-", responseModel.ErrorMessage), startExecutionTime, endExecutionTime,token);
                            throw new Exception($"{response.message}");
                        }
                        if (responseModel.ErrorCode != null)
                        {
                            var exception = new HANDLED_EXCEPTION();
                            exception.Data.Add("exception", responseModel);
                            endExecutionTime = DateTime.UtcNow.ToString();
                            insertDNRDLogs(approvalRequestModel.TransactionKey, approvalRequestModel.EventId, JsonConvert.SerializeObject(approvalRequestModel), "Failure", String.Concat(responseModel.ErrorCode, "-", responseModel.ErrorMessage), startExecutionTime, endExecutionTime,token);
                            throw exception;
                        }
                    }
                }
            }
            responseModel.TransactionKey = approvalRequestModel.TransactionKey;
            responseModel.ErrorCode = null;
            responseModel.ErrorMessage = null;
            endExecutionTime = DateTime.UtcNow.ToString();
            insertDNRDLogs(approvalRequestModel.TransactionKey, approvalRequestModel.EventId, JsonConvert.SerializeObject(approvalRequestModel), "Success", String.Empty, startExecutionTime, endExecutionTime, token);
            return responseModel;
        }

        private void insertDNRDLogs(string trxId, string recordId, string jsonData, string trxStatus, string trxError, string startExecutionTime, string endExecutionTime,string token)
        {
            try
            {
                var request = new AccelaRestRequestModel();
                request.header.action = "insertDNRDLogs";
                request.body = JsonConvert.SerializeObject(new
                {
                    trxId = trxId,
                    recordId = recordId,
                    jsonData = jsonData,
                    trxStatus = trxStatus,
                    trxError = trxError,
                    startExecutionTime = startExecutionTime,
                    endExecutionTime = endExecutionTime
                });
                var response = _accelaRestService.CallAdapterScript<dynamic>(request, token);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error inserting logs to databse : " + ex.Message);
            }
        }
        private List<ActionModel> getActionsList(string token)
        {
            var request = new AccelaRestRequestModel();
            request.header.action = "getActionsList";
            request.body = JsonConvert.SerializeObject(new { });
            var response = _accelaRestService.CallAdapterScript<dynamic>(request, token);
            return response.result.ToObject<List<ActionModel>>();
        }

        private string validateToken(string token)
        {
            var request = new AccelaRestRequestModel();
            request.header.action = "validateAccessToken";
            request.header.lang = "en_US";
            request.body = JsonConvert.SerializeObject(new { Token = token });
            var response = _accelaRestService.CallAdapterScript<dynamic>(request,token);
            return response.result;
        }
    }
}

