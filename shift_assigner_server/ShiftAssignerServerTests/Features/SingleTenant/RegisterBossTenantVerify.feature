Feature: Register Tenant Boss and verify tenant exists
  In order to ensure tenant creation
  As a client of the API
  I want to register a tenant boss and then GET the tenants list to verify the company exists

  @integration @registration @single-tenant
  Scenario: Register boss and verify tenant appears in tenants list
  Given I have a tenant boss registration payload for basic flow
  When Tenant registration "1" for basic flow
  Then the tenant registration response should contain a JWT token
    When I register a shiftleader with id "leader-1" for basic flow
    Then the shiftleader registration response should contain a JWT token
    When the shift leader "leader-1" logs in for basic flow
    Then the shiftleader login response should contain a JWT token
    When the shift leader registers a worker with id "worker-1" for basic flow
    Then the worker registration response should contain a JWT token
    When the worker "worker-1" logs in for basic flow
    Then the worker login response should contain a JWT token