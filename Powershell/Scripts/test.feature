Feature: I do what I want

Background: There is a service running
    Given service is running on http://localhost:8080 base address
    And proxy client is connected to http://localhost:6000

Scenario: Round trip is sound
    When request GET /first is made
    And outgoing request to /first intercepted
    And respond with
    Then response status code is 200
