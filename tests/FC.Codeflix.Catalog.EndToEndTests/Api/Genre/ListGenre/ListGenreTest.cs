using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using FC.Codeflix.Catalog.Application.UseCases.Genre.ListGenre;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.EndToEndTests.ApiModels;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using FluentAssertions;
using System.Net;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genre.ListGenre;

[Collection(nameof(ListGenreTestFixture))]
public class ListGenreTest : IDisposable
{
    private readonly ListGenreTestFixture _fixture;

    public ListGenreTest(ListGenreTestFixture fixture)
        => _fixture = fixture;

    [Trait("EndToEnd/API", "Genre/ListGenres - Endpoints")]
    [Fact(DisplayName = nameof(ListGenresOk))]
    public async Task ListGenresOk()
    {
        var exampleGeres = _fixture.GetExampleListGenres();
        await _fixture.Persistence.InsertListAsync(exampleGeres);
        var input = new ListGenresInput(1, exampleGeres.Count);

        var (response, output) = await _fixture.ApiClient.Get<TestApiResponseList<GenreModelOutPut>>("/genres", input);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Meta.Should().NotBeNull();
        output.Meta!.Total.Should().Be(exampleGeres.Count);
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        foreach (var item in output.Data!)
        {
            var outputItem = exampleGeres.FirstOrDefault(x => x.Id == item.Id);
            outputItem.Should().NotBeNull();
            item.Name.Should().Be(outputItem!.Name);
            item.IsActive.Should().Be(outputItem.IsActive);
            item.CreatedAt.Should().Be(outputItem.CreatedAt);
        }
    }

    [Trait("EndToEnd/API", "Genre/ListGenres - Endpoints")]
    [Fact(DisplayName = nameof(ListGenresWithRelations))]
    public async Task ListGenresWithRelations()
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
        await _fixture.CategoriesPersistence.InsertListAsync(categoryList);
        await _fixture.Persistence.InsertListAsync(exampleGeres);
        await _fixture.Persistence.InsertGenresCategoriesRelationsListAsync(genresCategories);
        var input = new ListGenresInput(1, exampleGeres.Count);

