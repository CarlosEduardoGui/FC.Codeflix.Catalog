using FC.CodeFlix.Catalog.Application.UseCases.Category.CreateCategory;
using FC.CodeFlix.Catalog.EndToEndTests.Api.Common;
using Xunit;

namespace FC.CodeFlix.Catalog.EndToEndTests.Api.CreateCategory;

[CollectionDefinition(nameof(CreateCategoryTestFixture))]
public class CreateCategoryTestFixtureCollection : ICollectionFixture<CreateCategoryTestFixture> { }

public class CreateCategoryTestFixture : CategoryBaseFixture
{
    public CreateCategoryInput GetExampleInput() =>
        new(GetValidCategoryName(), GetValidCategoryDescription());
}
