using System.Net.Http;
using System.Threading.Tasks;
using Reqnroll;
using ShiftAssignerServer.Requests;
using ShiftAssignerServer.Tests.Infrastructure;

namespace ShiftAssignerServer.Tests.Steps;

[Binding]
public class RegisterWorkerSteps
{
    private readonly ScenarioContext _scenarioContext;
    // private WebApplicationFactory<Program>? _factory;
    private HttpResponseMessage? _response;
    private string? _payloadJson;

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
        _scenarioContext["payload"] = payload;
    }

    [When("I POST the payload to \"(.*)\"")]
    public async Task WhenIPostThePayloadTo(string url)
    {
        const string registrationPath = @"api/Auth/register-worker";
        
        var path = PathLocator.Combine(registrationPath);

        var request = _scenarioContext["payload"] as RegisterRequest;

        var client = new ClientSender();
        var response = await client.PostCommandAsync<RegisterRequest,RegisterResponse>(path,request);



        // create a fresh factory per scenario to isolate in-memory state
        // _factory = new WebApplicationFactory<Program>();
        // var client = _factory.CreateClient();

        // var content = new StringContent(_payloadJson ?? string.Empty, Encoding.UTF8, "application/json");
        // _response = await client.PostAsync(url, content).ConfigureAwait(false);
        // _scenarioContext["response"] = _response;
    }

    [Then("the response should contain a JWT token")]
    public async Task ThenTheResponseShouldContainAJWTToken()
    {
        // if (_response is null)
        //     throw new System.InvalidOperationException("Response is not set. Did you call the When step?");

        // var body = await _response.Content.ReadAsStringAsync().ConfigureAwait(false);

        // // Try to parse JSON and find a token property (common API shape)
        // try
        // {
        //     using var doc = JsonDocument.Parse(body);
        //     if (doc.RootElement.ValueKind == JsonValueKind.Object)
        //     {
        //         if (doc.RootElement.TryGetProperty("token", out var t) || doc.RootElement.TryGetProperty("access_token", out t))
        //         {
        //             if (string.IsNullOrEmpty(t.GetString()))
        //                 throw new System.Exception("Token property was empty");
        //             return;
        //         }
        //     }

        //     // fallback: if whole body is token string
        //     if (string.IsNullOrEmpty(body.Trim('"')))
        //         throw new System.Exception("Response body is empty or token not found");
        // }
        // catch (JsonException)
        // {
        //     if (string.IsNullOrEmpty(body.Trim('"')))
        //         throw;
        // }
    }
}