        var (response, output) = await _fixture.ApiClient.Get<TestApiResponseList<GenreModelOutPut>>("/genres", input);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Meta.Should().NotBeNull();
        output.Meta!.Total.Should().Be(exampleGeres.Count);
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        foreach (var item in output.Data!)
        {
            var outputItem = exampleGeres.FirstOrDefault(x => x.Id == item.Id);
            outputItem.Should().NotBeNull();
            item.Name.Should().Be(outputItem!.Name);
            item.IsActive.Should().Be(outputItem.IsActive);
            item.CreatedAt.Should().Be(outputItem.CreatedAt);
            var relatedCategoriesIds = item.Categories.Select(relation => relation.Id).ToList();
            relatedCategoriesIds.Should().NotBeNull();
            relatedCategoriesIds.Should().BeEquivalentTo(outputItem.Categories);
            item.Categories.ToList().ForEach(outputRelatedCategory =>
            {
                var exampleCategory = categoryList.Find(x => x.Id == outputRelatedCategory.Id);
                exampleCategory.Should().NotBeNull();
                outputRelatedCategory.Name.Should().Be(exampleCategory!.Name);
            });
        }
    }

    [Trait("EndToEnd/API", "Genre/ListGenres - Endpoints")]
    [Fact(DisplayName = nameof(EmptyWhenThereAreNotItems))]
    public async Task EmptyWhenThereAreNotItems()
    {
        var input = new ListGenresInput();

        var (response, output) = await _fixture.ApiClient.Get<TestApiResponseList<GenreModelOutPut>>("/genres", input);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Meta.Should().NotBeNull();
        output.Meta!.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        output.Meta.Total.Should().Be(0);
        output.Data!.Count.Should().Be(0);
    }

    [Trait("EndToEnd/API", "Genre/ListGenres - Endpoints")]
    [Theory(DisplayName = nameof(ListPagineted))]
    [InlineData(10, 1, 5, 5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task ListPagineted(
        int quantityToGenerate,
        int page,
        int perPage,
        int expectedQuantityItems
    )
    {
        var exampleGeres = _fixture.GetExampleListGenres(quantityToGenerate);
        await _fixture.Persistence.InsertListAsync(exampleGeres);
        var input = new ListGenresInput(page, perPage);

        var (response, output) = await _fixture.ApiClient.Get<TestApiResponseList<GenreModelOutPut>>("/genres", input);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Meta.Should().NotBeNull();
        output.Meta!.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        output.Meta.Total.Should().Be(exampleGeres.Count);
        output.Data!.Count.Should().Be(expectedQuantityItems);
        foreach (var item in output.Data!)
        {
            var outputItem = exampleGeres.FirstOrDefault(x => x.Id == item.Id);
            outputItem.Should().NotBeNull();
            item.Name.Should().Be(outputItem!.Name);
            item.IsActive.Should().Be(outputItem.IsActive);
            item.CreatedAt.Should().Be(outputItem.CreatedAt);
        }
    }

    [Trait("EndToEnd/API", "Genre/ListGenres - Endpoints")]
    [Theory(DisplayName = nameof(SearchByText))]
    [InlineData("Action", 1, 5, 1, 1)]
    [InlineData("Horror", 1, 5, 3, 3)]
    [InlineData("Sci-fi", 1, 5, 5, 6)]
    [InlineData("Sci-fi", 1, 2, 2, 6)]
    [InlineData("Sci-fi", 2, 5, 1, 6)]
    [InlineData("Robots", 1, 5, 2, 2)]
    public async Task SearchByText(
            string search,
            int page,
            int perPage,
            int expectedQuantityItemsReturned,
            int expectedQuantityTotalItems
    )
    {
        var exampleGeres = _fixture.GetExampleListGenresByNames(new List<string>()
        {
            "Action",
            "Horror",
            "Horror - Robots",
            "Horror - Based on Real Facts",
            "Drama",
            "Sci-fi IA",
            "Sci-fi Future",
            "Sci-fi",
            "Sci-fi Robots",
            "Sci-fi StarWars",
            "Sci-fi StarTrek"
        });
        await _fixture.Persistence.InsertListAsync(exampleGeres);
        var input = new ListGenresInput(page, perPage, search);

        var (response, output) = await _fixture.ApiClient.Get<TestApiResponseList<GenreModelOutPut>>("/genres", input);

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Meta.Should().NotBeNull();
        output.Meta!.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        output.Meta.Total.Should().Be(expectedQuantityTotalItems);
        output.Data!.Count.Should().Be(expectedQuantityItemsReturned);
        foreach (var item in output.Data!)
        {
            var outputItem = exampleGeres.FirstOrDefault(x => x.Id == item.Id);
            outputItem.Should().NotBeNull();
            item.Name.Should().Be(outputItem!.Name);
            item.IsActive.Should().Be(outputItem.IsActive);
            item.CreatedAt.Should().Be(outputItem.CreatedAt);
        }
    }

    [Trait("EndToEnd/API", "Genre/ListGenres - Endpoints")]
    [Theory(DisplayName = nameof(SearchOrdered))]
    [InlineData("name", "asc")]
    [InlineData("name", "desc")]
    [InlineData("id", "asc")]
    [InlineData("id", "desc")]
    [InlineData("createdAt", "asc")]
    [InlineData("createdAt", "desc")]
    [InlineData("", "asc")]
    public async Task SearchOrdered(string orderby, string order)
    {
        var exampleGeres = _fixture.GetExampleListGenres();
        await _fixture.Persistence.InsertListAsync(exampleGeres);
        var searchOrder = order.ToLower() == "asc" ? SearchOrder.ASC : SearchOrder.DESC;
        var input = new ListGenresInput(
            1,
            10,
            "",
            orderby,
            searchOrder
        );

        var (response, output) = await _fixture.ApiClient.Get<TestApiResponseList<GenreModelOutPut>>("/genres", input);

        var expectedOrderedList = _fixture.CloneGenresListOrdered(
           exampleGeres,
           orderby,
           searchOrder
       );
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        output.Should().NotBeNull();
        output.Data.Should().NotBeNull();
        output.Meta.Should().NotBeNull();
        output.Meta!.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        output.Meta.Total.Should().Be(10);
        output.Data!.Count.Should().Be(10);
        for (int i = 0; i < expectedOrderedList.Count; i++)
        {
            var expectedItem = expectedOrderedList[i];
            var outPutItem = output.Data[i];

            expectedItem.Should().NotBeNull();
            outPutItem!.Name.Should().Be(expectedItem!.Name);
            outPutItem.IsActive.Should().Be(expectedItem.IsActive);
            outPutItem.CreatedAt.Should().Be(expectedItem.CreatedAt);
        }
    }

    public void Dispose() => _fixture.CleanPersistence();
}
