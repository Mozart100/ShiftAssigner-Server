using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Reqnroll;
using ShiftAssignerServer.Requests;
using ShiftAssignerServer.Tests.Common;
using ShiftAssignerServer.Tests.Infrastructure;
using Xunit;

namespace ShiftAssignerServer.Tests.Steps;

[Binding]
public class ReassignWorkerSteps : SingleTenantStep
{
    private const string CurrentLeaderId_Context = "currentLeaderId";
    private const string PeriodStart_Context = "periodStart";
    private const string ReassignWorkerResponse_Context = "ReassignWorkerResponse";
    private const string RetireWorkerResponse_Context = "RetireWorkerResponse";

    public ReassignWorkerSteps(ScenarioContext scenarioContext) : base(scenarioContext)
    {
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

    [Then("the workers list should contain {int} worker(s)")]
    public void ThenTheWorkersListShouldContainWorkers(int expectedCount)
    {
        var response = _scenarioContext[All_Workers_Context] as GetWorkerPerTenantResponse;
        Assert.NotNull(response);
        Assert.Equal(expectedCount, response.Workers.Count());
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

    [Then("the retire response should be successful")]
    public void ThenTheRetireResponseShouldBeSuccessful()
    {
        var response = _scenarioContext[RetireWorkerResponse_Context];
        Assert.NotNull(response);
    }
}
