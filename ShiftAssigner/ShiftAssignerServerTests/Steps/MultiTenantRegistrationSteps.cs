using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Reqnroll;
using ShiftAssignerServer.Controllers;
using ShiftAssignerServer.Requests;
using ShiftAssignerServer.Tests.Common;
using Xunit;

namespace ShiftAssignerServer.Tests.Steps;

/// <summary>
/// Step definitions for multi-tenant registration scenarios.
/// Handles registration and verification of multiple tenants with their respective shift leaders and workers.
/// </summary>
[Binding]
public class MultiTenantRegistrationSteps : FeatureStepBase
{
    // Context keys for scenario data
    private const string MultiTenant_Data_Context = "MultiTenantData";
    
    public MultiTenantRegistrationSteps(ScenarioContext scenarioContext) : base(scenarioContext)
    {
    }

    [Given(@"I have tenant boss registration payloads for multi tenant flow")]
    public void GivenIHaveTenantBossRegistrationPayloadsForMultiTenantFlow()
    {
        var multiTenantData = new MultiTenantTestData();
        _scenarioContext[MultiTenant_Data_Context] = multiTenantData;
    }

    [When(@"I register tenant ""(.*)"" for multi tenant flow")]
    public async Task WhenIRegisterTenantForMultiTenantFlow(string tenantId)
    {
        var multiTenantData = _scenarioContext.Get<MultiTenantTestData>(MultiTenant_Data_Context);
        var tenantRequest = CreateTenantRegistration(tenantId);

        var tenantResponse = await _serverSender.PostCommandAsync<TenantRegisterRequest, TenantRegisterResponse>(
            AUTH_REGISTER_BOSS_TENANT, tenantRequest);

        var tenantInfo = new TenantInfo
        {
            TenantId = tenantId,
            TenantRequest = tenantRequest,
            TenantResponse = tenantResponse
        };

        multiTenantData.Tenants[tenantId] = tenantInfo;
    }

    [Then(@"the tenant ""(.*)"" registration response should contain a JWT token")]
    public void ThenTheTenantRegistrationResponseShouldContainAJWTToken(string tenantId)
    {
        var multiTenantData = _scenarioContext.Get<MultiTenantTestData>(MultiTenant_Data_Context);
        var tenantInfo = multiTenantData.Tenants[tenantId];

        Assert.NotNull(tenantInfo.TenantResponse);
        Assert.NotNull(tenantInfo.TenantResponse.Token);
        Assert.NotEmpty(tenantInfo.TenantResponse.Token);
    }

    [When(@"I register shiftleader ""(.*)"" for tenant ""(.*)"" in multi tenant flow")]
    public async Task WhenIRegisterShiftleaderForTenantInMultiTenantFlow(string leaderId, string tenantId)
    {
        var multiTenantData = _scenarioContext.Get<MultiTenantTestData>(MultiTenant_Data_Context);
        var tenantInfo = multiTenantData.Tenants[tenantId];
        
        var leaderRequest = CreateShiftLeaderRegistration(leaderId, tenantInfo.TenantRequest.Tenant);

        var leaderResponse = await _serverSender.PostCommandAsync<RegisteringShiftLeaderRequest, RegisteringShiftLeaderResponse>(
            SHIFT_LEADERS_REGISTER,
            leaderRequest, tenantInfo.TenantResponse.Token);

        tenantInfo.ShiftLeaderInfo = new ShiftLeaderInfo
        {
            LeaderId = leaderId,
            LeaderRequest = leaderRequest,
            LeaderResponse = leaderResponse
        };
    }

    [Then(@"the shiftleader registration for tenant ""(.*)"" should contain a JWT token")]
    public void ThenTheShiftleaderRegistrationForTenantShouldContainAJWTToken(string tenantId)
    {
        var multiTenantData = _scenarioContext.Get<MultiTenantTestData>(MultiTenant_Data_Context);
        var tenantInfo = multiTenantData.Tenants[tenantId];

        Assert.NotNull(tenantInfo.ShiftLeaderInfo);
        Assert.NotNull(tenantInfo.ShiftLeaderInfo.LeaderResponse);
        Assert.NotNull(tenantInfo.ShiftLeaderInfo.LeaderResponse.Token);
        Assert.NotEmpty(tenantInfo.ShiftLeaderInfo.LeaderResponse.Token);
    }

