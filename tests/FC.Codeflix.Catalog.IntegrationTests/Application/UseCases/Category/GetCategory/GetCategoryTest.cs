using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.UseCases.Category.GetCategory;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FluentAssertions;
using Xunit;
using GetCategoryUseCase = FC.Codeflix.Catalog.Application.UseCases.Category.GetCategory.GetCategory;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.GetCategory;

[Collection(nameof(GetCategoryTestFixture))]
public class GetCategoryTest
{
    private readonly GetCategoryTestFixture _fixture;

    public GetCategoryTest(GetCategoryTestFixture fixture) =>
        _fixture = fixture;

    [Trait("Integration/Application", "GetCategory - Use Cases")]
    [Fact(DisplayName = nameof(GetCategory))]
    public async Task GetCategory()
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleCategory = _fixture.GetExampleCategory();
        dbContext.Categories.Add(exampleCategory);
        await dbContext.SaveChangesAsync();
        var repository = new CategoryRepository(dbContext);

        var input = new GetCategoryInput(exampleCategory.Id);
        var useCase = new GetCategoryUseCase(repository);

        var outPut = await useCase.Handle(input, CancellationToken.None);

        outPut.Should().NotBeNull();
        outPut.Id.Should().Be(exampleCategory.Id);
        outPut.Name.Should().Be(exampleCategory.Name);
        outPut.Description.Should().Be(exampleCategory.Description);
        outPut.IsActive.Should().Be(exampleCategory.IsActive);
        outPut.CreatedAt.Should().Be(exampleCategory.CreatedAt);
    }

    [Trait("Integration/Application", "GetCategory - Use Cases")]
    [Fact(DisplayName = nameof(NotFoundExceptionWhenCategoryDoesntExist))]
    public async Task NotFoundExceptionWhenCategoryDoesntExist()
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleCategory = _fixture.GetExampleCategory();
        dbContext.Categories.Add(exampleCategory);
        await dbContext.SaveChangesAsync();
        var repository = new CategoryRepository(dbContext);
        var input = new GetCategoryInput(Guid.NewGuid());
        var useCase = new GetCategoryUseCase(repository);

        var task = async () => await useCase.Handle(input, CancellationToken.None);

        await task.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Category '{input.Id}' not found.");
    }
}
