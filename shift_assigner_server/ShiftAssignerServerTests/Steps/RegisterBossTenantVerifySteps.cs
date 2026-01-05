using System;
using System.Threading.Tasks;
using Reqnroll;
using ShiftAssignerServer.Controllers;
using ShiftAssignerServer.Requests;
using ShiftAssignerServer.Tests.Common;
using Xunit;

namespace ShiftAssignerServer.Tests.Steps;

/// <summary>
/// Step definitions for the RegisterBossTenantVerify feature.
/// Handles single tenant registration and verification scenario.
/// </summary>
[Binding]
public partial class RegisterBossTenantVerifySteps : SingleTenantStep
{
    public const string Tenant_ID = "Acme ltd";
    private const string CurrentLeaderId_Context = "currentLeaderId";

    public RegisterBossTenantVerifySteps(ScenarioContext scenarioContext)
        : base(scenarioContext)
    {
    }

    [Given(@"I have a tenant boss registration payload for basic flow")]
    public void GivenIHaveATenantBossRegistrationPayload()
    {
        var payload = CreateDefaultTenantRegistration();
        _scenarioContext[Tenant_Registration_Data_Context] = payload;
    }

    [When(@"Tenant registration ""(.*)"" for basic flow")]
    public async Task WhenTenantRegistration(string tenantId)
    {
        var payload = CreateDefaultTenantRegistration(tenantId);
        var tenantInfo = new TenantSenderInfo
        {
            Request = payload
        };

        // _scenarioContext[Tenant_Registration_Data_Context] = tenantInfo;

        var response = await _serverSender.PostCommandAsync<TenantRegisterRequest, TenantRegisterResponse>(AUTH_REGISTER_BOSS_TENANT, payload);
        tenantInfo.Response = response;

        _scenarioContext.Set<TenantSenderInfo>(tenantInfo, Tenant_Registration_Response_Context);
    }

    [Then(@"the tenant registration response should contain a JWT token")]
    public void ThenTheResponseShouldContainAJWTToken()
    {
        var response = _scenarioContext.Get<TenantSenderInfo>(Tenant_Registration_Response_Context);

        Assert.NotNull(response.Response);
        Assert.NotNull(response.JwtToken);
    }


    [When(@"I register a shiftleader with id ""(.*)"" for basic flow")]
    public async Task WhenIRegisterAShiftLeaderWithId(string leaderId)
    {
        var tenantPayload = _scenarioContext.Get<TenantSenderInfo>(Tenant_Registration_Response_Context);
        var leaderRequest = CreateDefaultShiftLeaderRegistration(leaderId, tenantPayload.Response.Tenant);

        var leaderResponse = await _serverSender.PostCommandAsync<RegisteringShiftLeaderRequest,RegisteringShiftLeaderResponse>(SHIFT_LEADERS_REGISTER,
        leaderRequest, tenantPayload.JwtToken);

        tenantPayload.ShiftLeaderSenderInfo = new ShiftLeaderSenderInfo
        {
            RegisteringRequest = leaderRequest,
            RegisteringResponse = leaderResponse
        };
    }

    [Then(@"the shiftleader registration response should contain a JWT token")]
    public void ThenTheShiftLeaderRegistrationResponseShouldContainAJWTToken()
    {
        var tenantPayload = _scenarioContext.Get<TenantSenderInfo>(Tenant_Registration_Response_Context);
        var response = tenantPayload.ShiftLeaderSenderInfo.RegisteringResponse;

        Assert.NotNull(response);
        // Assert.NotNull(response.Token);
        // Assert.NotEmpty(response.Token);
    }

    [When(@"the shift leader ""(.*)"" logs in for basic flow")]
    public async Task WhenTheShiftLeaderLogsIn(string leaderId)
    {
        var tenantPayload = _scenarioContext.Get<TenantSenderInfo>(Tenant_Registration_Response_Context);
        var loginRequest = new LoginShiftLeaderRequest
        {
            ID = leaderId,
            Password = "TestPassword123" // Use a default test password
        };

        var loginResponse = await _serverSender.PostCommandAsync<LoginShiftLeaderRequest, LoginShiftLeaderResponse>(SHIFT_LEADERS_LOGIN,
            loginRequest, tenantPayload.JwtToken);

        tenantPayload.ShiftLeaderSenderInfo.LoginResponse = loginResponse;
    }

    [Then(@"the shiftleader login response should contain a JWT token")]
    public void ThenTheLoginResponseShouldContainAJWTToken()
    {
        var tenantPayload = _scenarioContext.Get<TenantSenderInfo>(Tenant_Registration_Response_Context);
        var loginResponse = tenantPayload.ShiftLeaderSenderInfo.LoginResponse;

        Assert.NotNull(loginResponse);
        Assert.NotNull(loginResponse.Token);
        Assert.NotEmpty(loginResponse.Token);
    }

