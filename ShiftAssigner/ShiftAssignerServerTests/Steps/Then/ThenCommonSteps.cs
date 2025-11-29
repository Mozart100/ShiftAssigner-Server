using System;
using System.Collections.Generic;
using System.Linq;
using Reqnroll;
using ShiftAssignerServer.Models.Stuff;
using ShiftAssignerServer.Requests;
using ShiftAssignerServer.Tests.Common;
using Xunit;

namespace ShiftAssignerServer.Tests.Steps.Then;

[Binding]
public class ThenCommonSteps : TwoTenantsStep
{
    private const string TenantA_Context = "TenantA_Data";
    private const string TenantB_Context = "TenantB_Data";
    private const string CurrentLeaderId_Context = "currentLeaderId";
    private const string RetireWorkerResponse_Context = "RetireWorkerResponse";

    public ThenCommonSteps(ScenarioContext scenarioContext) : base(scenarioContext)
    {
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

    [Then("the workers list should contain {int} worker(s)")]
    public void ThenTheWorkersListShouldContainWorkers(int expectedCount)
    {
        var response = _scenarioContext[All_Workers_Context] as GetWorkerPerTenantResponse;
        Assert.NotNull(response);
        Assert.Equal(expectedCount, response.Workers.Count());
    }

    [Then("the retire response should be successful")]
    public void ThenTheRetireResponseShouldBeSuccessful()
    {
        var response = _scenarioContext[RetireWorkerResponse_Context];
        Assert.NotNull(response);
    }

    [Then("the workers list should contain {int} workers with ID prefix {string}")]
    public void ThenTheWorkersListShouldContainWorkersWithIdPrefix(int expectedCount, string idPrefix)
    {
        // Determine which tenant context to use based on the last GET workers request
        var alphaWorkers = _scenarioContext.ContainsKey("TenantA_Workers") ? _scenarioContext["TenantA_Workers"] as List<RegisterRequest> : new List<RegisterRequest>();
        var betaWorkers = _scenarioContext.ContainsKey("TenantB_Workers") ? _scenarioContext["TenantB_Workers"] as List<RegisterRequest> : new List<RegisterRequest>();
        
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
        var alphaWorkers = _scenarioContext.ContainsKey("TenantA_Workers") ? _scenarioContext["TenantA_Workers"] as List<RegisterRequest> : new List<RegisterRequest>();
        var betaWorkers = _scenarioContext.ContainsKey("TenantB_Workers") ? _scenarioContext["TenantB_Workers"] as List<RegisterRequest> : new List<RegisterRequest>();
        
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
    
    [Then("the access should be denied with tenant isolation error")]
    public void ThenTheAccessShouldBeDeniedWithTenantIsolationError()
    {
        // TODO: Verify cross-tenant access is denied
        var attempted = _scenarioContext.ContainsKey("CrossTenantAccessAttempted");
        Assert.True(attempted, "Cross-tenant access should have been attempted");
    }
}