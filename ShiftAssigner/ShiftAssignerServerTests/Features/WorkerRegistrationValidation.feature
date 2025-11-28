Feature: Worker Registration Validation
  In order to ensure data integrity
  As a system administrator
  I want to validate all worker registration requests

  Background:
    Given I have a tenant boss registration payload
    When Tenant registration "validation-tenant-1"
    Then the response should contain a JWT token
    When I create a shift leader with id "validation-leader-1"
    Then the response should contain a JWT token

  @validation @worker-registration
  Scenario: Valid worker registration should succeed
    When I register a worker with valid data
      | Field          | Value              |
      | ID             | worker-valid-001   |
      | FirstName      | John               |
      | LastName       | Doe                |
      | PhoneNumber    | +1-555-123-4567    |
      | DateOfBirth    | 2000-01-15         |
      | PasswordHash   | SecurePass123      |
      | ShiftLeaderId  | validation-leader-1|
    Then the worker registration should succeed
    And the response should contain a JWT token

  @validation @worker-registration @negative
  Scenario: Worker registration without ID should fail
    When I register a worker with invalid data
      | Field          | Value              |
      | ID             |                    |
      | FirstName      | John               |
      | LastName       | Doe                |
      | PhoneNumber    | 555-1234           |
      | DateOfBirth    | 2000-01-15         |
      | PasswordHash   | SecurePass123      |
      | ShiftLeaderId  | validation-leader-1|
    Then the worker registration should fail with validation errors
    And the validation error should contain "ID is required"

  @validation @worker-registration @negative
  Scenario: Worker registration with short ID should fail
    When I register a worker with invalid data
      | Field          | Value              |
      | ID             | ab                 |
      | FirstName      | John               |
      | LastName       | Doe                |
      | PhoneNumber    | 555-1234           |
      | DateOfBirth    | 2000-01-15         |
      | PasswordHash   | SecurePass123      |
      | ShiftLeaderId  | validation-leader-1|
    Then the worker registration should fail with validation errors
    And the validation error should contain "ID must be at least 3 characters"

  @validation @worker-registration @negative
  Scenario: Worker registration with ID containing special characters should fail
    When I register a worker with invalid data
      | Field          | Value              |
      | ID             | worker@123#$       |
      | FirstName      | John               |
      | LastName       | Doe                |
      | PhoneNumber    | 555-1234           |
      | DateOfBirth    | 2000-01-15         |
      | PasswordHash   | SecurePass123      |
      | ShiftLeaderId  | validation-leader-1|
    Then the worker registration should fail with validation errors
    And the validation error should contain "ID must contain only alphanumeric characters, hyphens, or underscores"

  @validation @worker-registration @negative
  Scenario: Worker registration without FirstName should fail
    When I register a worker with invalid data
      | Field          | Value              |
      | ID             | worker-002         |
      | FirstName      |                    |
      | LastName       | Doe                |
      | PhoneNumber    | 555-1234           |
      | DateOfBirth    | 2000-01-15         |
      | PasswordHash   | SecurePass123      |
      | ShiftLeaderId  | validation-leader-1|
    Then the worker registration should fail with validation errors
    And the validation error should contain "FirstName is required"

  @validation @worker-registration @negative
  Scenario: Worker registration without LastName should fail
    When I register a worker with invalid data
      | Field          | Value              |
      | ID             | worker-003         |
      | FirstName      | John               |
      | LastName       |                    |
      | PhoneNumber    | 555-1234           |
      | DateOfBirth    | 2000-01-15         |
      | PasswordHash   | SecurePass123      |
      | ShiftLeaderId  | validation-leader-1|
    Then the worker registration should fail with validation errors
    And the validation error should contain "LastName is required"

  @validation @worker-registration @negative
  Scenario: Worker registration with FirstName containing numbers should fail
    When I register a worker with invalid data
      | Field          | Value              |
      | ID             | worker-004         |
      | FirstName      | John123            |
      | LastName       | Doe                |
      | PhoneNumber    | 555-1234           |
      | DateOfBirth    | 2000-01-15         |
      | PasswordHash   | SecurePass123      |
      | ShiftLeaderId  | validation-leader-1|
    Then the worker registration should fail with validation errors
    And the validation error should contain "Names must not contain numbers or special characters"

  @validation @worker-registration @negative
  Scenario: Worker registration with LastName containing special characters should fail
    When I register a worker with invalid data
      | Field          | Value              |
      | ID             | worker-005         |
      | FirstName      | John               |
      | LastName       | Doe@123            |
      | PhoneNumber    | 555-1234           |
      | DateOfBirth    | 2000-01-15         |
      | PasswordHash   | SecurePass123      |
      | ShiftLeaderId  | validation-leader-1|
    Then the worker registration should fail with validation errors
    And the validation error should contain "Names must not contain numbers or special characters"

  @validation @worker-registration @negative
  Scenario: Worker registration without PhoneNumber should fail
    When I register a worker with invalid data
      | Field          | Value              |
      | ID             | worker-006         |
      | FirstName      | John               |
      | LastName       | Doe                |
      | PhoneNumber    |                    |
      | DateOfBirth    | 2000-01-15         |
      | PasswordHash   | SecurePass123      |
      | ShiftLeaderId  | validation-leader-1|
    Then the worker registration should fail with validation errors
    And the validation error should contain "PhoneNumber is required"

  @validation @worker-registration @negative
  Scenario: Worker registration with invalid PhoneNumber format should fail
    When I register a worker with invalid data
      | Field          | Value              |
      | ID             | worker-007         |
      | FirstName      | John               |
      | LastName       | Doe                |
      | PhoneNumber    | ABC-DEF-GHIJ       |
      | DateOfBirth    | 2000-01-15         |
      | PasswordHash   | SecurePass123      |
      | ShiftLeaderId  | validation-leader-1|
    Then the worker registration should fail with validation errors
    And the validation error should contain "PhoneNumber must contain only digits"

  @validation @worker-registration @negative
  Scenario: Worker registration with age below 16 should fail
    When I register a worker with invalid data
      | Field          | Value              |
      | ID             | worker-008         |
      | FirstName      | Young              |
      | LastName       | Person             |
      | PhoneNumber    | 555-1234           |
      | DateOfBirth    | 2015-01-01         |
      | PasswordHash   | SecurePass123      |
      | ShiftLeaderId  | validation-leader-1|
    Then the worker registration should fail with validation errors
    And the validation error should contain "Person must be at least 16 years old"

  @validation @worker-registration @negative
  Scenario: Worker registration with age above 100 should fail
    When I register a worker with invalid data
      | Field          | Value              |
      | ID             | worker-009         |
      | FirstName      | Old                |
      | LastName       | Person             |
      | PhoneNumber    | 555-1234           |
      | DateOfBirth    | 1900-01-01         |
      | PasswordHash   | SecurePass123      |
      | ShiftLeaderId  | validation-leader-1|
    Then the worker registration should fail with validation errors
    And the validation error should contain "not older than 100 years"

  @validation @worker-registration @negative
  Scenario: Worker registration with short password should fail
    When I register a worker with invalid data
      | Field          | Value              |
      | ID             | worker-010         |
      | FirstName      | John               |
      | LastName       | Doe                |
      | PhoneNumber    | 555-1234           |
      | DateOfBirth    | 2000-01-15         |
      | PasswordHash   | 12345              |
      | ShiftLeaderId  | validation-leader-1|
    Then the worker registration should fail with validation errors
    And the validation error should contain "Password must be at least 6 characters"

  @validation @worker-registration @negative
  Scenario: Worker registration without ShiftLeaderId should fail
    When I register a worker with invalid data
      | Field          | Value              |
      | ID             | worker-011         |
      | FirstName      | John               |
      | LastName       | Doe                |
      | PhoneNumber    | 555-1234           |
      | DateOfBirth    | 2000-01-15         |
      | PasswordHash   | SecurePass123      |
      | ShiftLeaderId  |                    |
    Then the worker registration should fail with validation errors
    And the validation error should contain "ShiftLeaderId is required when registering a worker"

  @validation @worker-registration @negative
  Scenario: Worker registration with multiple validation errors should fail
    When I register a worker with invalid data
      | Field          | Value              |
      | ID             |                    |
      | FirstName      | John123            |
      | LastName       |                    |
      | PhoneNumber    |                    |
      | DateOfBirth    | 2015-01-01         |
      | PasswordHash   | 123                |
      | ShiftLeaderId  |                    |
    Then the worker registration should fail with validation errors
    And the validation should contain multiple errors
    And the validation error should contain "ID is required"
    And the validation error should contain "LastName is required"
    And the validation error should contain "PhoneNumber is required"
    And the validation error should contain "Password must be at least 6 characters"
    And the validation error should contain "ShiftLeaderId is required when registering a worker"
    And the validation error should contain "Names must not contain numbers"

  @validation @worker-registration
  Scenario: Worker registration with valid international phone number should succeed
    When I register a worker with valid data
      | Field          | Value              |
      | ID             | worker-intl-001    |
      | FirstName      | Maria              |
      | LastName       | Garcia             |
      | PhoneNumber    | +34-91-123-4567    |
      | DateOfBirth    | 1995-06-20         |
      | PasswordHash   | SecurePass456      |
      | ShiftLeaderId  | validation-leader-1|
    Then the worker registration should succeed
    And the response should contain a JWT token

  @validation @worker-registration
  Scenario: Worker registration with hyphenated name should succeed
    When I register a worker with valid data
      | Field          | Value              |
      | ID             | worker-hyphen-001  |
      | FirstName      | Mary-Anne          |
      | LastName       | Smith-Jones        |
      | PhoneNumber    | 555-9999           |
      | DateOfBirth    | 1990-03-10         |
      | PasswordHash   | SecurePass789      |
      | ShiftLeaderId  | validation-leader-1|
    Then the worker registration should succeed
    And the response should contain a JWT token

  @validation @worker-registration
  Scenario: Worker registration with name containing apostrophe should succeed
    When I register a worker with valid data
      | Field          | Value              |
      | ID             | worker-apos-001    |
      | FirstName      | Sean               |
      | LastName       | O'Brien            |
      | PhoneNumber    | 555-8888           |
      | DateOfBirth    | 1988-12-25         |
      | PasswordHash   | SecurePass000      |
      | ShiftLeaderId  | validation-leader-1|
    Then the worker registration should succeed
    And the response should contain a JWT token

  @validation @worker-registration
  Scenario: Worker registration at minimum age (16 years) should succeed
    When I register a worker with valid data
      | Field          | Value              |
      | ID             | worker-min-age-001 |
      | FirstName      | Young              |
      | LastName       | Worker             |
      | PhoneNumber    | 555-7777           |
      | DateOfBirth    | 2008-11-28         |
      | PasswordHash   | SecurePass111      |
      | ShiftLeaderId  | validation-leader-1|
    Then the worker registration should succeed
    And the response should contain a JWT token
