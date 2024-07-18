using FC.Codeflix.Catalog.Application.UseCases.Video.CreateVideo;
using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.UnitTests.Application.Video.Common;
using Xunit;
using System.Text;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.CreateVideo;

[CollectionDefinition(nameof(CreateVideoTestFixture))]
public class CreateVideoTestFixtureCollection : ICollectionFixture<CreateVideoTestFixture> { }

public class CreateVideoTestFixture : VideoUseCaseBaseFixture
{
    public CreateVideoInput GetValidVideoInput(
        List<Guid>? categoriesIds = null,
        List<Guid>? genresIds = null,
        List<Guid>? castMembersIds = null,
        FileInput? thumb = null,
        FileInput? banner = null,
        FileInput? thumbHalf = null)
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
            CastMembersIds: castMembersIds,
            Thumb: thumb,
            Banner: banner,
            ThumbHalf: thumbHalf
        );
}
