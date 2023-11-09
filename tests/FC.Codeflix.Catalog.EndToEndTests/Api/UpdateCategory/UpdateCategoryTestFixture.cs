using FC.Codeflix.Catalog.Api.ApiModels.Category;
using FC.Codeflix.Catalog.EndToEndTests.Api.Common;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.UpdateCategory;

[CollectionDefinition(nameof(UpdateCategoryTestFixture))]
public class UpdateCategoryTestFixtureCollection : ICollectionFixture<UpdateCategoryTestFixture> { }

public class UpdateCategoryTestFixture : CategoryBaseFixture
{
    public UpdateCategoryApiInput GetExampleApiInput() =>
        new(
            GetValidCategoryName(),
            GetValidCategoryDescription(),
            GetRandomBoolean());
}
