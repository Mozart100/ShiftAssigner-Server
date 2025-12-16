using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Reqnroll;
using ShiftAssignerServer.Controllers;
using ShiftAssignerServer.Models.WorkerScheduling;
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

    [Given(@"I have shift configurations for tenants:")]
    public void GivenIHaveShiftConfigurationsForTenants(Table table)
    {
        var multiTenantData = _scenarioContext.Get<MultiTenantTestData>(MultiTenant_Data_Context);

        // Group shifts by tenant
        var shiftsByTenant = table.Rows.GroupBy(row => row["TenantId"]);

        foreach (var tenantGroup in shiftsByTenant)
        {
            var tenantId = tenantGroup.Key;
            var shiftConfig = new TenantShiftConfig
            {
                IsActive = true,
                Shifts = new List<TenantShiftConfig.ShiftInfo>()
            };

            foreach (var row in tenantGroup)
            {
                shiftConfig.Shifts.Add(new TenantShiftConfig.ShiftInfo
                {
                    ShiftName = row["ShiftName"],
                    MinimumAmountOfWorkers = int.Parse(row["MinWorkers"]),
                    MaximumAmountOfWorkers = int.Parse(row["MaxWorkers"])
                });
            }

            multiTenantData.Tenants[tenantId] = new TenantInfo { TenantId = tenantId };
            multiTenantData.Tenants[tenantId].ShiftConfigForRegistration = shiftConfig;
        }
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

        tenantInfo.ShiftLeaders[leaderId] = new ShiftLeaderInfo
        {
            LeaderId = leaderId,
            LeaderRequest = leaderRequest,
            LeaderResponse = leaderResponse
        };
    }

    [Then(@"the shiftleader ""(.*)"" registration for tenant ""(.*)"" should contain a JWT token")]
    public void ThenTheShiftleaderRegistrationForTenantShouldContainAJWTToken(string leaderId, string tenantId)
    {
        var multiTenantData = _scenarioContext.Get<MultiTenantTestData>(MultiTenant_Data_Context);
        var tenantInfo = multiTenantData.Tenants[tenantId];

        // Get the specific shift leader's registration response
        var shiftLeader = tenantInfo.ShiftLeaders[leaderId];
        Assert.NotNull(shiftLeader);
        Assert.NotNull(shiftLeader.LeaderResponse);
        Assert.NotNull(shiftLeader.LeaderResponse.Token);
        Assert.NotEmpty(shiftLeader.LeaderResponse.Token);
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

        tenantInfo.ShiftLeaders[leaderId].LeaderLoginResponse = loginResponse;
    }

    [Then(@"the shiftleader ""(.*)"" login for tenant ""(.*)"" should contain a JWT token")]
    public void ThenTheShiftleaderLoginForTenantShouldContainAJWTToken(string leaderId, string tenantId)
    {
        var multiTenantData = _scenarioContext.Get<MultiTenantTestData>(MultiTenant_Data_Context);
        var tenantInfo = multiTenantData.Tenants[tenantId];

        // Get the specific shift leader's login response
        var shiftLeader = tenantInfo.ShiftLeaders[leaderId];
        Assert.NotNull(shiftLeader);
        Assert.NotEmpty(shiftLeader.ShiftLeaderToken);
    }

    [When(@"shiftleader ""(.*)"" registers worker ""(.*)"" for tenant ""(.*)"" in multi tenant flow")]
    public async Task WhenShiftleaderRegistersWorkerForTenantInMultiTenantFlow(string leaderId, string workerId, string tenantId)
    {
        var multiTenantData = _scenarioContext.Get<MultiTenantTestData>(MultiTenant_Data_Context);
        var tenantInfo = multiTenantData.Tenants[tenantId];

        var workerRequest = CreateWorkerRegistration(workerId);

        // Use the specific shift leader's token for worker registration
        var shiftLeader = tenantInfo.ShiftLeaders[leaderId];
        var workerResponse = await _serverSender.PostCommandAsync<WorkerRegisteringRequest, RegisteringWorkerResponse>(
            WORKERS_REGISTER,
            workerRequest, shiftLeader.LeaderLoginResponse.Token);

        tenantInfo.Workers[workerId] = new WorkerInfo
        {
            WorkerId = workerId,
            WorkerRequest = workerRequest,
            WorkerResponse = workerResponse,
            AssignedToShiftLeaderId = leaderId  // Track which shift leader registered this worker
        };

        // Automatically verify worker assignment after registration
        // await VerifyWorkerAssignment(leaderId, workerId, tenantId);
    }

    // private async Task VerifyWorkerAssignment(string leaderId, string workerId, string tenantId)
    // {
    //     var multiTenantData = _scenarioContext.Get<MultiTenantTestData>(MultiTenant_Data_Context);
    //     var tenantInfo = multiTenantData.Tenants[tenantId];

    //     try
    //     {
    //         var endpoint = string.Format(STUFF_BOOKINGS_SHIFTLEADER_WORKERS, leaderId);

    //         // Use the specific shift leader's token for verification
    //         var shiftLeader = tenantInfo.ShiftLeaders[leaderId];
    //         var shiftLeaderWithWorkers = await _serverSender.GetAsync<GetWorkerPerShiftLeaderResponse>(
    //             endpoint, shiftLeader.LeaderLoginResponse.Token);

    //         Assert.NotNull(shiftLeaderWithWorkers);
    //         Assert.Equal(leaderId, shiftLeaderWithWorkers.ShiftLeaderID);
    //         Assert.NotEmpty(shiftLeaderWithWorkers.Workers);

    //         var assignedWorker = shiftLeaderWithWorkers.Workers.FirstOrDefault(w => w.ID == workerId);
    //         Assert.NotNull(assignedWorker);
    //         Assert.Equal(workerId, assignedWorker.ID);

    //         // Log successful verification
    //         Console.WriteLine($"✅ Verified: Worker {workerId} is correctly assigned to shift leader {leaderId} in tenant {tenantId}");
    //     }
    //     catch (Exception ex)
    //     {
    //         // Log the verification attempt but don't fail the test - this is additional verification
    //         Console.WriteLine($"⚠️  Worker assignment verification failed for {workerId} -> {leaderId} in tenant {tenantId}: {ex.Message}");
    //     }
    // }

    [Then(@"I verify that shiftleader ""(.*)"" has worker ""(.*)"" assigned for tenant ""(.*)""")]
    public async Task ThenIVerifyThatShiftleaderHasWorkerAssignedForTenant(string leaderId, string workerId, string tenantId)
    {
        var multiTenantData = _scenarioContext.Get<MultiTenantTestData>(MultiTenant_Data_Context);
        var tenantInfo = multiTenantData.Tenants[tenantId];

        var endpoint = string.Format(STUFF_BOOKINGS_SHIFTLEADER_WORKERS, leaderId);

        // Use the specific shift leader's token for verification
        var shiftLeader = tenantInfo.ShiftLeaders[leaderId];
        var shiftLeaderWithWorkers = await _serverSender.GetAsync<GetWorkerPerShiftLeaderResponse>(
            endpoint, shiftLeader.LeaderLoginResponse.Token);

        Assert.NotNull(shiftLeaderWithWorkers);
        Assert.Equal(leaderId, shiftLeaderWithWorkers.ShiftLeaderID);
        Assert.NotNull(shiftLeaderWithWorkers.Workers);

        var assignedWorker = shiftLeaderWithWorkers.Workers.FirstOrDefault(w => w.ID == workerId);
        Assert.NotNull(assignedWorker);
        Assert.Equal(workerId, assignedWorker.ID);
    }

    [Then(@"the worker registration for tenant ""(.*)"" should contain a JWT token")]
    public void ThenTheWorkerRegistrationForTenantShouldContainAJWTToken(string tenantId)
    {
        var multiTenantData = _scenarioContext.Get<MultiTenantTestData>(MultiTenant_Data_Context);
        var tenantInfo = multiTenantData.Tenants[tenantId];

        // Get the most recently added worker
        var latestWorker = tenantInfo.Workers.Values.LastOrDefault();
        Assert.NotNull(latestWorker);
        Assert.NotNull(latestWorker.WorkerResponse);
        Assert.NotNull(latestWorker.WorkerResponse.Token);
        Assert.NotEmpty(latestWorker.WorkerResponse.Token);
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

        // Use the shift leader token who registered this worker
        var worker = tenantInfo.Workers[workerId];
        var assignedShiftLeader = tenantInfo.ShiftLeaders[worker.AssignedToShiftLeaderId];

        var workerLoginResponse = await _serverSender.PostCommandAsync<LoginWorkerRequest, LoginWorkerResponse>(
            WORKERS_LOGIN,
            workerLoginRequest, assignedShiftLeader.LeaderLoginResponse.Token);

        worker.WorkerLoginResponse = workerLoginResponse;
    }

    [Then(@"the worker ""(.*)"" login for tenant ""(.*)"" should contain a JWT token")]
    public void ThenTheWorkerLoginForTenantShouldContainAJWTToken(string workerId, string tenantId)
    {
        var multiTenantData = _scenarioContext.Get<MultiTenantTestData>(MultiTenant_Data_Context);
        var tenantInfo = multiTenantData.Tenants[tenantId];

        // Get the specific worker's login response
        var worker = tenantInfo.Workers[workerId];
        Assert.NotNull(worker);
        Assert.NotNull(worker.WorkerLoginResponse);
        Assert.NotNull(worker.WorkerLoginResponse.Token);
        Assert.NotEmpty(worker.WorkerLoginResponse.Token);
    }

    [When(@"shift leader ""(.*)"" reassigns worker ""(.*)"" to shift leader ""(.*)"" for tenant ""(.*)"" in multi tenant flow")]
    public async Task WhenShiftLeaderReassignsWorkerToShiftLeaderForTenantInMultiTenantFlow(string fromLeaderId, string workerId, string toLeaderId, string tenantId)
    {
        var multiTenantData = _scenarioContext.Get<MultiTenantTestData>(MultiTenant_Data_Context);
        var tenantInfo = multiTenantData.Tenants[tenantId];

        var reassignRequest = new ReassignWorkerRequest
        {
            WorkerIds = new List<string> { workerId },
            ReassignToShiftLeaderId = toLeaderId,
            Notes = $"Reassigning {workerId} from {fromLeaderId} to {toLeaderId} for testing"
        };

        // Use the fromLeader's token to perform the reassignment
        var fromShiftLeader = tenantInfo.ShiftLeaders[fromLeaderId];

        var reassignResponse = await _serverSender.PostCommandAsync<ReassignWorkerRequest, ReassignWorkerResponse>(
            $"/api/v1/{StuffBookingsControllerName}/reassign",
            reassignRequest, fromShiftLeader.ShiftLeaderToken);

        // Update the worker's assigned shift leader in our test data
        var worker = tenantInfo.Workers[workerId];
        worker.AssignedToShiftLeaderId = toLeaderId;

        // Store the reassign response if needed for future assertions
        _scenarioContext["LastReassignResponse"] = reassignResponse;
    }

    [Then(@"shift leader ""(.*)"" should have ""(.*)"" workers assigned for tenant ""(.*)""")]
    public async Task ThenShiftLeaderShouldHaveWorkersAssignedForTenant(string leaderId, string expectedWorkerCount, string tenantId)
    {
        var multiTenantData = _scenarioContext.Get<MultiTenantTestData>(MultiTenant_Data_Context);
        var tenantInfo = multiTenantData.Tenants[tenantId];

        var endpoint = string.Format(STUFF_BOOKINGS_SHIFTLEADER_WORKERS, leaderId);

        // Use the shift leader's token for verification
        var shiftLeader = tenantInfo.ShiftLeaders[leaderId];
        var shiftLeaderWithWorkers = await _serverSender.GetAsync<GetWorkerPerShiftLeaderResponse>(
            endpoint, shiftLeader.ShiftLeaderToken);

        Assert.NotNull(shiftLeaderWithWorkers);
        Assert.Equal(leaderId, shiftLeaderWithWorkers.ShiftLeaderID);
        Assert.NotNull(shiftLeaderWithWorkers.Workers);

        var actualWorkerCount = shiftLeaderWithWorkers.Workers.Count();
        var expected = int.Parse(expectedWorkerCount);

        Assert.Equal(expected, actualWorkerCount);
    }

    // Helper methods for creating test data
    private TenantRegisterRequest CreateTenantRegistration(string tenantId)
    {
        var multiTenantData = _scenarioContext.Get<MultiTenantTestData>(MultiTenant_Data_Context);

        return new TenantRegisterRequest
        {
            ID = $"boss-{tenantId}",
            FirstName = "Boss",
            LastName = $"Tenant{tenantId}",
            PhoneNumber = $"+1-555-010{tenantId}",
            DateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddYears(-30)),
            Tenant = $"MultiTenant_Company{tenantId}",
            PasswordHash = "BossPassword123",
            ShiftConfig = multiTenantData.Tenants[tenantId].ShiftConfigForRegistration
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

    private WorkerRegisteringRequest CreateWorkerRegistration(string workerId)
    {
        return new WorkerRegisteringRequest
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
    public Dictionary<string, ShiftLeaderInfo> ShiftLeaders { get; set; } = new Dictionary<string, ShiftLeaderInfo>();
    public Dictionary<string, WorkerInfo> Workers { get; set; } = new Dictionary<string, WorkerInfo>();
    public TenantShiftConfig ShiftConfigForRegistration { get; set; } = new TenantShiftConfig();
    public string TenantToken => TenantResponse.Token;

    public TenantShiftConfig ShiftConfig => TenantRequest.ShiftConfig;
}

public class ShiftLeaderInfo
{
    public string LeaderId { get; set; } = string.Empty;
    public RegisteringShiftLeaderRequest LeaderRequest { get; set; } = new RegisteringShiftLeaderRequest();
    public RegisteringShiftLeaderResponse LeaderResponse { get; set; } = new RegisteringShiftLeaderResponse();
    public LoginShiftLeaderResponse LeaderLoginResponse { get; set; } = new LoginShiftLeaderResponse();

    public string ShiftLeaderToken => LeaderLoginResponse.Token;
}

public class WorkerInfo
{
    public string WorkerId { get; set; } = string.Empty;
    public string AssignedToShiftLeaderId { get; set; } = string.Empty;
    public WorkerRegisteringRequest WorkerRequest { get; set; } = new WorkerRegisteringRequest();
    public RegisteringWorkerResponse WorkerResponse { get; set; } = new RegisteringWorkerResponse();
    public LoginWorkerResponse WorkerLoginResponse { get; set; } = new LoginWorkerResponse();
}