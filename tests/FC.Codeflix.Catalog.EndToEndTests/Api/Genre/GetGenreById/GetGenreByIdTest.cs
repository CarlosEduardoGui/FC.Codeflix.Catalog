using FC.Codeflix.Catalog.Api.ApiModels.Response;
using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using FC.Codeflix.Catalog.EndToEndTests.Api.Genre.Common;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genre.GetGenreById;

[Collection(nameof(GenreBaseFixture))]
public class GetGenreByIdTest : IDisposable
{
    private readonly GenreBaseFixture _fixture;

    public GetGenreByIdTest(GenreBaseFixture fixture)
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

    [Trait("EndToEnd/API", "Genre/GetGenre - Endpoints")]
    [Fact(DisplayName = nameof(NotFoundGenre))]
    public async Task NotFoundGenre()
    {
        var exampleGeres = _fixture.GetExampleListGenres();
        var targetGenre = Guid.NewGuid();
        await _fixture.Persistence.InsertListAsync(exampleGeres);

        var (response, output) = await _fixture.ApiClient
                        .Get<ProblemDetails>($"/genres/{targetGenre}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.NotFound);
        output.Should().NotBeNull();
        output!.Status.Should().Be((int)HttpStatusCode.NotFound);
        output.Title.Should().Be("Not Found");
        output.Detail.Should().Be($"Genre '{targetGenre}' not found.");
        output.Type.Should().Be("NotFound");
    }

    [Trait("EndToEnd/API", "Genre/GetGenre - Endpoints")]
    [Fact(DisplayName = nameof(GetGenreWithRelations))]
    public async Task GetGenreWithRelations()
    {
        var exampleGeres = _fixture.GetExampleListGenres();
        var categoryList = _fixture.GetExampleCategoriesList();
        var random = new Random();
        exampleGeres.ForEach(genre =>
        {
            var relationsCount = random.Next(2, categoryList.Count - 1);
            for (int i = 0; i < relationsCount; i++)
            {
                var randomCategory = random.Next(0, categoryList.Count - 1);
                var selected = categoryList[randomCategory];
                if (genre.Categories.Contains(selected.Id) is not true)
                    genre.AddCategory(selected.Id);
            }
        });
        var genresCategories = new List<GenresCategories>();
        exampleGeres.ForEach(genre =>
        {
            genre.Categories.ToList().ForEach(categoryId =>
            {
                genresCategories.Add(new GenresCategories(categoryId, genre.Id));
            });
        });
        var targetGenre = exampleGeres[5];
        await _fixture.CategoriesPersistence.InsertListAsync(categoryList);
        await _fixture.Persistence.InsertListAsync(exampleGeres);
        await _fixture.Persistence.InsertGenresCategoriesRelationsListAsync(genresCategories);

        var (response, output) = await _fixture.ApiClient
                        .Get<ApiResponse<GenreModelOutPut>>($"/genres/{targetGenre.Id}");

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output.Data.Id.Should().Be(targetGenre.Id);
        output.Data.Name.Should().Be(targetGenre.Name);
        output.Data.IsActive.Should().Be(targetGenre.IsActive);
        var relatedCategoriesIds = output.Data.Categories.Select(relation => relation.Id).ToList();
        relatedCategoriesIds.Should().BeEquivalentTo(targetGenre.Categories);
    }

    public void Dispose() =>
        _fixture.CleanPersistence();
}
