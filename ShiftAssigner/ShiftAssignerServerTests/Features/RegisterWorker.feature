Feature: Register Worker
  In order to allow workers to authenticate
  As a client of the API
  I want to register a worker and receive an authentication token

  Scenario: Successful worker registration returns auth token
    Given I have a worker registration payload
    When I POST the payload to "/api/auth/register-worker"
    Then the response should contain a JWT token
