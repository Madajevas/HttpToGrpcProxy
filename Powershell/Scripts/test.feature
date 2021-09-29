Feature: Proxy is available in powershell

Background: There is a service running
    Given service is running on http://local.host:5000 base address
    And proxy client is connected to http://localhost:6000

Scenario: Round trip is sound
    When request GET /first is made
    And outgoing request to /first intercepted
    And test responds with "responding from unit test"
    Then response body is "responding from unit test"

Scenario: test responds with json
    When request POST /json is made with the following data and application/json content type
    | name        | value | date                |
    | integration | tests | 2021-09-29 21:29:00 |
    And outgoing request to /json intercepted
    # Then requested method is POST
    And sent object is like
    | name        | value | date                |
    | integration | tests | 2021-09-29 21:29:00 |

