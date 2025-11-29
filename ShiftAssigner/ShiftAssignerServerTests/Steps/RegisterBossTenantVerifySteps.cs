using System;
using System.Collections.Generic;
using System.Linq;
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
public class RegisterBossTenantVerifySteps : SingleTenantStep
{
    public const string Tenant_ID = "Acme ltd";
    private const string CurrentLeaderId_Context = "currentLeaderId";


    public RegisterBossTenantVerifySteps(ScenarioContext scenarioContext):base(scenarioContext)
    {
    }

    [Given("I have a tenant boss registration payload")]
    public void GivenIHaveATenantBossRegistrationPayload()
    {
        var tenantName = $"{Tenant_ID}_{Guid.NewGuid()}";
        var payload = new TenantRegisterRequest
        {
            ID = "boss-verify-1",
            FirstName = "Alice",
            LastName = "Owner",
            PhoneNumber = "555-0100",
            DateOfBirth = new System.DateOnly(1985, 1, 1),
            Tenant = tenantName,
            PasswordHash = "P@ssw0rd!"
        };

        _scenarioContext[Tenant_Registration_Data_Context] = payload;
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

    [Then("the response should contain a JWT token")]
    public void ThenTheResponseShouldContainJwtToken()
    {
        // Try to get as RegisterResponse first (for single tenant tests)
        var response = _scenarioContext[Tenant_Registration_Response_Context] as RegisterResponse;
        
        // If not found, try as TenantRegisterResponse (for multi-tenant tests)
        if (response == null)
        {
            response = _scenarioContext[Tenant_Registration_Response_Context] as TenantRegisterResponse;
        }
        
        Assert.NotNull(response);
        Assert.True(!string.IsNullOrWhiteSpace(response!.Token));
    }

    [When("I GET the tenants list")]
    public async Task WhenIGetTheTenantsList()
    {
        var path = PathLocator.Combine("api/v1/Tenants");

        var response = await _serverSender.GetAsync<AllTenantsResponse>(path);
        _scenarioContext[All_Tenants_Context] = response;
    }

    [Then("the tenants list should contain the tenant")]
    public void ThenTheTenantsListShouldContainCompany()
    {
        var response = _scenarioContext[All_Tenants_Context] as AllTenantsResponse;
        var isContains = false;

        var payload = _scenarioContext[Tenant_Registration_Data_Context] as TenantRegisterRequest;
        var tenant = payload?.Tenant ?? string.Empty;

        foreach (var ten in response.Tenants)
        {
            if (ten.Equals(tenant, System.StringComparison.InvariantCulture))
            {
                isContains = true;
                break;
            }
        }

        Assert.True(isContains);
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

    [When("I GET the shiftleaders")]
    public async Task WhenIGetTheShiftLeadersForTenant()
    {
        var tenant = (_scenarioContext[Tenant_Registration_Data_Context] as TenantRegisterRequest)?.Tenant;
        var path = PathLocator.Combine($"api/v1/ShiftLeaders/{tenant}");
        var response = await _serverSender.GetAsync<GetShiftLeaderPerTenantResponse>(path);
        _scenarioContext[All_ShiftLeaders_Context] = response;
    }

    [Then("the shiftleaders list should contain id \"(.*)\"")]
    public void ThenTheShiftLeadersListShouldContainId(string leaderId)
    {
        var response = _scenarioContext[All_ShiftLeaders_Context] as GetShiftLeaderPerTenantResponse;
        Assert.NotNull(response);
        
        // Get the actual leader ID that was created (with unique suffix)
        var actualLeaderId = _scenarioContext.ContainsKey(CurrentLeaderId_Context) 
            ? _scenarioContext[CurrentLeaderId_Context] as string 
            : leaderId;
        
        var exists = false;

        foreach (var shiftLeader in response.ShifLeaders)
        {
            if (shiftLeader.ID.Equals(actualLeaderId, System.StringComparison.InvariantCulture) ||
                shiftLeader.ID.StartsWith(leaderId, System.StringComparison.InvariantCulture))
            {
                exists = true;
                break;
            }
        }

        Assert.True(exists);
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

    [When("I GET the workers")]
    public async Task WhenIGetTheWorkers()
    {
        var shiftLeadersResponse = _scenarioContext[All_ShiftLeaders_Context] as GetShiftLeaderPerTenantResponse;
        var leaderId = shiftLeadersResponse?.ShifLeaders.FirstOrDefault()?.ID;
        var path = PathLocator.Combine($"api/v1/Workers/leader/{leaderId}");
        var response = await _serverSender.GetAsync<GetWorkerPerTenantResponse>(path);
        _scenarioContext[All_Workers_Context] = response;
    }

    [Then("the workers list should contain the created workers")]
    public void ThenTheWorkersListShouldContainCreated()
    {
        var response = _scenarioContext[All_Workers_Context] as GetWorkerPerTenantResponse;
        Assert.NotNull(response);
        
        var workersData = _scenarioContext[Workers_Registration_Data_Context] as List<RegisterRequest> ?? new List<RegisterRequest>();

        foreach (var workerData in workersData)
        {
            var found = false;
            foreach (var w in response.Workers)
            {
                if (w.ID.Equals(workerData.ID, StringComparison.InvariantCulture))
                {
                    found = true;
                    break;
                }
            }

            Assert.True(found, $"Worker with id {workerData.ID} was not found in workers list");
        }
    }
}
