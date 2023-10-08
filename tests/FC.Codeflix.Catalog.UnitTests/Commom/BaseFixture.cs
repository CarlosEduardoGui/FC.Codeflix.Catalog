using Bogus;

namespace FC.CodeFlix.Catalog.UnitTests.Commom;
public abstract class BaseFixture
{
    protected BaseFixture()
    {
        Faker = new Faker("pt_BR");
    }

    public Faker Faker { get; set; }
}
