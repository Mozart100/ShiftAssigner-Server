using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Reqnroll;
using ShiftAssignerServer.Controllers;
using ShiftAssignerServer.Requests;
using ShiftAssignerServer.Tests.Common;
using ShiftAssignerServer.Tests.Infrastructure;

namespace ShiftAssignerServer.Tests.Steps.When;

[Binding]
public class WhenCommonSteps : TwoTenantsStep
{
    private const string TenantA_Context = "TenantA_Data";
    private const string TenantB_Context = "TenantB_Data";
    private const string TenantA_Response_Context = "TenantA_Response";
    private const string TenantB_Response_Context = "TenantB_Response";
    private const string TenantA_Leaders_Context = "TenantA_Leaders";
    private const string TenantB_Leaders_Context = "TenantB_Leaders";
    private const string CurrentLeaderId_Context = "currentLeaderId";
    private const string PeriodStart_Context = "periodStart";
    private const string ReassignWorkerResponse_Context = "ReassignWorkerResponse";
    private const string RetireWorkerResponse_Context = "RetireWorkerResponse";

    public WhenCommonSteps(ScenarioContext scenarioContext) : base(scenarioContext)
    {
    }

    [When("Tenant registration \"(.*)\"")]
    public async Task WhenIPostThePayloadTo(string tenantId)
    {
        const string registrationPath = $"api/v1/Auth/{AuthController.Register_Tenant}";

        var request = _scenarioContext[Tenant_Registration_Data_Context] as TenantRegisterRequest;
        // Generate unique ID to avoid conflicts
        request.ID = $"{tenantId}_{Guid.NewGuid().ToString("N").Substring(0, 8)}";

        var response = await _serverSender.PostCommandAsync<TenantRegisterRequest, TenantRegisterResponse>(request, registrationPath!);

        _scenarioContext[Tenant_Registration_Response_Context] = response;
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

    [When("I create a shift leader with id \"(.*)\"")]
    public async Task WhenICreateAShiftLeaderForTenant(string leaderId)
    {
        // Generate unique ID to avoid conflicts
        var uniqueLeaderId = $"{leaderId}_{Guid.NewGuid().ToString("N").Substring(0, 8)}";
        
        var payload = new RegisterRequest
        {
            ID = uniqueLeaderId,
            FirstName = "Bob",
            LastName = "Leader",
            PhoneNumber = "555-0200",
            DateOfBirth = new System.DateOnly(1990, 6, 1),
            PasswordHash = "P@ssw0rd!"
        };
        var tenant = (_scenarioContext[Tenant_Registration_Data_Context] as TenantRegisterRequest)?.Tenant;
        var registrationPath = PathLocator.Combine($"api/v1/Auth/register-shift-leader?tenant={tenant}");

        var response = await _serverSender.PostCommandAsync<RegisterRequest, RegisterResponse>(payload, registrationPath);
        _scenarioContext[Tenant_Registration_Response_Context] = response;
        
        // Store the actual leader ID for later verification using a key pattern
        _scenarioContext[CurrentLeaderId_Context] = uniqueLeaderId;
        _scenarioContext[$"LeaderID_{leaderId}"] = uniqueLeaderId;
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

    [When("I GET the tenants list")]
    public async Task WhenIGetTheTenantsList()
    {
        var path = PathLocator.Combine("api/v1/Tenants");

        var response = await _serverSender.GetAsync<AllTenantsResponse>(path);
        _scenarioContext[All_Tenants_Context] = response;
    }

    [When("I GET the shiftleaders")]
    public async Task WhenIGetTheShiftLeadersForTenant()
    {
        var tenant = (_scenarioContext[Tenant_Registration_Data_Context] as TenantRegisterRequest)?.Tenant;
        var path = PathLocator.Combine($"api/v1/ShiftLeaders/{tenant}");
        var response = await _serverSender.GetAsync<GetShiftLeaderPerTenantResponse>(path);
        _scenarioContext[All_ShiftLeaders_Context] = response;
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

    [When("the shift leader creates 2 workers")]
    public async Task WhenTheShiftLeaderCreatesTwoWorkers()
    {
        var shiftLeadersResponse = _scenarioContext[All_ShiftLeaders_Context] as GetShiftLeaderPerTenantResponse;
        var shiftLeaderId = shiftLeadersResponse?.ShifLeaders.FirstOrDefault()?.ID;
        
        var workersData = new List<RegisterRequest>();
        var workersResponses = new List<RegisterResponse>();

        for (var i = 0; i < 2; i++)
        {
            var id = $"Worker_ID_{Guid.NewGuid():N}";
            var payload = new RegisterRequest
            {
                ID = id,
                FirstName = "Worker",
                LastName = i == 0 ? "One" : "Two",
                PhoneNumber = "555-0300",
                DateOfBirth = new System.DateOnly(1995, 1, 1),
                ShiftLeaderId = shiftLeaderId,
                PasswordHash = "P@ssw0rd!"
            };

            const string registrationPath = "api/v1/Auth/register-worker";
            var response = await _serverSender.PostCommandAsync<RegisterRequest, RegisterResponse>(payload, registrationPath);
            
            // Store each worker registration data and response
            workersData.Add(payload);
            workersResponses.Add(response);
        }

        // Store all workers registration data and responses in scenario context
        _scenarioContext[Workers_Registration_Data_Context] = workersData;
        _scenarioContext[Workers_Registration_Responses_Context] = workersResponses;
    }

    [When("the shift leader with id \"(.*)\" creates 2 workers")]
    public async Task WhenTheShiftLeaderWithIdCreatesTwoWorkers(string leaderId)
    {
        // Get the actual leader ID (with unique suffix) from context if it exists
        var actualLeaderId = leaderId;
        var leaderKey = $"LeaderID_{leaderId}";
        if (_scenarioContext.ContainsKey(leaderKey))
        {
            actualLeaderId = _scenarioContext[leaderKey] as string ?? leaderId;
        }
        
        _scenarioContext[CurrentLeaderId_Context] = actualLeaderId;

        var workersData = new List<RegisterRequest>();
        var workersResponses = new List<RegisterResponse>();

        for (var i = 0; i < 2; i++)
        {
            var id = $"Worker_ID_{Guid.NewGuid():N}";
            var payload = new RegisterRequest
            {
                ID = id,
                FirstName = "Worker",
                LastName = i == 0 ? "One" : "Two",
                PhoneNumber = "555-0300",
                DateOfBirth = new System.DateOnly(1995, 1, 1),
                ShiftLeaderId = actualLeaderId,
                PasswordHash = "P@ssw0rd!"
            };

            const string registrationPath = "api/v1/Auth/register-worker";
            var response = await _serverSender.PostCommandAsync<RegisterRequest, RegisterResponse>(payload, registrationPath);

            // Store each worker registration data and response
            workersData.Add(payload);
            workersResponses.Add(response);
        }

        // Store all workers registration data and responses in scenario context
        _scenarioContext[Workers_Registration_Data_Context] = workersData;
        _scenarioContext[Workers_Registration_Responses_Context] = workersResponses;
    }

    [When("I GET the workers")]
    public async Task WhenIGetTheWorkers()
    {
        var shiftLeadersResponse = _scenarioContext[All_ShiftLeaders_Context] as GetShiftLeaderPerTenantResponse;
        var leaderId = shiftLeadersResponse?.ShifLeaders.FirstOrDefault()?.ID;
        var path = PathLocator.Combine($"api/v1/Workers/leader/{leaderId}");
        var response = await _serverSender.GetAsync<GetWorkerPerTenantResponse>(path);
        _scenarioContext[All_Workers_Context] = response;
    }

    [When("I GET the workers for leader \"(.*)\"")]
    public async Task WhenIGetTheWorkersForLeader(string leaderId)
    {
        // Get the actual leader ID (with unique suffix) from context if it exists
        var actualLeaderId = leaderId;
        var leaderKey = $"LeaderID_{leaderId}";
        if (_scenarioContext.ContainsKey(leaderKey))
        {
            actualLeaderId = _scenarioContext[leaderKey] as string ?? leaderId;
        }
        
        var tenant = (_scenarioContext[Tenant_Registration_Data_Context] as TenantRegisterRequest)?.Tenant;
        var periodStart = DateOnly.FromDateTime(DateTime.UtcNow).ToString("yyyy-MM-dd");
        _scenarioContext[PeriodStart_Context] = periodStart;

        var path = PathLocator.Combine($"api/v1/StuffBookings/leader/{actualLeaderId}?tenant={tenant}&period={periodStart}");
        var response = await _serverSender.GetAsync<GetWorkerPerTenantResponse>(path);
        _scenarioContext[All_Workers_Context] = response;
    }

    [When("I reassign the second worker to leader \"(.*)\"")]
    public async Task WhenIReassignTheSecondWorkerToLeader(string targetLeaderId)
    {
        // Get the actual leader ID (with unique suffix) from context if it exists
        var actualLeaderId = targetLeaderId;
        var leaderKey = $"LeaderID_{targetLeaderId}";
        if (_scenarioContext.ContainsKey(leaderKey))
        {
            actualLeaderId = _scenarioContext[leaderKey] as string ?? targetLeaderId;
        }
        
        var tenant = (_scenarioContext[Tenant_Registration_Data_Context] as TenantRegisterRequest)?.Tenant;
        var workersData = _scenarioContext[Workers_Registration_Data_Context] as List<RegisterRequest>;
        var secondWorker = workersData?[1];
        var periodStart = _scenarioContext[PeriodStart_Context] as string;

        var reassignRequest = new ReassignWorkerRequest
        {
            WorkerId = secondWorker.ID,
            ShiftLeaderId = actualLeaderId,
            Tenant = tenant,
            PeriodStart = periodStart,
            Notes = "Reassigned for testing"
        };

        const string reassignPath = "api/v1/StuffBookings/reassign";
        var response = await _serverSender.PostCommandAsync<ReassignWorkerRequest, ReassignWorkerResponse>(reassignRequest, reassignPath);
        
        // Store the response for potential validation
        _scenarioContext[ReassignWorkerResponse_Context] = response;
    }

    [When("I retire the remaining worker under leader \"(.*)\"")]
    public async Task WhenIRetireTheRemainingWorkerUnderLeader(string leaderId)
    {
        var tenant = (_scenarioContext[Tenant_Registration_Data_Context] as TenantRegisterRequest)?.Tenant;
        var workersData = _scenarioContext[Workers_Registration_Data_Context] as List<RegisterRequest>;
        
        // Get the first worker (the one still under leader-A)
        var firstWorker = workersData?[0];

        var retireRequest = new RetireWorkerRequest
        {
            WorkerId = firstWorker.ID,
            Tenant = tenant,
            Reason = "Worker retired for testing"
        };

        const string retirePath = "api/v1/Workers/retire";
        var response = await _serverSender.PostCommandAsync<RetireWorkerRequest, object>(retireRequest, retirePath);
        
        // Store the response for validation
        _scenarioContext[RetireWorkerResponse_Context] = response;
    }

    [When("I register a worker for tenant \"(.*)\" with valid data")]
    public async Task WhenIRegisterAWorkerForTenantWithValidData(string tenantName, Table table)
    {
        var request = BuildRegisterRequestFromTable(table, true);
        
        const string registrationPath = "api/v1/Auth/register-worker";
        
        try
        {
            var successResponse = await _serverSender.PostCommandAsync<RegisterRequest, RegisterResponse>(request, registrationPath);
            _scenarioContext["WorkerRegistrationResponse"] = successResponse;
        }
        catch (Exception ex)
        {
            _scenarioContext["RegistrationException"] = ex;
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
            var content = new System.Net.Http.StringContent(
                System.Text.Json.JsonSerializer.Serialize(request),
                System.Text.Encoding.UTF8,
                "application/json");
            
            var httpResponse = await client.PostAsync(registrationPath, content);
            var errorResponseBody = await httpResponse.Content.ReadAsStringAsync();
            
            _scenarioContext["HttpResponse"] = httpResponse;
            _scenarioContext["ErrorResponseBody"] = errorResponseBody;
        }
        catch (Exception ex)
        {
            _scenarioContext["RegistrationException"] = ex;
        }
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
    
    [When("I try to access {string} data using {string} credentials")]
    public void WhenITryToAccessDataUsingCredentials(string targetTenant, string sourceTenant)
    {
        // TODO: Implement cross-tenant access attempt when security is implemented
        _scenarioContext["CrossTenantAccessAttempted"] = true;
    }

    [When("shift leader \"(.*)\" in tenant \"(.*)\" creates (\\d+) workers with ID prefix \"(.*)\"")]
    public async Task WhenShiftLeaderInTenantCreatesWorkersWithIdPrefix(string leaderId, string tenantName, int workerCount, string idPrefix)
    {
        var leaderContextKey = tenantName.Contains("TenantA") || tenantName.Contains("CompanyA") || tenantName.Contains("ValidTenantA") ? "TenantA_Leaders" : "TenantB_Leaders";
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

        var workerContextKey = tenantName.Contains("TenantA") || tenantName.Contains("CompanyA") || tenantName.Contains("ValidTenantA") ? "TenantA_Workers" : "TenantB_Workers";
        _scenarioContext[workerContextKey] = workersList;
    }
}