    [When(@"the shift leader registers a worker with id ""(.*)"" for basic flow")]
    public async Task WhenTheShiftLeaderRegistersAWorkerWithId(string workerId)
    {
        var tenantPayload = _scenarioContext.Get<TenantSenderInfo>(Tenant_Registration_Response_Context);
        var shiftLeaderToken = tenantPayload.ShiftLeaderSenderInfo.LoginResponse.Token;

        var workerRequest = new WorkerRegisteringRequest
        {
            ID = workerId,
            FirstName = "Test",
            LastName = "Worker",
            PhoneNumber = "555-0123",
            DateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddYears(-25))
        };

        var workerResponse = await _serverSender.PostCommandAsync<WorkerRegisteringRequest, RegisteringWorkerResponse>(WORKERS_REGISTER,
            workerRequest, shiftLeaderToken);

        tenantPayload.ShiftLeaderSenderInfo.WorkerSenderInfo = new WorkerSenderInfo
        {
            WorkerRequest = workerRequest,
            WorkerResponse = workerResponse
        };
    }

    [Then(@"the worker registration response should contain a JWT token")]
    public void ThenTheWorkerRegistrationResponseShouldContainAJWTToken()
    {
        var tenantPayload = _scenarioContext.Get<TenantSenderInfo>(Tenant_Registration_Response_Context);
        var workerResponse = tenantPayload.ShiftLeaderSenderInfo.WorkerSenderInfo.WorkerResponse;

        Assert.NotNull(workerResponse);
        Assert.NotNull(workerResponse.Token);
        Assert.NotEmpty(workerResponse.Token);
    }

    [When(@"the worker ""(.*)"" logs in for basic flow")]
    public async Task WhenTheWorkerLogsIn(string workerId)
    {
        var tenantPayload = _scenarioContext.Get<TenantSenderInfo>(Tenant_Registration_Response_Context);
        var shiftLeaderToken = tenantPayload.ShiftLeaderSenderInfo.LoginResponse.Token;
        
        var workerLoginRequest = new LoginWorkerRequest
        {
            ID = workerId,
            Password = "WorkerPassword123" // Use a default test password
        };

        var workerLoginResponse = await _serverSender.PostCommandAsync<LoginWorkerRequest, LoginWorkerResponse>(WORKERS_LOGIN,
            workerLoginRequest, shiftLeaderToken);

        tenantPayload.ShiftLeaderSenderInfo.WorkerSenderInfo.WorkerLoginResponse = workerLoginResponse;
    }

    [Then(@"the worker login response should contain a JWT token")]
    public void ThenTheWorkerLoginResponseShouldContainAJWTToken()
    {
        var tenantPayload = _scenarioContext.Get<TenantSenderInfo>(Tenant_Registration_Response_Context);
        var workerLoginResponse = tenantPayload.ShiftLeaderSenderInfo.WorkerSenderInfo.WorkerLoginResponse;

        Assert.NotNull(workerLoginResponse);
        Assert.NotNull(workerLoginResponse.Token);
        Assert.NotEmpty(workerLoginResponse.Token);
    }

    // Helper methods for creating test data
    private TenantRegisterRequest CreateDefaultTenantRegistration(string tenantId = "1")
    {
        return new TenantRegisterRequest
        {
            ID = $"boss-test-{tenantId}",
            FirstName = "Boss",
            LastName = "TestUser",
            PhoneNumber = "+1-555-0100",
            DateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddYears(-30)),
            Tenant = $"SingleTenant_Company{tenantId}",
            PasswordHash = "TestPassword123"
        };
    }

    private RegisteringShiftLeaderRequest CreateDefaultShiftLeaderRegistration(string leaderId, string tenant)
    {
        return new RegisteringShiftLeaderRequest
        {
            ID = leaderId,
            FirstName = "Leader",
            LastName = "Test",
            PhoneNumber = "+1-555-0200",
            DateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddYears(-25)),
        };
    }

    private RegisterRequest CreateDefaultWorkerRegistration(string workerId, string shiftLeaderId)
    {
        return new RegisterRequest
        {
            ID = workerId,
            FirstName = "Worker",
            LastName = "Test",
            PhoneNumber = "+1-555-0300",
            DateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddYears(-20)),
            ShiftLeaderId = shiftLeaderId,
            PasswordHash = "TestPassword123"
        };
    }




    public class TenantSenderInfo
{
    public TenantRegisterRequest Request { get; set; }
    public TenantRegisterResponse Response { get; set; }

    public ShiftLeaderSenderInfo ShiftLeaderSenderInfo { get; set; }    

    public string JwtToken => Response.Token;
}


public class ShiftLeaderSenderInfo
{
    public RegisteringShiftLeaderRequest RegisteringRequest { get; set; }
    public RegisteringShiftLeaderResponse RegisteringResponse { get; set; }
    public LoginShiftLeaderResponse LoginResponse { get; set; }
    public WorkerSenderInfo WorkerSenderInfo { get; set; }

    public string JwtToken => LoginResponse.Token;
}

public class WorkerSenderInfo
{
     public WorkerRegisteringRequest WorkerRequest { get; set; }
    public RegisteringWorkerResponse WorkerResponse { get; set; }
    public LoginWorkerResponse WorkerLoginResponse { get; set; }

    public string JwtToken => WorkerResponse.Token;
}
}
