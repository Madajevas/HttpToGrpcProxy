Feature: Proxy is available in powershell

Background: There is a service running
    Given service is running on http://localhost:8080 base address
    And proxy client is connected to http://localhost:6000

Scenario: Round trip is sound
    When request GET /first is made
    And outgoing request to /first intercepted
    And test responds with "responding from unit test"
    Then response body is "responding from unit test"
