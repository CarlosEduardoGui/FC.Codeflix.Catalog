using FC.Codeflix.Catalog.Application.UseCases.Video.CreateVideo;
using FC.Codeflix.Catalog.UnitTests.Application.Video.Common;
using Xunit;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.CreateVideo;

[CollectionDefinition(nameof(CreateVideoTestFixture))]
public class CreateVideoTestFixtureCollection : ICollectionFixture<CreateVideoTestFixture> { }

public class CreateVideoTestFixture : VideoUseCaseBaseFixture
{
    public CreateVideoInput GetValidVideoInput(
        List<Guid>? categoriesIds = null,
        List<Guid>? genresIds = null,
        List<Guid>? castMembersIds = null
    )
        => new(
            GetValidTitle(),
            GetValidDescription(),
            GetRandomBoolean(),
            GetValidDuration(),
            GetRandomRating(),
            GetValidYearLauched(),
            GetRandomBoolean(),
            CategoriesIds: categoriesIds,
            GenresIds: genresIds,
            CastMembersIds: castMembersIds
        );
}
