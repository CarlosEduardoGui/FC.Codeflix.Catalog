using FC.Codeflix.Catalog.Application.UseCases.Genre.CreateGenre;
using FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.Common;
using Xunit;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.CreateGenre;

[CollectionDefinition(nameof(CreateGenreTestFixture))]
public class CreateGenreTestFixtureCollection : ICollectionFixture<CreateGenreTestFixture> { }

public class CreateGenreTestFixture : GenreUseCaseBaseFixture
{
    public CreateGenreInput GetExampleInput() =>
        new(
            GetValidGenreName(),
            GetRandomBoolean()
        );
}
