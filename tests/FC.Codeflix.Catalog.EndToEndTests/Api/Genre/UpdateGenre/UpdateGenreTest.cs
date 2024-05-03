using FC.Codeflix.Catalog.Api.ApiModels.Genre;
using FC.Codeflix.Catalog.Api.ApiModels.Response;
using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using FC.Codeflix.Catalog.EndToEndTests.Api.Genre.Common;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genre.UpdateGenre;

[Collection(nameof(GenreBaseFixture))]
public class UpdateGenreTest : IDisposable
{
    private readonly GenreBaseFixture _fixture;

    public UpdateGenreTest(GenreBaseFixture fixture)
        => _fixture = fixture;

    [Trait("EndToEnd/API", "Genre/UpdateGenre - Endpoints")]
    [Fact(DisplayName = nameof(UpdateGenreOk))]
    public async Task UpdateGenreOk()
    {
        var exampleGeres = _fixture.GetExampleListGenres();
        var targetGenre = exampleGeres[5];
        await _fixture.Persistence.InsertListAsync(exampleGeres);
        var input = new UpdateGenreApiInput(
            _fixture.GetValidGenreName(),
            _fixture.GetRandomBoolean()
        );

        var (response, output) = await _fixture.ApiClient
                        .Put<ApiResponse<GenreModelOutPut>>($"/genres/{targetGenre.Id}", input);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output.Data.Id.Should().Be(targetGenre.Id);
        output.Data.Name.Should().Be(input.Name);
        output.Data.IsActive.Should().Be(input.IsActive!.Value);
        output.Data.Categories.Should().HaveCount(0);
        var genreFromDb = await _fixture.Persistence.GetByIdAsync(output.Data.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb!.Name.Should().Be(input.Name);
        genreFromDb.IsActive.Should().Be(input.IsActive!.Value);
    }

    [Trait("EndToEnd/API", "Genre/UpdateGenre - Endpoints")]
    [Fact(DisplayName = nameof(ProblemDetailsNotFound))]
    public async Task ProblemDetailsNotFound()
    {
        var exampleGeres = _fixture.GetExampleListGenres();
        var randomGuid = Guid.NewGuid();
        await _fixture.Persistence.InsertListAsync(exampleGeres);
        var input = new UpdateGenreApiInput(
            _fixture.GetValidGenreName(),
            _fixture.GetRandomBoolean()
        );

        var (response, output) = await _fixture.ApiClient
                        .Put<ProblemDetails>($"/genres/{randomGuid}", input);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        output.Should().NotBeNull();
        output!.Status.Should().Be((int)HttpStatusCode.NotFound);
        output.Title.Should().Be("Not Found");
        output.Detail.Should().Be($"Genre '{randomGuid}' not found.");
        output.Type.Should().Be("NotFound");
    }

    [Trait("EndToEnd/API", "Genre/UpdateGenre - Endpoints")]
    [Fact(DisplayName = nameof(UpdateGenreWithRelations))]
    public async Task UpdateGenreWithRelations()
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
        var newRelationsCount = random.Next(2, categoryList.Count - 1);
        var newRelatedCategoriesIds = new List<Guid>();
        for (int i = 0; i < newRelationsCount; i++)
        {
            var randomCategory = random.Next(0, categoryList.Count - 1);
            var selected = categoryList[randomCategory];
            if (newRelatedCategoriesIds.Contains(selected.Id) is not true)
                newRelatedCategoriesIds.Add(selected.Id);
        }
        await _fixture.CategoriesPersistence.InsertListAsync(categoryList);
        await _fixture.Persistence.InsertListAsync(exampleGeres);
        await _fixture.Persistence.InsertGenresCategoriesRelationsListAsync(genresCategories);
        var input = new UpdateGenreApiInput(
            _fixture.GetValidGenreName(),
            _fixture.GetRandomBoolean(),
           newRelatedCategoriesIds
        );

        var (response, output) = await _fixture.ApiClient
                        .Put<ApiResponse<GenreModelOutPut>>($"/genres/{targetGenre.Id}", input);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output.Data.Id.Should().Be(targetGenre.Id);
        output.Data.Name.Should().Be(input.Name);
        output.Data.IsActive.Should().Be(input.IsActive!.Value);
        var relatedCategoriesIds = output.Data.Categories.Select(relation => relation.Id).ToList();
        relatedCategoriesIds.Should().BeEquivalentTo(newRelatedCategoriesIds);
        var genreFromDb = await _fixture.Persistence.GetByIdAsync(output.Data.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb!.Name.Should().Be(input.Name);
        genreFromDb.IsActive.Should().Be(input.IsActive!.Value);
        var genresCategoriesFromDb = await
            _fixture.Persistence.GetGenresCategoriesRelationsByIdAsync(targetGenre.Id);
        genresCategoriesFromDb.Should().NotBeNull();
        var relatedCategoriesIdsFromDb = genresCategoriesFromDb
            .Select(x => x.CategoryId)
            .ToList();
        relatedCategoriesIdsFromDb.Should().BeEquivalentTo(newRelatedCategoriesIds);
    }


    [Trait("EndToEnd/API", "Genre/UpdateGenre - Endpoints")]
    [Fact(DisplayName = nameof(ErrorWithInvalidRelations))]
    public async Task ErrorWithInvalidRelations()
    {
        var exampleGeres = _fixture.GetExampleListGenres();
        var targetGenre = exampleGeres[5];
        var invalidCategoryId = Guid.NewGuid();
        await _fixture.Persistence.InsertListAsync(exampleGeres);
        var input = new UpdateGenreApiInput(
            _fixture.GetValidGenreName(),
            _fixture.GetRandomBoolean(),
            new List<Guid>()
            {
                invalidCategoryId
            }
        );

        var (response, output) = await _fixture.ApiClient
                        .Put<ProblemDetails>($"/genres/{targetGenre.Id}", input);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        output.Should().NotBeNull();
        output!.Type.Should().Be("RelatedAggregate");
        output.Detail.Should().Be($"Related category Id (or Ids) not found: {invalidCategoryId}");
    }

    [Trait("EndToEnd/API", "Genre/UpdateGenre - Endpoints")]
    [Fact(DisplayName = nameof(UpdateGenreWhithoutRelations))]
    public async Task UpdateGenreWhithoutRelations()
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
        var input = new UpdateGenreApiInput(
            _fixture.GetValidGenreName(),
            _fixture.GetRandomBoolean()
        );

        var (response, output) = await _fixture.ApiClient
                        .Put<ApiResponse<GenreModelOutPut>>($"/genres/{targetGenre.Id}", input);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output.Data.Id.Should().Be(targetGenre.Id);
        output.Data.Name.Should().Be(input.Name);
        output.Data.IsActive.Should().Be(input.IsActive!.Value);
        var relatedCategoriesIds = output.Data.Categories.Select(relation => relation.Id).ToList();
        relatedCategoriesIds.Should().BeEquivalentTo(targetGenre.Categories);
        var genreFromDb = await _fixture.Persistence.GetByIdAsync(output.Data.Id);
        genreFromDb.Should().NotBeNull();
        genreFromDb!.Name.Should().Be(input.Name);
        genreFromDb.IsActive.Should().Be(input.IsActive!.Value);
        var genresCategoriesFromDb = await
            _fixture.Persistence.GetGenresCategoriesRelationsByIdAsync(targetGenre.Id);
        genresCategoriesFromDb.Should().NotBeNull();
        var relatedCategoriesIdsFromDb = genresCategoriesFromDb
            .Select(x => x.CategoryId)
            .ToList();
        relatedCategoriesIdsFromDb.Should().BeEquivalentTo(targetGenre.Categories);
    }

    public void Dispose() => _fixture.CleanPersistence();
}
