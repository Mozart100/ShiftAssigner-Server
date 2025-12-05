Feature: Register Tenant Boss and verify tenant exists
  In order to ensure tenant creation
  As a client of the API
  I want to register a tenant boss and then GET the tenants list to verify the company exists

  @integration @registration
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

  @integration @registration @multi-tenant
  Scenario: Register two tenants with shift leaders and workers
  Given I have tenant boss registration payloads for multi tenant flow
  When I register tenant "1" for multi tenant flow
  Then the tenant "1" registration response should contain a JWT token
  When I register tenant "2" for multi tenant flow  
  Then the tenant "2" registration response should contain a JWT token
  When I register shiftleader "leader-1" for tenant "1" in multi tenant flow
  Then the shiftleader registration for tenant "1" should contain a JWT token
  When I register shiftleader "leader-2" for tenant "2" in multi tenant flow
  Then the shiftleader registration for tenant "2" should contain a JWT token
  When shiftleader "leader-1" logs in for tenant "1" in multi tenant flow
  Then the shiftleader login for tenant "1" should contain a JWT token
  When shiftleader "leader-2" logs in for tenant "2" in multi tenant flow
  Then the shiftleader login for tenant "2" should contain a JWT token
  When shiftleader "leader-1" registers worker "worker-1" for tenant "1" in multi tenant flow
  Then the worker registration for tenant "1" should contain a JWT token
  When shiftleader "leader-2" registers worker "worker-2" for tenant "2" in multi tenant flow
  Then the worker registration for tenant "2" should contain a JWT token
  When worker "worker-1" logs in for tenant "1" in multi tenant flow
  Then the worker login for tenant "1" should contain a JWT token
  When worker "worker-2" logs in for tenant "2" in multi tenant flow
  Then the worker login for tenant "2" should contain a JWT token