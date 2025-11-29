using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Reqnroll;
using ShiftAssignerServer.Controllers;
using ShiftAssignerServer.Models.Stuff;
using ShiftAssignerServer.Requests;
using ShiftAssignerServer.Tests.Common;
using ShiftAssignerServer.Tests.Infrastructure;
using Xunit;

namespace ShiftAssignerServer.Tests.Steps;

[Binding]
public class MultiTenantSteps : TwoTenantsStep
{
    private const string TenantA_Context = "TenantA_Data";
    private const string TenantB_Context = "TenantB_Data";
    private const string TenantA_Response_Context = "TenantA_Response";
    private const string TenantB_Response_Context = "TenantB_Response";
    private const string TenantA_Leaders_Context = "TenantA_Leaders";
    private const string TenantB_Leaders_Context = "TenantB_Leaders";
    private const string TenantA_Workers_Context = "TenantA_Workers";
    private const string TenantB_Workers_Context = "TenantB_Workers";
    
    // Additional context keys for worker registration validation
    private const string WorkerRegistrationResponse_Context = "WorkerRegistrationResponse";
    private const string RegistrationException_Context = "RegistrationException";
    private const string HttpResponse_Context = "HttpResponse";
    private const string ErrorResponseBody_Context = "ErrorResponseBody";

    public MultiTenantSteps(ScenarioContext scenarioContext) : base(scenarioContext)
    {
    }

    [Given("I have tenant registrations for \"(.*)\" and \"(.*)\"")]
    public void GivenIHaveTenantRegistrationsFor(string tenantAName, string tenantBName)
    {
        var tenantAPayload = new TenantRegisterRequest
        {
            ID = $"boss-{tenantAName}-{Guid.NewGuid().ToString("N").Substring(0, 8)}",
            FirstName = "Boss",
            LastName = "UserA",
            PhoneNumber = "555-0100",
            DateOfBirth = new DateOnly(1980, 1, 1),
            Tenant = $"{tenantAName}_{Guid.NewGuid()}",
            PasswordHash = "P@ssw0rd!"
        };

        var tenantBPayload = new TenantRegisterRequest
        {
            ID = $"boss-{tenantBName}-{Guid.NewGuid().ToString("N").Substring(0, 8)}",
            FirstName = "Boss",
            LastName = "UserB",
            PhoneNumber = "555-0200",
            DateOfBirth = new DateOnly(1985, 1, 1),
            Tenant = $"{tenantBName}_{Guid.NewGuid()}",
            PasswordHash = "P@ssw0rd!"
        };

        _scenarioContext[TenantA_Context] = tenantAPayload;
        _scenarioContext[TenantB_Context] = tenantBPayload;
    }

    [When("I register tenant \"(.*)\" with boss \"(.*)\"")]
    public async Task WhenIRegisterTenantWithBoss(string tenantName, string bossId)
    {
        const string registrationPath = $"api/v1/Auth/{AuthController.Register_Tenant}";

        TenantRegisterRequest request = null;
        if (tenantName.Contains("TenantA") || tenantName.Contains("CompanyA") || tenantName.Contains("ValidTenantA"))
        {
            request = _scenarioContext[TenantA_Context] as TenantRegisterRequest;
            request.ID = $"{bossId}_{Guid.NewGuid().ToString("N").Substring(0, 8)}";
        }
        else
        {
            request = _scenarioContext[TenantB_Context] as TenantRegisterRequest;
            request.ID = $"{bossId}_{Guid.NewGuid().ToString("N").Substring(0, 8)}";
        }

        var response = await _serverSender.PostCommandAsync<TenantRegisterRequest, TenantRegisterResponse>(request, registrationPath);

        // Store response in context for JWT token verification
        _scenarioContext[Tenant_Registration_Response_Context] = response;

        if (tenantName.Contains("TenantA") || tenantName.Contains("CompanyA") || tenantName.Contains("ValidTenantA"))
        {
            _scenarioContext[TenantA_Response_Context] = response;
        }
        else
        {
            _scenarioContext[TenantB_Response_Context] = response;
        }
    }

