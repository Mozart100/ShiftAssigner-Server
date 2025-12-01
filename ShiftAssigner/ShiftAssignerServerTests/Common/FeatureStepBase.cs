using Reqnroll;
using ShiftAssignerServer.Tests.Infrastructure;

namespace ShiftAssignerServer.Tests.Common;

public class FeatureStepBase
{
//https://localhost:7083/api/v1/Auth/register-boss-tenant'
    public const string HttpBaseurl = $"http://localhost:7083/";
    protected readonly ScenarioContext _scenarioContext;
    protected readonly ClientSender _serverSender;

    public FeatureStepBase(ScenarioContext scenarioContext)
    {

        _scenarioContext = scenarioContext;
        _serverSender = new ClientSender(HttpBaseurl);
        
    }
    
}
