[![.NET](https://github.com/CarlosEduardoGui/FC.Codeflix.Catalog/actions/workflows/dotnet.yml/badge.svg?branch=master)](https://github.com/CarlosEduardoGui/FC.Codeflix.Catalog/actions/workflows/dotnet.yml)

## Necessary tools.

- [.NET 6 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- For Windows:
  - [Docker Desktop](https://docs.docker.com/desktop/install/windows-install/)
  - OR
  - [WSL's Docker](https://learn.microsoft.com/en-us/windows/wsl/tutorials/wsl-containers)
- For Linux or MacOS:
  - [Docker](https://docs.docker.com/engine/install/ubuntu/)

## How to run?

- Clone the repository:
```sh
git clone https://github.com/CarlosEduardoGui/FC.Codefix.Catalog
```

- Then run the solution file with Visual Studio 2022 or open using Visual Code.

- If you are using a **Windows S.O.** you **SHOULD** change the string connection, but just the server attribute, inside of the **appSettings.json**, **appSettings.Delevepment.json**, and **appSettings.EndToEndTest.json**
  
Linux / MacOS:
```
"CatalogDb": "Server=localhost;
```
Windows:
```
"CatalogDb": "Server=(localdb)\\mssqllocaldb;"
```
<br />

- Executing the docker-compose.yaml to run all unit tests, integration tests, and end to end tests
  - Browsing to FC.Codeflix.Catalog.EndToEndTests and then execute this command below
    
    ```
    docker-compose up -d
    ```
  Then wait until the SQL Server up.
  - If some test broke, open a issue and inform all details.
    

## Technologies

- [.Net 6.*/C# 10^](https://learn.microsoft.com/en-us/dotnet/)
- [MediatoR](https://github.com/jbogard/MediatR)
- [Fluent Validations](https://docs.fluentvalidation.net/en/latest/)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/)
- [Polly](https://www.thepollyproject.org)
- [Bogus](https://github.com/bchavez/Bogus)
- [Xunit](https://xunit.net)

### Infrastructure

- [Docker]((https://docs.docker.com/desktop/install/windows-install/))
- [Github Actions](https://docs.github.com/pt/actions/quickstart)
- [RabbitMQ](https://www.rabbitmq.com)
- [Kafka](https://kafka.apache.org/documentation/)
- [Keycloack](https://www.keycloak.org/documentation)

### Concepts

- [Test Driven Development](https://martinfowler.com/bliki/TestDrivenDevelopment.html)
- [Domain Driven Design](https://martinfowler.com/bliki/DomainDrivenDesign.html)
- [Clean architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [SOLID](https://en.wikipedia.org/wiki/SOLID)
- [Unit of Work](https://learn.microsoft.com/en-us/aspnet/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application)
- [Triple A (Arrange, Act, Assert)](https://martinfowler.com/articles/practical-test-pyramid.html#:~:text=There%27s%20a%20nice%20mnemonic%20to,and%20then%20the%20assertion%20part.)

## Complete application diagram.

![PlantUML's solution](https://github.com/CarlosEduardoGui/FC.Codeflix.Catalog/assets/43711772/d8979056-d3f8-487b-acd7-27ebe8aee140)
