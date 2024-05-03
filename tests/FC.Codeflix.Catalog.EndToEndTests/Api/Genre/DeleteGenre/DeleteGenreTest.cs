using FC.Codeflix.Catalog.EndToEndTests.Api.Genre.Common;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genre.DeleteCategory;

[Collection(nameof(GenreBaseFixture))]
public class DeleteGenreTest : IDisposable
{
    private readonly GenreBaseFixture _fixture;

    public DeleteGenreTest(GenreBaseFixture fixture)
        => _fixture = fixture;

    [Trait("EndToEnd/Api", "Genre/Delete - Endpoints")]
    [Fact(DisplayName = nameof(DeleteGenreOk))]
    public async Task DeleteGenreOk()
    {
        var exampleGenreList = _fixture.GetExampleListGenres();
        await _fixture.Persistence.InsertListAsync(exampleGenreList);
        var exampleGenre = exampleGenreList[5];

        var (response, output) = await
            _fixture.ApiClient.Delete<object>(
                $"/genres/{exampleGenre.Id}"
            );

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        output.Should().BeNull();
        var persistenceGenre = await _fixture
            .Persistence
            .GetByIdAsync(exampleGenre.Id);
        persistenceGenre.Should().BeNull();
    }

    [Trait("EndToEnd/Api", "Genre/Delete - Endpoints")]
    [Fact(DisplayName = nameof(NotFound))]
    public async Task NotFound()
    {
        var exampleGenreList = _fixture.GetExampleListGenres();
        await _fixture.Persistence.InsertListAsync(exampleGenreList);
        var targetGenre = Guid.NewGuid();

        var (response, output) = await
            _fixture.ApiClient.Delete<ProblemDetails>(
                $"/genres/{targetGenre}"
            );

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        output.Should().NotBeNull();
        output!.Title.Should().Be("Not Found");
        output!.Status.Should().Be((int)HttpStatusCode.NotFound);
        output!.Type.Should().Be("NotFound");
        output!.Detail.Should().Be($"Genre '{targetGenre}' not found.");
    }

    [Trait("EndToEnd/Api", "Genre/Delete - Endpoints")]
    [Fact(DisplayName = nameof(DeleteGenreWithRelations))]
    public async Task DeleteGenreWithRelations()
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

        var (response, output) = await
            _fixture.ApiClient.Delete<object>(
                $"/genres/{targetGenre.Id}"
            );

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        output.Should().BeNull();
        var persistenceGenre = await _fixture
            .Persistence
            .GetByIdAsync(targetGenre.Id);
        persistenceGenre.Should().BeNull();
        var relations = await _fixture
            .Persistence
            .GetGenresCategoriesRelationsByIdAsync(targetGenre.Id);
        relations.Should().HaveCount(0);
    }

    public void Dispose() => _fixture.CleanPersistence();
}
