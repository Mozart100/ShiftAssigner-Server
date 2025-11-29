using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
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
public class MultiTenantSteps : TwoTenantsStep
{
    private const string TenantA_Context = "TenantA_Data";
    private const string TenantB_Context = "TenantB_Data";
    private const string TenantA_Response_Context = "TenantA_Response";
    private const string TenantB_Response_Context = "TenantB_Response";
    private const string TenantA_Leaders_Context = "TenantA_Leaders";
    private const string TenantB_Leaders_Context = "TenantB_Leaders";
    private const string TenantA_Workers_Context = "TenantA_Workers";
    private const string TenantB_Workers_Context = "TenantB_Workers";
    
    // Additional context keys for worker registration validation
    private const string WorkerRegistrationResponse_Context = "WorkerRegistrationResponse";
    private const string RegistrationException_Context = "RegistrationException";
    private const string HttpResponse_Context = "HttpResponse";
    private const string ErrorResponseBody_Context = "ErrorResponseBody";

    public MultiTenantSteps(ScenarioContext scenarioContext) : base(scenarioContext)
    {
    }



    // Helper method
    private RegisterRequest BuildRegisterRequestFromTable(Table table, bool generateUniqueId = false)
    {
        var request = new RegisterRequest();
        
        foreach (var row in table.Rows)
        {
            var field = row["Field"];
            var value = row["Value"];
            
            switch (field)
            {
                case "ID":
                    if (generateUniqueId && !string.IsNullOrWhiteSpace(value))
                    {
                        request.ID = $"{value}_{Guid.NewGuid().ToString("N").Substring(0, 8)}";
                    }
                    else
                    {
                        request.ID = value;
                    }
                    break;
                case "FirstName":
                    request.FirstName = value;
                    break;
                case "LastName":
                    request.LastName = value;
                    break;
                case "PhoneNumber":
                    request.PhoneNumber = value;
                    break;
                case "DateOfBirth":
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        request.DateOfBirth = DateOnly.Parse(value);
                    }
                    break;
                case "PasswordHash":
                    request.PasswordHash = value;
                    break;
                case "ShiftLeaderId":
                    request.ShiftLeaderId = value;
                    break;
            }
        }
        
        return request;
    }
}