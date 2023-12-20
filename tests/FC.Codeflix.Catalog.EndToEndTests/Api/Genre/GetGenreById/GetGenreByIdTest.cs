using FC.Codeflix.Catalog.Api.ApiModels.Response;
using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using FluentAssertions;
using System.Net;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genre.GetGenreById;

[Collection(nameof(GetGenreByIdTestFixture))]
public class GetGenreByIdTest : IDisposable
{
    private readonly GetGenreByIdTestFixture _fixture;

    public GetGenreByIdTest(GetGenreByIdTestFixture fixture)
        => _fixture = fixture;

    [Trait("EndToEnd/API", "Genre/GetGenre - Endpoints")]
    [Fact(DisplayName = nameof(GetGenreOk))]
    public async Task GetGenreOk()
    {
        var exampleGeres = _fixture.GetExampleListGenres();
        var targetGenre = exampleGeres[5];
        await _fixture.Persistence.InsertListAsync(exampleGeres);

        var (response, output) = await _fixture.ApiClient
                        .Get<ApiResponse<GenreModelOutPut>>($"/genres/{targetGenre.Id}");

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output.Data.Id.Should().Be(targetGenre.Id);
        output.Data.Name.Should().Be(targetGenre.Name);
        output.Data.IsActive.Should().Be(targetGenre.IsActive);
        output.Data.Categories.Should().HaveCount(0);
    }

    public void Dispose() =>
        _fixture.CleanPersistence();
}
