using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FC.Codeflix.Catalog.IntegrationTests.Base;
using GenreEntity = FC.Codeflix.Catalog.Domain.Entity.Genre;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.Common;
public class GenreUseCaseBaseFixture : BaseFixture
{
    public GenreRepository GenreRepository(CodeflixCatelogDbContext dbContext)
        => new(dbContext);

    public List<GenreEntity> GetExampleListGenres(int count = 10)
        => Enumerable
            .Range(1, count)
            .Select(_ => GetExampleGenre())
            .ToList();

    public GenreEntity GetExampleGenre(bool? isActive = null, List<Guid>? categoriesIds = null,
      string? name = null)
    {

        var genre = new GenreEntity(
            name ?? GetValidGenreName(),
            isActive ?? GetRandomBoolean()
        );

        categoriesIds?.ForEach(genre.AddCategory);

        return genre;
    }

    public string GetValidGenreName()
    {
        var genreName = "";
        while (genreName.Length < 3)
            genreName = Faker.Commerce.Categories(1)[0];

        return genreName;
    }

}
