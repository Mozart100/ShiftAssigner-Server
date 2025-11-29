Feature: Register Two Tenant Bosses and verify tenant isolation
  In order to ensure multi-tenant creation and isolation
  As a client of the API
  I want to register two tenant bosses and verify both companies exist independently

  @integration @registration @multitenant
  Scenario: Register two bosses and verify both tenants appear in tenants list with isolation
    Given I have tenant registrations for "CompanyAlpha" and "CompanyBeta"
    
    # Register first tenant
    When I register tenant "CompanyAlpha" with boss "boss-alpha-1"
    Then the response should contain a JWT token
    
    # Register second tenant
    When I register tenant "CompanyBeta" with boss "boss-beta-1"
    Then the response should contain a JWT token
    
    # Verify both tenants exist in global list
    When I GET the tenants list
    Then the tenants list should contain tenant "CompanyAlpha"
    And the tenants list should contain tenant "CompanyBeta"
    
    # CompanyAlpha creates shift leader
    When I create shift leader "leader-alpha-1" for tenant "CompanyAlpha"
    Then the response should contain a JWT token
    
    # CompanyBeta creates shift leader
    When I create shift leader "leader-beta-1" for tenant "CompanyBeta"
    Then the response should contain a JWT token
    
    # Verify CompanyAlpha shift leaders isolation
    When I GET the shift leaders for tenant "CompanyAlpha"
    Then the shift leaders list should contain "leader-alpha-1"
    And the shift leaders list should not contain "leader-beta-1"
    
    # Verify CompanyBeta shift leaders isolation
    When I GET the shift leaders for tenant "CompanyBeta"
    Then the shift leaders list should contain "leader-beta-1"
    And the shift leaders list should not contain "leader-alpha-1"
    
    # CompanyAlpha shift leader creates workers
    When shift leader "leader-alpha-1" in tenant "CompanyAlpha" creates 2 workers with ID prefix "ALPHA"
    
    # CompanyBeta shift leader creates workers
    When shift leader "leader-beta-1" in tenant "CompanyBeta" creates 2 workers with ID prefix "BETA"
    
    # Verify CompanyAlpha workers isolation
    When I GET the workers for tenant "CompanyAlpha"
    Then the workers list should contain 2 workers with ID prefix "ALPHA"
    And the workers list should not contain workers with ID prefix "BETA"
    
    # Verify CompanyBeta workers isolation
    When I GET the workers for tenant "CompanyBeta"
    Then the workers list should contain 2 workers with ID prefix "BETA"
    And the workers list should not contain workers with ID prefix "ALPHA"
    
    # Verify cross-tenant data access prevention
    When I try to access "CompanyBeta" data using "CompanyAlpha" credentials
    Then the access should be denied with tenant isolation error