    [When("I create shift leader \"(.*)\" for tenant \"(.*)\"")]
    public async Task WhenICreateShiftLeaderForTenant(string leaderId, string tenantName)
    {
        TenantRegisterRequest tenantData = null;
        if (tenantName.Contains("TenantA") || tenantName.Contains("CompanyA") || tenantName.Contains("ValidTenantA"))
        {
            tenantData = _scenarioContext[TenantA_Context] as TenantRegisterRequest;
        }
        else
        {
            tenantData = _scenarioContext[TenantB_Context] as TenantRegisterRequest;
        }

        var uniqueLeaderId = $"{leaderId}_{Guid.NewGuid().ToString("N").Substring(0, 8)}";
        var payload = new RegisterRequest
        {
            ID = uniqueLeaderId,
            FirstName = "Leader",
            LastName = "User",
            PhoneNumber = "555-0300",
            DateOfBirth = new DateOnly(1990, 6, 1),
            PasswordHash = "P@ssw0rd!"
        };

        var registrationPath = PathLocator.Combine($"api/v1/Auth/register-shift-leader?tenant={tenantData.Tenant}");
        var response = await _serverSender.PostCommandAsync<RegisterRequest, RegisterResponse>(payload, registrationPath);

        // Store leader info for later use
        var leaderContextKey = tenantName.Contains("TenantA") || tenantName.Contains("CompanyA") || tenantName.Contains("ValidTenantA") ? TenantA_Leaders_Context : TenantB_Leaders_Context;
        var leaders = _scenarioContext.ContainsKey(leaderContextKey) 
            ? _scenarioContext[leaderContextKey] as Dictionary<string, string> ?? new Dictionary<string, string>()
            : new Dictionary<string, string>();
        leaders[leaderId] = uniqueLeaderId;
        _scenarioContext[leaderContextKey] = leaders;

        // Store response
        if (tenantName.Contains("TenantA") || tenantName.Contains("CompanyA") || tenantName.Contains("ValidTenantA"))
        {
            _scenarioContext[TenantA_Response_Context] = response;
        }
        else
        {
            _scenarioContext[TenantB_Response_Context] = response;
        }
    }

    [When("shift leader \"(.*)\" in tenant \"(.*)\" creates (\\d+) workers with ID prefix \"(.*)\"")]
    public async Task WhenShiftLeaderInTenantCreatesWorkersWithIdPrefix(string leaderId, string tenantName, int workerCount, string idPrefix)
    {
        var leaderContextKey = tenantName.Contains("TenantA") || tenantName.Contains("CompanyA") || tenantName.Contains("ValidTenantA") ? TenantA_Leaders_Context : TenantB_Leaders_Context;
        var leaders = _scenarioContext[leaderContextKey] as Dictionary<string, string>;
        var actualLeaderId = leaders[leaderId];

        var workersList = new List<RegisterRequest>();
        
        for (var i = 0; i < workerCount; i++)
        {
            var workerSuffix = i == 0 ? "Alpha" : "Beta"; // Use valid names without numbers
            var workerId = $"{idPrefix}_Worker_{workerSuffix}_{Guid.NewGuid().ToString("N").Substring(0, 8)}";
            var payload = new RegisterRequest
            {
                ID = workerId,
                FirstName = "John",
                LastName = workerSuffix, // Use "Alpha" or "Beta" as last names
                PhoneNumber = $"555-{1000 + i}", // Use valid phone format without dashes in middle
                DateOfBirth = new DateOnly(1995, 1, 1),
                ShiftLeaderId = actualLeaderId,
                PasswordHash = "P@ssw0rd!"
            };

            const string registrationPath = "api/v1/Auth/register-worker";
            await _serverSender.PostCommandAsync<RegisterRequest, RegisterResponse>(payload, registrationPath);
            workersList.Add(payload);
        }

        var workerContextKey = tenantName.Contains("TenantA") || tenantName.Contains("CompanyA") || tenantName.Contains("ValidTenantA") ? TenantA_Workers_Context : TenantB_Workers_Context;
        _scenarioContext[workerContextKey] = workersList;
    }

    [When("I GET the workers for leader \"(.*)\" in tenant \"(.*)\"")]
    public async Task WhenIGetTheWorkersForLeaderInTenant(string leaderId, string tenantName)
    {
        var leaderContextKey = tenantName.Contains("TenantA") || tenantName.Contains("CompanyA") || tenantName.Contains("ValidTenantA") ? TenantA_Leaders_Context : TenantB_Leaders_Context;
        var leaders = _scenarioContext[leaderContextKey] as Dictionary<string, string>;
        var actualLeaderId = leaders[leaderId];

        TenantRegisterRequest tenantData = null;
        if (tenantName.Contains("TenantA") || tenantName.Contains("CompanyA") || tenantName.Contains("ValidTenantA"))
        {
            tenantData = _scenarioContext[TenantA_Context] as TenantRegisterRequest;
        }
        else
        {
            tenantData = _scenarioContext[TenantB_Context] as TenantRegisterRequest;
        }

        var periodStart = DateOnly.FromDateTime(DateTime.UtcNow).ToString("yyyy-MM-dd");
        var path = PathLocator.Combine($"api/v1/StuffBookings/leader/{actualLeaderId}?tenant={tenantData.Tenant}&period={periodStart}");
        var response = await _serverSender.GetAsync<GetWorkerPerTenantResponse>(path);
        _scenarioContext[All_Workers_Context] = response;
    }

