using Reqnroll;
using ShiftAssignerServer.Tests.Infrastructure;
using ShiftAssignerServer.Controllers;

namespace ShiftAssignerServer.Tests.Common;

public class FeatureStepBase
{
    //https://localhost:7083/api/v1/Auth/register-boss-tenant'
    public const string HttpBaseurl = $"https://localhost:7083/";
    
    // Endpoint Constants
    protected const string AUTH_REGISTER_BOSS_TENANT = "/api/v1/Auth/register-boss-tenant";
    protected const string SHIFT_LEADERS_REGISTER = "/api/v1/ShiftLeaders/register";
    protected const string SHIFT_LEADERS_LOGIN = "/api/v1/ShiftLeaders/login";
    protected const string WORKERS_REGISTER = "/api/v1/Workers/register";
    protected const string WORKERS_LOGIN = "/api/v1/Workers/login";

    
    protected readonly ScenarioContext _scenarioContext;
    protected readonly ClientSender _serverSender;

    public FeatureStepBase(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
        _serverSender = new ClientSender(HttpBaseurl);
    }


    protected static readonly string STUFF_BOOKINGS_SHIFTLEADER_WORKERS = $"/api/v1/{StuffBookingsControllerName}/shiftleader/{{0}}/workers";


    public static string StuffBookingsControllerName=>nameof(StuffBookingsController).Replace("Controller", "");

}
