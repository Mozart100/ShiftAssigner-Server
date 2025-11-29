using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Reqnroll;
using ShiftAssignerServer.Requests;
using ShiftAssignerServer.Tests.Common;
using ShiftAssignerServer.Tests.Infrastructure;
using Xunit;

namespace ShiftAssignerServer.Tests.Steps;

[Binding]
public class ReassignWorkerSteps : SingleTenantStep
{
    private const string CurrentLeaderId_Context = "currentLeaderId";
    private const string PeriodStart_Context = "periodStart";
    private const string ReassignWorkerResponse_Context = "ReassignWorkerResponse";
    private const string RetireWorkerResponse_Context = "RetireWorkerResponse";

    public ReassignWorkerSteps(ScenarioContext scenarioContext) : base(scenarioContext)
    {
    }

    // These are specific reassignment feature methods not shared with other features
    // They should remain here rather than in common steps
}
