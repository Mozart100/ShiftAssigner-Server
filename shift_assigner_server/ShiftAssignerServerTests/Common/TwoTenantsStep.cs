using Reqnroll;

namespace ShiftAssignerServer.Tests.Common;

public class TwoTenantsStep : FeatureStepBase
{
    
    protected const string Tenant_Registration_Data_Context = "payload";
    protected const string Tenant_Registration_Response_Context = "response";

    protected const string Workers_Registration_Data_Context = "workersRegistrationData";
    protected const string Workers_Registration_Responses_Context = "workersRegistrationResponses";

    public const string First_Worker_ID_Starts = "1111";
    public const string Second_Worker_ID_Starts = "2222";


    protected const string All_Tenants_Context = "tenants";

    protected const string All_ShiftLeaders_Context = "shiftLeaders";

    protected const string All_Workers_Context = "workers";





    public TwoTenantsStep(ScenarioContext scenarioContext) : base(scenarioContext)
    {
    }
}
