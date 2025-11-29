using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Reqnroll;
using ShiftAssignerServer.Controllers;
using ShiftAssignerServer.Models.Stuff;
using ShiftAssignerServer.Requests;
using ShiftAssignerServer.Tests.Common;
using ShiftAssignerServer.Tests.Infrastructure;
using Xunit;

namespace ShiftAssignerServer.Tests.Steps;


[Binding]
public class RegisterBossTenantVerifySteps : SingleTenantStep
{
    public const string Tenant_ID = "Acme ltd";
    private const string CurrentLeaderId_Context = "currentLeaderId";


    public RegisterBossTenantVerifySteps(ScenarioContext scenarioContext):base(scenarioContext)
    {
    }
}