    [When(@"shiftleader ""(.*)"" logs in for tenant ""(.*)"" in multi tenant flow")]
    public async Task WhenShiftleaderLogsInForTenantInMultiTenantFlow(string leaderId, string tenantId)
    {
        var multiTenantData = _scenarioContext.Get<MultiTenantTestData>(MultiTenant_Data_Context);
        var tenantInfo = multiTenantData.Tenants[tenantId];

        var loginRequest = new LoginShiftLeaderRequest
        {
            ID = leaderId,
            Password = "LeaderPassword123"
        };

        var loginResponse = await _serverSender.PostCommandAsync<LoginShiftLeaderRequest, LoginShiftLeaderResponse>(
            SHIFT_LEADERS_LOGIN,
            loginRequest, tenantInfo.TenantResponse.Token);

        tenantInfo.ShiftLeaderInfo.LeaderLoginResponse = loginResponse;
    }

    [Then(@"the shiftleader login for tenant ""(.*)"" should contain a JWT token")]
    public void ThenTheShiftleaderLoginForTenantShouldContainAJWTToken(string tenantId)
    {
        var multiTenantData = _scenarioContext.Get<MultiTenantTestData>(MultiTenant_Data_Context);
        var tenantInfo = multiTenantData.Tenants[tenantId];

        Assert.NotNull(tenantInfo.ShiftLeaderInfo.LeaderLoginResponse);
        Assert.NotNull(tenantInfo.ShiftLeaderInfo.LeaderLoginResponse.Token);
        Assert.NotEmpty(tenantInfo.ShiftLeaderInfo.LeaderLoginResponse.Token);
    }

    [When(@"shiftleader ""(.*)"" registers worker ""(.*)"" for tenant ""(.*)"" in multi tenant flow")]
    public async Task WhenShiftleaderRegistersWorkerForTenantInMultiTenantFlow(string leaderId, string workerId, string tenantId)
    {
        var multiTenantData = _scenarioContext.Get<MultiTenantTestData>(MultiTenant_Data_Context);
        var tenantInfo = multiTenantData.Tenants[tenantId];

        var workerRequest = CreateWorkerRegistration(workerId);

        var workerResponse = await _serverSender.PostCommandAsync<RegisteringWorkerRequest, RegisteringWorkerResponse>(
            WORKERS_REGISTER,
            workerRequest, tenantInfo.ShiftLeaderInfo.LeaderLoginResponse.Token);

        tenantInfo.WorkerInfo = new WorkerInfo
        {
            WorkerId = workerId,
            WorkerRequest = workerRequest,
            WorkerResponse = workerResponse
        };
    }

    [Then(@"the worker registration for tenant ""(.*)"" should contain a JWT token")]
    public void ThenTheWorkerRegistrationForTenantShouldContainAJWTToken(string tenantId)
    {
        var multiTenantData = _scenarioContext.Get<MultiTenantTestData>(MultiTenant_Data_Context);
        var tenantInfo = multiTenantData.Tenants[tenantId];

        Assert.NotNull(tenantInfo.WorkerInfo);
        Assert.NotNull(tenantInfo.WorkerInfo.WorkerResponse);
        Assert.NotNull(tenantInfo.WorkerInfo.WorkerResponse.Token);
        Assert.NotEmpty(tenantInfo.WorkerInfo.WorkerResponse.Token);
    }

