using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Reqnroll;
using ShiftAssignerServer.Controllers;
using ShiftAssignerServer.Models.Stuff;
using ShiftAssignerServer.Requests;
using ShiftAssignerServer.Tests.Infrastructure;
using Xunit;

namespace ShiftAssignerServer.Tests.Steps;

[Binding]
public class RegisterBossTenantVerifySteps
{
    public const string HttpBaseurl = $"http://localhost:8080/";
    public const string Tenant_ID = "Acme ltd";
    public const string Worker_ID = "1111";


    private const string Payload_Context = "payload";
    private const string Response_Context = "response";
    private const string Tenants_Context = "tenants";
    private const string ShiftLeaders_Context = "shiftleaders";
    private const string ShiftLeaderID_Context = "shiftleaderId";
    private const string WorkersIDs_Context = "workersIds";
    private const string Workers_Context = "workers";

    private readonly ScenarioContext _scenarioContext;
    private readonly ClientSender _serverSender;

    public RegisterBossTenantVerifySteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
        _serverSender = new ClientSender(HttpBaseurl);


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

        _scenarioContext[Payload_Context] = payload;
    }

    [When("Tenant registration \"(.*)\"")]
    public async Task WhenIPostThePayloadTo(string tenantId)
    {
        const string registrationPath = $"api/v1/Auth/{AuthController.Register_Tenant}";

        var ptr = _scenarioContext[Payload_Context];

        var request = _scenarioContext[Payload_Context] as TenantRegisterRequest;
        request.ID = tenantId;

        var response = await _serverSender.PostCommandAsync<TenantRegisterRequest, TenantRegisterResponse>(request, registrationPath!);

        _scenarioContext[Response_Context] = response;
    }

    [Then("the response should contain a JWT token")]
    public void ThenTheResponseShouldContainJwtToken()
    {
        var response = _scenarioContext[Response_Context] as RegisterResponse;
        Assert.NotNull(response);
        Assert.True(!string.IsNullOrWhiteSpace(response!.Token));
    }

    [When("I GET the tenants list")]
    public async Task WhenIGetTheTenantsList()
    {
        var path = PathLocator.Combine("api/v1/Tenants");

        var response = await _serverSender.GetAsync<TenantResponse>(path);
        _scenarioContext[Tenants_Context] = response;
    }

    [Then("the tenants list should contain the tenant")]
    public void ThenTheTenantsListShouldContainCompany()
    {
        var response = _scenarioContext[Tenants_Context] as TenantResponse;
        var isContains = false;

        var payload = _scenarioContext[Payload_Context] as TenantRegisterRequest;
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
        var payload = new RegisterRequest
        {
            ID = leaderId,
            FirstName = "Bob",
            LastName = "Leader",
            PhoneNumber = "555-0200",
            DateOfBirth = new System.DateOnly(1990, 6, 1),
            PasswordHash = "P@ssw0rd!"
        };
        var tenant = (_scenarioContext[Payload_Context] as TenantRegisterRequest)?.Tenant;
        var registrationPath = PathLocator.Combine($"api/v1/Auth/register-shift-leader?tenant={tenant}");

        var response = await _serverSender.PostCommandAsync<RegisterRequest, RegisterResponse>(payload, registrationPath);
        _scenarioContext[Response_Context] = response;
        _scenarioContext[ShiftLeaderID_Context] = leaderId;
    }

    [When("I GET the shiftleaders")]
    public async Task WhenIGetTheShiftLeadersForTenant()
    {
        var tenant = (_scenarioContext[Payload_Context] as TenantRegisterRequest)?.Tenant;
        var path = PathLocator.Combine($"api/v1/ShiftLeaders/{tenant}");
        var response = await _serverSender.GetAsync<GetShiftLeaderPerTenantResponse>(path);
        _scenarioContext[ShiftLeaders_Context] = response;
    }

    [Then("the shiftleaders list should contain id \"(.*)\"")]
    public void ThenTheShiftLeadersListShouldContainId(string leaderId)
    {
        var response = _scenarioContext[ShiftLeaders_Context] as GetShiftLeaderPerTenantResponse;
        Assert.NotNull(response);
        var exists = false;

        foreach (var shiftLeader in response.ShifLeaders)
        {
            if (shiftLeader.ID.Equals(leaderId, System.StringComparison.InvariantCulture))
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
        var tenant = (_scenarioContext[Payload_Context] as TenantRegisterRequest)?.Tenant;
        var created = new List<string>();

        var shiftLeaderId = _scenarioContext[ShiftLeaderID_Context] as string;
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
            // store created id
            created.Add(id);
        }

        _scenarioContext[WorkersIDs_Context] = created;
    }

    [When("I GET the workers")]
    public async Task WhenIGetTheWorkers()
    {
        var leaderId = _scenarioContext[ShiftLeaderID_Context] as string;
        var path = PathLocator.Combine($"api/v1/Workers/leader/{leaderId}");
        var response = await _serverSender.GetAsync<GetWorkerPerTenantResponse>(path);
        _scenarioContext[Workers_Context] = response;
    }

    [Then("the workers list should contain the created workers")]
    public void ThenTheWorkersListShouldContainCreated()
    {
        var response = _scenarioContext[Workers_Context] as GetWorkerPerTenantResponse;
        Assert.NotNull(response);
        var created = _scenarioContext[WorkersIDs_Context] as List<string> ?? new List<string>();

        foreach (var id in created)
        {
            var found = false;
            foreach (var w in response.Workers)
            {
                if (w.ID.Equals(id, StringComparison.InvariantCulture))
                {
                    found = true;
                    break;
                }
            }

            Assert.True(found, $"Worker with id {id} was not found in workers list");
        }
    }
}
