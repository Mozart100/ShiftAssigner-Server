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


public class SingleTenantStep : FeatureStepBase
{
    
    protected const string Tenant_Registration_Data_Context = "payload";
    protected const string Tenant_Registration_Response_Context = "response";

    protected const string Workers_Registration_Data_Context = "workersRegistrationData";
    protected const string Workers_Registration_Responses_Context = "workersRegistrationResponses";

    public const string First_Worker_ID_Starts = "1111";
    public const string Second_Worker_ID_Starts = "2222";


    protected const string All_Tenants_Context = "tenants";

    protected const string All_ShiftLeaders_Context = "shiftleaders";

    protected const string All_Workers_Context = "workers";





    public SingleTenantStep(ScenarioContext scenarioContext) : base(scenarioContext)
    {
    }
}
