using System.Net.Http;
using System.Reflection.Emit;
using System.Threading.Tasks;
using NuGet.Frameworks;
using Reqnroll;
using ShiftAssignerServer.Requests;
using ShiftAssignerServer.Tests.Infrastructure;
using Xunit;

namespace ShiftAssignerServer.Tests.Steps;

[Binding]
public class RegisterWorkerSteps
{
    public const string HttpBaseurl = $"http://localhost:8080/";

    private const string Payload_Context = "payload";
    private const string Response_Context = "response";

    private readonly ScenarioContext _scenarioContext;
    private readonly ClientSender _serverSender;

    // private WebApplicationFactory<Program>? _factory;

    public RegisterWorkerSteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
        _serverSender = new ClientSender(HttpBaseurl);

    }

    [Given("I have a worker registration payload")]
    public void GivenIHaveAWorkerRegistrationPayload()
    {
        var payload = new RegisterRequest
        {
            ID = "111",
            FirstName = "Test",
            LastName = "Worker",
            PhoneNumber = "555-0100",
            DateOfBirth = new System.DateOnly(1990, 1, 1),
            Tenant = "CompanyA",
            PasswordHash = "P@ssw0rd!"
        };


        // _payloadJson = JsonSerializer.Serialize(payload);
        _scenarioContext[Payload_Context] = payload;
    }

    [When("I POST the payload to \"(.*)\"")]
    public async Task WhenIPostThePayloadTo(string url)
    {
        const string registrationPath = @"api/v1/Auth/register-worker";

        var request = _scenarioContext[Payload_Context] as RegisterRequest;

        var response = await _serverSender.PostCommandAsync<RegisterRequest, RegisterResponse>(request, registrationPath);
        _scenarioContext[Response_Context] = response;
    }

    [Then("the response should contain a JWT token")]
    public async Task ThenTheResponseShouldContainAJWTToken()
    {
        var response = _scenarioContext[Response_Context] as RegisterResponse;
        Assert.True(response.Token.IsNotEmpty());
    }
}
