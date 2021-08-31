# Http To Grpc Proxy - POC
Solution intended to intercept communication with external services in integration tests.

## Problem
Predictable and isolated integration testing of application like in diagram is complicated.
![usual flow](https://www.plantuml.com/plantuml/png/XLDDJyCm3BtdLrZSGOBZ3WZ4XxG3HyG1JIXAJHkHn25nsIR4VyTfIcaHWnscok_Px_aZ5vbrujuxw7El7JmxsYeLjNdHBSBHpOun1UsU7YYvvWZU5SZl4SVmISRqPWsUKC3D0OYtj54VivftrYGQqJoLl4qKU0F7_V4vFAw35F7OMCsuMK6Regci1J2IRveMJuNbN__cd5LE9TMa4VsrdHLqpF6RVNQGNJeSsTAdDam9xWLKvMzCgksjm1PpGibA1MA4PDkEWPvXCHLr2Hc7i9-7WJlDOCZxFbWZxULQJypirJMP5v4JD4g20E38o4ORV4D7qMDWXQ4fgZ-rlysASorO9vzvdnbzbPnSmzPofsGyXUaf8U2ko3KmMJ-WCGn0tKeQxhIF3bSrz9MuR6kGiRD5JhI230S1CH6J8NT8SNu7I-7rRuNd-cfGGTdwr5SLzfXpEUB_j9VlHbs6FNoT3IotgMDqHTHIa2SeqbUJZn9Y4bhPvvIbPUX69oipfvu5JTItNJs_oSs5VCcZ_WG0 "usual flow")

## Solution
In the following diagram `HttpToGrpcMock` is service hosted in docker compose with `external.service.one` and `external.service.two` names assigned to it. Edge services will have those URL's resolved to proxy service and no communication to external world will happen at this point.

![proxy flow](https://www.plantuml.com/plantuml/png/VP9FJyCm3CNl-HGM9v3O97RaW2OX_XrscyHXcb2MU9srDEuIjp0XlZjEkp61q7fgxP-VtqiwpMAJj7M09l777jQ1turfhDlQ2k5YHYvnnjIHmtm1dmRqMJmcZhA4KJkwWyLB54mrUhA2orkeKkC6wbVZ03_u2c-KgdVdcSd03D0mTEW-YpHpVj8jCtPRnJPQE76Jyzm0_kcPhrFYOgXALeZZsnopv6FcGjy7mZvu-6T3KOai22iMuHfu3QSPyxh12D8BfZPUW-lBu3X6T5eKFbn0e8vY1HGpUGIlofWKgnkz1cEZXuGEgKFLmLshWcMKf9yczD2HxU-W58GTXt5l0oX3cxKlR33QLL2Vv4d5IbGRoeesakkJGsRGAC0nx7_MN1ujNe6pqQomE27iL7FLYdxaXcDMN-fKIb5FTAoxV3aKOwRwfh_PDm00 "proxy flow")

Sequence diagram may explain better how everyting works:
![test_flow](https://www.plantuml.com/plantuml/png/TLFBSi8m3BpxA_G3b7iE7JgTFZWmxRJv0U48ui6nKbk1ylUL4mWaCxqfahMxMYch91YbQxmnhGREkXQ3G4b99ectsDfdxoXCqw-HPvcNQa-JnBj8MtBktx0z69EXOR5u77eB-FpuBc6PA5JBbfLrhDye2t0XIUoiW32Sq0diECAIVeLYGB-qwBtfyK0RiuG2rc4eWkMcwOApA8w3oSNyQ2ghW5g_JM0XY6vV2_1k6AMiNJWaS0aoYGjxG2iTUdCbXqJnw2mPdI3tcoaop2RPQppZcJcq3Zi6t78d4CcIErAbmK-NLw8oTF8JnagJweLeCetCDvxCskzmS7FuAE0fgLWPqQx5UtrPGa-OMWvfZZcJEXzrDBztSx3wkkdPq4FkgwUfigrRL8HTlyC6pw2lOKiGr86KC9xr1A3H-z0r94eFPeDdBUibdBZpLNvQeU1MYz2r6XBeXIHkIEgCxjs1G6gKqQpBkOpUg1JsHxexHMWZDquz-CV4YqDcfGtw1_q1 "test_flow")

## Runing in docker compose and docker build
```powershell
docker compose -f ./DockerComposeTests/docker-compose.yml build
docker compose -f ./DockerComposeTests/docker-compose.yml -d up

docker build -o . -f ./DockerComposeTests/TestApp.Tests/Dockerfile --build-arg CACHEBUST=$(date) .
```
> Tests result xml file will be copied out of container into `./test_results` folder