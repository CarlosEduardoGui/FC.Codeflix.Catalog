using Bogus;

namespace FC.CodeFlix.Catalog.IntegrationTests.Base;
public abstract class BaseFixture
{
    protected Faker Faker { get; set; }

    protected BaseFixture() => Faker = new Faker("pt_BR");
}
