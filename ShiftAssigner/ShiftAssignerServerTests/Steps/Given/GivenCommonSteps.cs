using System;
using Reqnroll;
using ShiftAssignerServer.Requests;
using ShiftAssignerServer.Tests.Common;

namespace ShiftAssignerServer.Tests.Steps.Given;

[Binding]
public class GivenCommonSteps : TwoTenantsStep
{
    private const string TenantA_Context = "TenantA_Data";
    private const string TenantB_Context = "TenantB_Data";
    public const string Tenant_ID = "Acme ltd";

    public GivenCommonSteps(ScenarioContext scenarioContext) : base(scenarioContext)
    {
    }

    [Given("I have tenant registrations for \"(.*)\" and \"(.*)\"")]
    public void GivenIHaveTenantRegistrationsFor(string tenantAName, string tenantBName)
    {
        var tenantAPayload = new TenantRegisterRequest
        {
            ID = $"boss-{tenantAName}-{Guid.NewGuid().ToString("N").Substring(0, 8)}",
            FirstName = "Boss",
            LastName = "UserA",
            PhoneNumber = "555-0100",
            DateOfBirth = new DateOnly(1980, 1, 1),
            Tenant = $"{tenantAName}_{Guid.NewGuid()}",
            PasswordHash = "P@ssw0rd!"
        };

        var tenantBPayload = new TenantRegisterRequest
        {
            ID = $"boss-{tenantBName}-{Guid.NewGuid().ToString("N").Substring(0, 8)}",
            FirstName = "Boss",
            LastName = "UserB",
            PhoneNumber = "555-0200",
            DateOfBirth = new DateOnly(1985, 1, 1),
            Tenant = $"{tenantBName}_{Guid.NewGuid()}",
            PasswordHash = "P@ssw0rd!"
        };

        _scenarioContext[TenantA_Context] = tenantAPayload;
        _scenarioContext[TenantB_Context] = tenantBPayload;
    }

    [Given("I have a tenant boss registration payload")]
    public void GivenIHaveATenantBossRegistrationPayload()
    {
        var tenantName = $"{Tenant_ID}_{Guid.NewGuid()}";
        var payload = new TenantRegisterRequest
        {
            ID = "boss-verify-1",
            FirstName = "Alice",
            LastName = "Owner",
            PhoneNumber = "555-0100",
            DateOfBirth = new System.DateOnly(1985, 1, 1),
            Tenant = tenantName,
            PasswordHash = "P@ssw0rd!"
        };

        _scenarioContext[Tenant_Registration_Data_Context] = payload;
    }
}