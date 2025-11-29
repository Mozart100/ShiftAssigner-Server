using System;
using Reqnroll;
using ShiftAssignerServer.Requests;
using ShiftAssignerServer.Tests.Common;

namespace ShiftAssignerServer.Tests.Steps;

[Binding]
public class MultiTenantSteps : TwoTenantsStep
{
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