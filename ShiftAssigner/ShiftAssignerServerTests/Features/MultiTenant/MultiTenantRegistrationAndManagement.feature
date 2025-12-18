Feature: Multi-Tenant Registration and Worker Management
  In order to ensure multi-tenant functionality works correctly
  As a client of the API
  I want to register multiple tenants with their shift leaders and workers, and test worker reassignment

  @integration @registration @multi-tenant
  Scenario: Register two tenants with shift leaders and workers
  Given I have tenant boss registration payloads for multi tenant flow
  And I have shift configurations for tenants:
    | TenantId | ShiftName | MinWorkers | MaxWorkers |
    | 1        | Morning   | 2          | 5          |
    | 1        | Night     | 1          | 2          |
    | 2        | Morning   | 2          | 4          |
    | 2        | Day       | 3          | 6          |
    | 2        | Night     | 1          | 2          |
  
  When I register tenant "1" for multi tenant flow
  Then the tenant "1" registration response should contain a JWT token

  When I register tenant "2" for multi tenant flow  
  Then the tenant "2" registration response should contain a JWT token
  
  When I register shiftleader "leader-1" for tenant "1" in multi tenant flow
  Then the shiftleader "leader-1" registration for tenant "1" should contain a JWT token
  
  When I register shiftleader "leader-2" for tenant "1" in multi tenant flow
  Then the shiftleader "leader-2" registration for tenant "1" should contain a JWT token
  
  When I register shiftleader "leader-3" for tenant "2" in multi tenant flow
  Then the shiftleader "leader-3" registration for tenant "2" should contain a JWT token
  
  When shiftleader "leader-1" logs in for tenant "1" in multi tenant flow
  Then the shiftleader "leader-1" login for tenant "1" should contain a JWT token
  
  When shiftleader "leader-2" logs in for tenant "1" in multi tenant flow
  Then the shiftleader "leader-2" login for tenant "1" should contain a JWT token
  
  When shiftleader "leader-3" logs in for tenant "2" in multi tenant flow
  Then the shiftleader "leader-3" login for tenant "2" should contain a JWT token
  
  When shiftleader "leader-1" registers worker "worker-1" for tenant "1" in multi tenant flow
  Then the worker registration for tenant "1" should contain a JWT token
  Then I verify that shiftleader "leader-1" has worker "worker-1" assigned for tenant "1"
  
  When shiftleader "leader-1" registers worker "worker-4" for tenant "1" in multi tenant flow
  Then the worker registration for tenant "1" should contain a JWT token
  Then I verify that shiftleader "leader-1" has worker "worker-4" assigned for tenant "1"
  
  When shiftleader "leader-1" registers worker "worker-5" for tenant "1" in multi tenant flow
  Then the worker registration for tenant "1" should contain a JWT token
  Then I verify that shiftleader "leader-1" has worker "worker-5" assigned for tenant "1"
  
  When shiftleader "leader-2" registers worker "worker-2" for tenant "1" in multi tenant flow
  Then the worker registration for tenant "1" should contain a JWT token
  Then I verify that shiftleader "leader-2" has worker "worker-2" assigned for tenant "1"
  
  When shiftleader "leader-3" registers worker "worker-3" for tenant "2" in multi tenant flow
  Then the worker registration for tenant "2" should contain a JWT token
  Then I verify that shiftleader "leader-3" has worker "worker-3" assigned for tenant "2"
  
  When worker "worker-1" logs in for tenant "1" in multi tenant flow
  Then the worker "worker-1" login for tenant "1" should contain a JWT token
  
  When worker "worker-2" logs in for tenant "1" in multi tenant flow
  Then the worker "worker-2" login for tenant "1" should contain a JWT token
  
  When worker "worker-3" logs in for tenant "2" in multi tenant flow
  Then the worker "worker-3" login for tenant "2" should contain a JWT token
  
  When worker "worker-4" logs in for tenant "1" in multi tenant flow
  Then the worker "worker-4" login for tenant "1" should contain a JWT token
  
  When worker "worker-5" logs in for tenant "1" in multi tenant flow
  Then the worker "worker-5" login for tenant "1" should contain a JWT token
  
  # Test worker reassignment in multi-tenant environment
  When shift leader "leader-1" reassigns worker "worker-1" to shift leader "leader-2" for tenant "1" in multi tenant flow
  Then shift leader "leader-1" should have "2" workers assigned for tenant "1"
  Then shift leader "leader-2" should have "2" workers assigned for tenant "1"

  # Test shift period creation by shift leader
  When shift leader "leader-1" creates a weekly shift period for tenant "1" with the following schedule:
    | Date       | ShiftName | AmountOfWorkers |
    | 2024-01-15 | Morning   | 3               |
    | 2024-01-15 | Night     | 1               |
    | 2024-01-16 | Morning   | 3               |
    | 2024-01-16 | Night     | 1               |
    | 2024-01-17 | Morning   | 4               |
    | 2024-01-17 | Night     | 2               |
    | 2024-01-18 | Morning   | 4               |
    | 2024-01-18 | Night     | 2               |
    | 2024-01-19 | Morning   | 5               |
    | 2024-01-19 | Night     | 2               |
    | 2024-01-20 | Morning   | 3               |
    | 2024-01-20 | Night     | 1               |
    | 2024-01-21 | Morning   | 2               |
    | 2024-01-21 | Night     | 1               |
  
  Then the shift period creation should be successful for tenant "1"
  And the shift period should start from "2024-01-15" for tenant "1"
  And the shift period should end on "2024-01-21" for tenant "1"
  And the shift period should contain "7" days for tenant "1"
  And the shift period should have shifts for all configured days for tenant "1"