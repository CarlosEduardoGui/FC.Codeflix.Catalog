using Bogus;
using FC.Codeflix.Catalog.Application.Interfaces;
using Moq;

namespace FC.Codeflix.Catalog.UnitTests.Commom;
public abstract class BaseFixture
{
    protected BaseFixture()
    {
        Faker = new Faker("pt_BR");
    }

    public Faker Faker { get; set; }

    public Mock<IUnitOfWork> GetUnitOfWorkMock() => new();

    public bool GetRandomBoolean() => new Random().NextDouble() <= 0.5;
}
