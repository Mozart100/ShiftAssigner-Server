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


    private const string Payload_Context = "payload";
    private const string Response_Context = "response";
    private const string Tenants_Context = "tenants";
    private const string ShiftLeaders_Context = "shiftleaders";

    private readonly ScenarioContext _scenarioContext;
    private readonly ClientSender _serverSender;

    public RegisterBossTenantVerifySteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
        _serverSender = new ClientSender(HttpBaseurl);

    }

    [Given("I have a tenant boss registration payload for tenant \"(.*)\"")]
    public void GivenIHaveATenantBossRegistrationPayload(string tenant)
    {
        var payload = new TenantRegisterRequest
        {
            ID = "boss-verify-1",
            FirstName = "Alice",
            LastName = "Owner",
            PhoneNumber = "555-0100",
            DateOfBirth = new System.DateOnly(1985, 1, 1),
            Tenant = tenant,
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
        // httpResponse.EnsureSuccessStatusCode();

        // var responseContent = await httpResponse.Content.ReadAsStringAsync();
        // var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        // var tenants = JsonSerializer.Deserialize<List<TenantResponse>>(responseContent, options) ?? new List<TenantResponse>();

        _scenarioContext[Tenants_Context] = response;
    }

    [Then("the tenants list should contain tenant \"(.*)\"")]
    public void ThenTheTenantsListShouldContainCompany(string tenant)
    {
        var response = _scenarioContext[Tenants_Context] as TenantResponse;
        var isContains = false;

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

    [When("I create a shift leader for tenant \"(.*)\" with id \"(.*)\"")]
    public async Task WhenICreateAShiftLeaderForTenant(string tenant, string leaderId)
    {
        var payload = new RegisterRequest
        {
            ID = leaderId,
            FirstName = "Bob",
            LastName = "Leader",
            PhoneNumber = "555-0200",
            DateOfBirth = new System.DateOnly(1990, 6, 1),
            Tenant = tenant,
            PasswordHash = "P@ssw0rd!"
        };

        const string registrationPath = "api/v1/Auth/register-shift-leader";

        var response = await _serverSender.PostCommandAsync<RegisterRequest, RegisterResponse>(payload, registrationPath);
        _scenarioContext[Response_Context] = response;
    }

    [When("I GET the shiftleaders for tenant \"(.*)\"")]
    public async Task WhenIGetTheShiftLeadersForTenant(string tenant)
    {
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
}