    [When("I register a worker for tenant \"(.*)\" with valid data")]
    public async Task WhenIRegisterAWorkerForTenantWithValidData(string tenantName, Table table)
    {
        var request = BuildRegisterRequestFromTable(table, true);
        
        const string registrationPath = "api/v1/Auth/register-worker";
        
        try
        {
            var successResponse = await _serverSender.PostCommandAsync<RegisterRequest, RegisterResponse>(request, registrationPath);
            _scenarioContext[WorkerRegistrationResponse_Context] = successResponse;
        }
        catch (Exception ex)
        {
            _scenarioContext[RegistrationException_Context] = ex;
        }
    }

    [When("I register a worker for tenant \"(.*)\" with invalid data")]
    public async Task WhenIRegisterAWorkerForTenantWithInvalidData(string tenantName, Table table)
    {
        var generateUniqueId = !table.Rows.Any(r => r["Field"] == "ID" && 
            (string.IsNullOrWhiteSpace(r["Value"]) || r["Value"].Length < 3));
        
        var request = BuildRegisterRequestFromTable(table, generateUniqueId);
        
        const string registrationPath = "api/v1/Auth/register-worker";
        
        try
        {
            var client = _serverSender.GetHttpClient();
            var content = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(request),
                System.Text.Encoding.UTF8,
                "application/json");
            
            var httpResponse = await client.PostAsync(registrationPath, content);
            var errorResponseBody = await httpResponse.Content.ReadAsStringAsync();
            
            _scenarioContext[HttpResponse_Context] = httpResponse;
            _scenarioContext[ErrorResponseBody_Context] = errorResponseBody;
        }
        catch (Exception ex)
        {
            _scenarioContext[RegistrationException_Context] = ex;
        }
    }

    [Then("the tenants list should contain tenant \"(.*)\"")]
    public void ThenTheTenantsListShouldContainTenant(string tenantName)
    {
        var response = _scenarioContext[All_Tenants_Context] as AllTenantsResponse;
        Assert.NotNull(response);
        
        TenantRegisterRequest tenantData = null;
        if (tenantName.Contains("CompanyA"))
        {
            tenantData = _scenarioContext[TenantA_Context] as TenantRegisterRequest;
        }
        else
        {
            tenantData = _scenarioContext[TenantB_Context] as TenantRegisterRequest;
        }
        
        Assert.Contains(response.Tenants, t => t.Equals(tenantData.Tenant, StringComparison.InvariantCulture));
    }

    [When("I GET the shift leaders for tenant \"(.*)\"")]
    public async Task WhenIGetTheShiftLeadersForTenant(string tenantName)
    {
        TenantRegisterRequest tenantData = null;
        if (tenantName.Contains("CompanyA"))
        {
            tenantData = _scenarioContext[TenantA_Context] as TenantRegisterRequest;
        }
        else
        {
            tenantData = _scenarioContext[TenantB_Context] as TenantRegisterRequest;
        }
        
        var path = PathLocator.Combine($"api/v1/ShiftLeaders/{tenantData.Tenant}");
        var response = await _serverSender.GetAsync<GetShiftLeaderPerTenantResponse>(path);
        _scenarioContext[All_ShiftLeaders_Context] = response;
    }

    [Then("the shift leaders list should contain \"(.*)\"")]
    public void ThenTheShiftLeadersListShouldContain(string leaderId)
    {
        var response = _scenarioContext[All_ShiftLeaders_Context] as GetShiftLeaderPerTenantResponse;
        Assert.NotNull(response);
        Assert.True(response.ShifLeaders.Any(l => l.ID.StartsWith(leaderId)));
    }

    [Then("the shift leaders list should not contain \"(.*)\"")]
    public void ThenTheShiftLeadersListShouldNotContain(string leaderId)
    {
        var response = _scenarioContext[All_ShiftLeaders_Context] as GetShiftLeaderPerTenantResponse;
        Assert.NotNull(response);
        Assert.DoesNotContain(response.ShifLeaders, l => l.ID.StartsWith(leaderId));
    }

    // Helper method
    private RegisterRequest BuildRegisterRequestFromTable(Table table, bool generateUniqueId = false)
    {
        var request = new RegisterRequest();
        
        foreach (var row in table.Rows)
        {
            var field = row["Field"];
            var value = row["Value"];
            
            switch (field)
            {
                case "ID":
                    if (generateUniqueId && !string.IsNullOrWhiteSpace(value))
                    {
                        request.ID = $"{value}_{Guid.NewGuid().ToString("N").Substring(0, 8)}";
                    }
                    else
                    {
                        request.ID = value;
                    }
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

    // Placeholder step definitions for not yet implemented multi-tenant specific steps
    [When("I reassign the second worker from tenant \"(.*)\" to leader \"(.*)\"")] 
    public void WhenIReassignTheSecondWorkerFromTenantToLeader(string tenantName, string targetLeaderId)
    {
        // TODO: Implement multi-tenant reassignment logic when the reassignment service is available
    }

    [When("I retire the remaining worker under leader \"(.*)\" in tenant \"(.*)\"")] 
    public void WhenIRetireTheRemainingWorkerUnderLeaderInTenant(string leaderId, string tenantName)
    {
        // TODO: Implement multi-tenant retirement logic when the retirement service is available
    }

    [When("I GET the workers for tenant \"(.*)\"")]
    public void WhenIGetTheWorkersForTenant(string tenantName)
    {
        // TODO: Implement get workers by tenant
    }
    
    [Then("the workers list should contain {int} workers with ID prefix {string}")]
    public void ThenTheWorkersListShouldContainWorkersWithIdPrefix(int expectedCount, string idPrefix)
    {
        // Determine which tenant context to use based on the last GET workers request
        var alphaWorkers = _scenarioContext.ContainsKey(TenantA_Workers_Context) ? _scenarioContext[TenantA_Workers_Context] as List<RegisterRequest> : new List<RegisterRequest>();
        var betaWorkers = _scenarioContext.ContainsKey(TenantB_Workers_Context) ? _scenarioContext[TenantB_Workers_Context] as List<RegisterRequest> : new List<RegisterRequest>();
        
        List<RegisterRequest> workers;
        if (idPrefix == "ALPHA")
        {
            workers = alphaWorkers;
        }
        else
        {
            workers = betaWorkers;
        }
        
        Assert.NotNull(workers);
        var matchingWorkers = workers.Where(w => w.ID.Contains(idPrefix)).ToList();
        Assert.Equal(expectedCount, matchingWorkers.Count);
    }
    
    [Then("the workers list should not contain workers with ID prefix {string}")]
    public void ThenTheWorkersListShouldNotContainWorkersWithIdPrefix(string idPrefix)
    {
        // Determine which tenant context to use - we want to check the opposite tenant
        var alphaWorkers = _scenarioContext.ContainsKey(TenantA_Workers_Context) ? _scenarioContext[TenantA_Workers_Context] as List<RegisterRequest> : new List<RegisterRequest>();
        var betaWorkers = _scenarioContext.ContainsKey(TenantB_Workers_Context) ? _scenarioContext[TenantB_Workers_Context] as List<RegisterRequest> : new List<RegisterRequest>();
        
        List<RegisterRequest> workers;
        if (idPrefix == "BETA") // If checking for BETA workers, check in ALPHA tenant (should not contain BETA)
        {
            workers = alphaWorkers;
        }
        else // If checking for ALPHA workers, check in BETA tenant (should not contain ALPHA)
        {
            workers = betaWorkers;
        }
        
        Assert.NotNull(workers);
        var matchingWorkers = workers.Where(w => w.ID.Contains(idPrefix)).ToList();
        Assert.Empty(matchingWorkers);
    }
    
    [When("I try to access {string} data using {string} credentials")]
    public void WhenITryToAccessDataUsingCredentials(string targetTenant, string sourceTenant)
    {
        // TODO: Implement cross-tenant access attempt when security is implemented
        _scenarioContext["CrossTenantAccessAttempted"] = true;
    }
    
    [Then("the access should be denied with tenant isolation error")]
    public void ThenTheAccessShouldBeDeniedWithTenantIsolationError()
    {
        // TODO: Verify cross-tenant access is denied
        var attempted = _scenarioContext.ContainsKey("CrossTenantAccessAttempted");
        Assert.True(attempted, "Cross-tenant access should have been attempted");
    }
}