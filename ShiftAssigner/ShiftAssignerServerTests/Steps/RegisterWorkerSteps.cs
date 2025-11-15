using System.Net.Http;
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
    private const string Payload_Context = "payload";
    private const string Response_Context = "response";

    private readonly ScenarioContext _scenarioContext;
    // private WebApplicationFactory<Program>? _factory;

    public RegisterWorkerSteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [Given("I have a worker registration payload")]
    public void GivenIHaveAWorkerRegistrationPayload()
    {
        var payload = new RegisterRequest
        {
            ID="111", 
            FirstName = "Test",
            LastName = "Worker",
            PhoneNumber = "555-0100",
            DateOfBirth = new System.DateOnly(1990,1,1),
            Tenant = "CompanyA",
            Password = "P@ssw0rd!"
        };

        
        // _payloadJson = JsonSerializer.Serialize(payload);
        _scenarioContext[Payload_Context] = payload;
    }

    [When("I POST the payload to \"(.*)\"")]
    public async Task WhenIPostThePayloadTo(string url)
    {
        const string registrationPath = @"api/Auth/register-worker";
        
        var path = PathLocator.Combine(registrationPath);

        var request = _scenarioContext[Payload_Context] as RegisterRequest;

        var client = new ClientSender();
        var response = await client.PostCommandAsync<RegisterRequest,RegisterResponse>(path,request);


        _scenarioContext[Response_Context] = response;

        // create a fresh factory per scenario to isolate in-memory state
        // _factory = new WebApplicationFactory<Program>();
        // var client = _factory.CreateClient();

        // _response = await client.PostAsync(url, content).ConfigureAwait(false);
        // var content = new StringContent(_payloadJson ?? string.Empty, Encoding.UTF8, "application/json");
    }

    [Then("the response should contain a JWT token")]
    public async Task ThenTheResponseShouldContainAJWTToken()
    {
        var  response = _scenarioContext[Response_Context] as RegisterResponse;
        Assert.True(response.Token.IsNotEmpty());
    }
}