    [When(@"worker ""(.*)"" logs in for tenant ""(.*)"" in multi tenant flow")]
    public async Task WhenWorkerLogsInForTenantInMultiTenantFlow(string workerId, string tenantId)
    {
        var multiTenantData = _scenarioContext.Get<MultiTenantTestData>(MultiTenant_Data_Context);
        var tenantInfo = multiTenantData.Tenants[tenantId];

        var workerLoginRequest = new LoginWorkerRequest
        {
            ID = workerId,
            Password = "WorkerPassword123"
        };

        var workerLoginResponse = await _serverSender.PostCommandAsync<LoginWorkerRequest, LoginWorkerResponse>(
            WORKERS_LOGIN,
            workerLoginRequest, tenantInfo.ShiftLeaderInfo.LeaderLoginResponse.Token);

        tenantInfo.WorkerInfo.WorkerLoginResponse = workerLoginResponse;
    }

    [Then(@"the worker login for tenant ""(.*)"" should contain a JWT token")]
    public void ThenTheWorkerLoginForTenantShouldContainAJWTToken(string tenantId)
    {
        var multiTenantData = _scenarioContext.Get<MultiTenantTestData>(MultiTenant_Data_Context);
        var tenantInfo = multiTenantData.Tenants[tenantId];

        Assert.NotNull(tenantInfo.WorkerInfo.WorkerLoginResponse);
        Assert.NotNull(tenantInfo.WorkerInfo.WorkerLoginResponse.Token);
        Assert.NotEmpty(tenantInfo.WorkerInfo.WorkerLoginResponse.Token);
    }

    // Helper methods for creating test data
    private TenantRegisterRequest CreateTenantRegistration(string tenantId)
    {
        return new TenantRegisterRequest
        {
            ID = $"boss-{tenantId}",
            FirstName = "Boss",
            LastName = $"Tenant{tenantId}",
            PhoneNumber = $"+1-555-010{tenantId}",
            DateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddYears(-30)),
            Tenant = $"TestCompany-{tenantId}",
            PasswordHash = "BossPassword123"
        };
    }

    private RegisteringShiftLeaderRequest CreateShiftLeaderRegistration(string leaderId, string tenant)
    {
        return new RegisteringShiftLeaderRequest
        {
            ID = leaderId,
            FirstName = "Shift",
            LastName = "Leader",
            PhoneNumber = "+1-555-0200",
            DateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddYears(-25))
        };
    }

    private RegisteringWorkerRequest CreateWorkerRegistration(string workerId)
    {
        return new RegisteringWorkerRequest
        {
            ID = workerId,
            FirstName = "Test",
            LastName = "Worker",
            PhoneNumber = "+1-555-0300",
            DateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddYears(-22))
        };
    }
}

// Data classes for multi-tenant test scenario
public class MultiTenantTestData
{
    public Dictionary<string, TenantInfo> Tenants { get; set; } = new Dictionary<string, TenantInfo>();
}

public class TenantInfo
{
    public string TenantId { get; set; } = string.Empty;
    public TenantRegisterRequest TenantRequest { get; set; } = new TenantRegisterRequest();
    public TenantRegisterResponse TenantResponse { get; set; } = new TenantRegisterResponse();
    public ShiftLeaderInfo ShiftLeaderInfo { get; set; } = new ShiftLeaderInfo();
    public WorkerInfo WorkerInfo { get; set; } = new WorkerInfo();
}

public class ShiftLeaderInfo
{
    public string LeaderId { get; set; } = string.Empty;
    public RegisteringShiftLeaderRequest LeaderRequest { get; set; } = new RegisteringShiftLeaderRequest();
    public RegisteringShiftLeaderResponse LeaderResponse { get; set; } = new RegisteringShiftLeaderResponse();
    public LoginShiftLeaderResponse LeaderLoginResponse { get; set; } = new LoginShiftLeaderResponse();
}

public class WorkerInfo
{
    public string WorkerId { get; set; } = string.Empty;
    public RegisteringWorkerRequest WorkerRequest { get; set; } = new RegisteringWorkerRequest();
    public RegisteringWorkerResponse WorkerResponse { get; set; } = new RegisteringWorkerResponse();
    public LoginWorkerResponse WorkerLoginResponse { get; set; } = new LoginWorkerResponse();
}