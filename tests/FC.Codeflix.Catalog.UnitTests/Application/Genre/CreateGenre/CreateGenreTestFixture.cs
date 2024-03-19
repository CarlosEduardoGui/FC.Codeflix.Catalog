using FC.Codeflix.Catalog.Application.UseCases.Genre.CreateGenre;
using FC.Codeflix.Catalog.UnitTests.Application.Genre.Common;
using Xunit;

namespace FC.Codeflix.Catalog.UnitTests.Application.Genre.CreateGenre;

[CollectionDefinition(nameof(CreateGenreTestFixture))]
public class CreateGenreTestFixtureCollection : ICollectionFixture<CreateGenreTestFixture> { }

public class CreateGenreTestFixture : GenreUseCaseBaseFixture
{
    public CreateGenreInput GetInput() =>
        new(
            GetValidGenreName(),
            GetRandomBoolean()
    );

    public CreateGenreInput GetInput(string? name = null) =>
        new(
            name!,
            GetRandomBoolean()
    );

    public CreateGenreInput GetInputWithCategories()
    {
        var numberOfCategoriesIds = new Random().Next(1, 10);

        var categoriesIds = Enumerable.Range(1, numberOfCategoriesIds)
            .Select(_ => Guid.NewGuid()).ToList();

        return new(
             GetValidGenreName(),
             GetRandomBoolean(),
             categoriesIds
        );
    }
}
