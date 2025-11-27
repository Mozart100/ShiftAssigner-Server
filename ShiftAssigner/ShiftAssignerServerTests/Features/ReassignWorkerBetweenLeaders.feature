Feature: Reassign worker between shift leaders
  In order to allow flexible worker management
  As a tenant with multiple shift leaders
  I want to reassign a worker from one shift leader to another

  @reassign
  Scenario: Tenant creates two leaders, first leader creates two workers, then reassigns one worker to second leader
    Given I have a tenant boss registration payload
    When Tenant registration "reassign-tenant-1"
    Then the response should contain a JWT token

    When I create a shift leader with id "leader-A"
    Then the response should contain a JWT token

    When I create a shift leader with id "leader-B"
    Then the response should contain a JWT token

    When the shift leader with id "leader-A" creates 2 workers

    When I GET the workers for leader "leader-A"
    Then the workers list should contain 2 workers

    When I reassign the second worker to leader "leader-B"

    When I GET the workers for leader "leader-A"
    Then the workers list should contain 1 worker

    When I GET the workers for leader "leader-B"
    Then the workers list should contain 1 worker
