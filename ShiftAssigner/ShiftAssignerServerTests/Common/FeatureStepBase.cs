using Reqnroll;
using ShiftAssignerServer.Tests.Infrastructure;

namespace ShiftAssignerServer.Tests.Common;

public class FeatureStepBase
{
    public const string HttpBaseurl = $"http://localhost:8080/";
    protected readonly ScenarioContext _scenarioContext;
    protected readonly ClientSender _serverSender;

    public FeatureStepBase(ScenarioContext scenarioContext)
    {

        _scenarioContext = scenarioContext;
        _serverSender = new ClientSender(HttpBaseurl);
        
    }
    
}
