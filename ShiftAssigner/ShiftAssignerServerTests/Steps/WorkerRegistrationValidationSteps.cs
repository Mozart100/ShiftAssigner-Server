using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Reqnroll;
using ShiftAssignerServer.Requests;
using ShiftAssignerServer.Tests.Common;
using ShiftAssignerServer.Tests.Infrastructure;
using Xunit;

namespace ShiftAssignerServer.Tests.Steps;

[Binding]
public class WorkerRegistrationValidationSteps : SingleTenantStep
{
    // Scenario context keys
    private const string WorkerRegistrationResponse_Context = "WorkerRegistrationResponse";
    private const string RegistrationException_Context = "RegistrationException";
    private const string HttpResponse_Context = "HttpResponse";
    private const string ErrorResponseBody_Context = "ErrorResponseBody";

    private RegisterRequest _currentRequest;
    private RegisterResponse _successResponse;
    private HttpResponseMessage _httpResponse;
    private string _errorResponseBody;

    public WorkerRegistrationValidationSteps(ScenarioContext scenarioContext) : base(scenarioContext)
    {
    }

    [When("I register a worker with valid data")]
    public async Task WhenIRegisterAWorkerWithValidData(Table table)
    {
        _currentRequest = BuildRegisterRequestFromTable(table);
        
        const string registrationPath = "api/v1/Auth/register-worker";
        
        try
        {
            _successResponse = await _serverSender.PostCommandAsync<RegisterRequest, RegisterResponse>(_currentRequest, registrationPath);
            _scenarioContext[WorkerRegistrationResponse_Context] = _successResponse;
        }
        catch (Exception ex)
        {
            // Store exception for validation
            _scenarioContext[RegistrationException_Context] = ex;
        }
    }

    [When("I register a worker with invalid data")]
    public async Task WhenIRegisterAWorkerWithInvalidData(Table table)
    {
        _currentRequest = BuildRegisterRequestFromTable(table);
        
        const string registrationPath = "api/v1/Auth/register-worker";
        
        try
        {
            // Try to send the request - expecting it to fail
            var client = _serverSender.GetHttpClient();
            var content = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(_currentRequest),
                System.Text.Encoding.UTF8,
                "application/json");
            
            _httpResponse = await client.PostAsync(registrationPath, content);
            _errorResponseBody = await _httpResponse.Content.ReadAsStringAsync();
            
            // Store for assertions
            _scenarioContext[HttpResponse_Context] = _httpResponse;
            _scenarioContext[ErrorResponseBody_Context] = _errorResponseBody;
        }
        catch (Exception ex)
        {
            _scenarioContext[RegistrationException_Context] = ex;
        }
    }

    [Then("the worker registration should succeed")]
    public void ThenTheWorkerRegistrationShouldSucceed()
    {
        if (_scenarioContext.ContainsKey(RegistrationException_Context))
        {
            var ex = _scenarioContext[RegistrationException_Context] as Exception;
            Assert.Fail($"Worker registration should have succeeded but threw exception: {ex?.Message}");
        }
        
        Assert.NotNull(_successResponse);
    }

    [Then("the worker registration should fail with validation errors")]
    public void ThenTheWorkerRegistrationShouldFailWithValidationErrors()
    {
        var httpResponse = _scenarioContext[HttpResponse_Context] as HttpResponseMessage;
        var errorBody = _scenarioContext[ErrorResponseBody_Context] as string;
        
        Assert.NotNull(httpResponse);
        Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
        Assert.False(string.IsNullOrWhiteSpace(errorBody));
        
        // Verify it contains validation error structure
        Assert.Contains("validation", errorBody.ToLower());
    }

    [Then("the validation error should contain {string}")]
    public void ThenTheValidationErrorShouldContain(string expectedError)
    {
        var errorBody = _scenarioContext[ErrorResponseBody_Context] as string;
        
        Assert.NotNull(errorBody);
        Assert.Contains(expectedError, errorBody, StringComparison.OrdinalIgnoreCase);
    }

    [Then("the validation should contain multiple errors")]
    public void ThenTheValidationShouldContainMultipleErrors()
    {
        var errorBody = _scenarioContext[ErrorResponseBody_Context] as string;
        
        Assert.NotNull(errorBody);
        
        // Parse JSON to count errors
        var doc = System.Text.Json.JsonDocument.Parse(errorBody);
        var root = doc.RootElement;
        
        if (root.TryGetProperty("errors", out var errorsElement))
        {
            var errorCount = errorsElement.GetArrayLength();
            Assert.True(errorCount > 1, $"Expected multiple errors but found only {errorCount}");
        }
        else
        {
            Assert.Fail("Response does not contain 'errors' array");
        }
    }

    private RegisterRequest BuildRegisterRequestFromTable(Table table)
    {
        var request = new RegisterRequest();
        
        foreach (var row in table.Rows)
        {
            var field = row["Field"];
            var value = row["Value"];
            
            switch (field)
            {
                case "ID":
                    request.ID = value;
                    break;
                case "FirstName":
                    request.FirstName = value;
                    break;
                case "LastName":
                    request.LastName = value;
                    break;
                case "PhoneNumber":
                    request.PhoneNumber = value;
                    break;
                case "DateOfBirth":
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        request.DateOfBirth = DateOnly.Parse(value);
                    }
                    break;
                case "PasswordHash":
                    request.PasswordHash = value;
                    break;
                case "ShiftLeaderId":
                    request.ShiftLeaderId = value;
                    break;
            }
        }
        
        return request;
    }
}
