using FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;
using FC.Codeflix.Catalog.EndToEndTests.Api.Common;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.UpdateCategory;

[CollectionDefinition(nameof(UpdateCategoryTestFixture))]
public class UpdateCategoryTestFixtureCollection : ICollectionFixture<UpdateCategoryTestFixture> { }

public class UpdateCategoryTestFixture : CategoryBaseFixture
{
    public UpdateCategoryInput GetExampleInput(Guid? id = null) => 
        new(
            id ?? Guid.NewGuid(),
            GetValidCategoryName(),
            GetValidCategoryDescription(),
            GetRandomBoolean());
}
