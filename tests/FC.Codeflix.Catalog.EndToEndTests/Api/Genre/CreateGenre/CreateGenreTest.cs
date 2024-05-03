using FC.Codeflix.Catalog.Api.ApiModels.Response;
using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using FC.Codeflix.Catalog.Application.UseCases.Genre.CreateGenre;
using FC.Codeflix.Catalog.EndToEndTests.Api.Genre.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genre.CreateGenre;

[Collection(nameof(GenreBaseFixture))]
public class CreateGenreTest : IDisposable
{
    private readonly GenreBaseFixture _fixture;

    public CreateGenreTest(GenreBaseFixture fixture)
        => _fixture = fixture;

    [Trait("EndToEnd/API", "Genre/CreateGenre - Endpoints")]
    [Fact(DisplayName = nameof(CreateGenreOk))]
    public async Task CreateGenreOk()
    {
        var targetGenre = new CreateGenreInput(
            _fixture.GetValidGenreName(),
            _fixture.GetRandomBoolean()
        );

        var (response, output) = await _fixture.ApiClient
                        .Post<ApiResponse<GenreModelOutPut>>($"/genres", targetGenre);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Data.Id.Should().NotBeEmpty();
        output.Data.Name.Should().Be(targetGenre.Name);
        output.Data.IsActive.Should().Be(targetGenre.IsAtive);
        output.Data.Categories.Should().HaveCount(0);
        var genreFromDb = await _fixture.Persistence.GetByIdAsync(output.Data.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb!.Name.Should().Be(targetGenre.Name);
        genreFromDb.IsActive.Should().Be(targetGenre.IsAtive);
        genreFromDb.Categories.Should().HaveCount(0);
    }

    [Trait("EndToEnd/API", "Genre/CreateGenre - Endpoints")]
    [Fact(DisplayName = nameof(CreateGenreWithRelations))]
    public async Task CreateGenreWithRelations()
    {
        var categoriesExample =
            _fixture.GetExampleCategoriesList();
        await _fixture.CategoriesPersistence.InsertListAsync(categoriesExample);
        var relatedCategories = categoriesExample
            .Skip(3)
            .Take(3)
            .Select(x => x.Id)
            .ToList();
        var targetGenre = new CreateGenreInput(
            _fixture.GetValidGenreName(),
            _fixture.GetRandomBoolean(),
            relatedCategories
        );

        var (response, output) = await _fixture.ApiClient
                        .Post<ApiResponse<GenreModelOutPut>>($"/genres", targetGenre);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Data.Id.Should().NotBeEmpty();
        output.Data.Name.Should().Be(targetGenre.Name);
        output.Data.IsActive.Should().Be(targetGenre.IsAtive);
        output.Data.Categories.Should().HaveCount(relatedCategories.Count);
        var outputRelatedCategoryIds = output.Data.Categories.Select(x => x.Id).ToList();
        outputRelatedCategoryIds.Should().BeEquivalentTo(relatedCategories);
        var genreFromDb = await _fixture.Persistence.GetByIdAsync(output.Data.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb!.Name.Should().Be(targetGenre.Name);
        genreFromDb.IsActive.Should().Be(targetGenre.IsAtive);
        var relationsFromDb = await _fixture.Persistence
            .GetGenresCategoriesRelationsByIdAsync(output.Data.Id);
        relationsFromDb.Should().NotBeNull();
        relationsFromDb.Should().HaveCount(relatedCategories.Count);
        var relatedCategoriesIdsFromDb = relationsFromDb
            .Select(x => x.CategoryId)
            .ToList();
        relatedCategoriesIdsFromDb.Should().BeEquivalentTo(relatedCategories);
    }

    [Trait("EndToEnd/API", "Genre/CreateGenre - Endpoints")]
    [Fact(DisplayName = nameof(CreateGenreWithInvalidRelations))]
    public async Task CreateGenreWithInvalidRelations()
    {
        var categoriesExample =
            _fixture.GetExampleCategoriesList();
        await _fixture.CategoriesPersistence.InsertListAsync(categoriesExample);
        var relatedCategories = categoriesExample
            .Skip(3)
            .Take(3)
            .Select(x => x.Id)
            .ToList();
        var invalidCategoryId = Guid.NewGuid();
        relatedCategories.Add(invalidCategoryId);
        var targetGenre = new CreateGenreInput(
            _fixture.GetValidGenreName(),
            _fixture.GetRandomBoolean(),
            relatedCategories
        );

        var (response, output) = await _fixture.ApiClient
                        .Post<ProblemDetails>($"/genres", targetGenre);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        output.Should().NotBeNull();
        output!.Type.Should().Be("RelatedAggregate");
        output.Detail.Should().Be($"Related category Id (or Ids) not found: {invalidCategoryId}");
    }

    public void Dispose()
        => _fixture.CleanPersistence();
}
