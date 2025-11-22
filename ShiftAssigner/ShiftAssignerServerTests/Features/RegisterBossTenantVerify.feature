Feature: Register Tenant Boss and verify tenant exists
  In order to ensure tenant creation
  As a client of the API
  I want to register a tenant boss and then GET the tenants list to verify the company exists

  @integration @registration
  Scenario: Register boss and verify tenant appears in tenants list
  Given I have a tenant boss registration payload
  When Tenant registration "1"
  Then the response should contain a JWT token
    When I GET the tenants list
    Then the tenants list should contain the tenant
    When I create a shift leader with id "leader-1"
    Then the response should contain a JWT token
  When I GET the shiftleaders
  Then the shiftleaders list should contain id "leader-1"
  When the shift leader creates 2 workers
  When I GET the workers
  Then the workers list should contain the created workers
