using FC.Codeflix.Catalog.EndToEndTests.Common;
using GenreEntity = FC.Codeflix.Catalog.Domain.Entity.Genre;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genre.Common;
public class GenreBaseFixture : BaseFixture
{
    public GenrePersistence Persistence;

    public GenreBaseFixture() : base() =>
        Persistence = new GenrePersistence(CreateDbContext());

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

    public bool GetRandomBoolean() => new Random().NextDouble() <= 0.5;

}
