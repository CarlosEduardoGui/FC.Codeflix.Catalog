using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
public class GenreRepository : IGenreRepository
{
    private readonly CodeflixCatelogDbContext _dbContext;
    private DbSet<Genre> _genres =>
        _dbContext.Set<Genre>();

    private DbSet<GenresCategories> _genresCategories =>
        _dbContext.Set<GenresCategories>();

    public GenreRepository(CodeflixCatelogDbContext dbContext)
        => _dbContext = dbContext;

    public async Task InsertAsync(Genre aggregate, CancellationToken cancellationToken)
    {
        await _genres.AddAsync(aggregate, cancellationToken);
        if (aggregate.Categories.Count > 0)
        {
            var relations = aggregate.Categories
                .Select(categoryId =>
                    new GenresCategories(categoryId, aggregate.Id)
                );

            await _genresCategories.AddRangeAsync(relations);
        }
    }

    public Task DeleteAsync(Genre aggregate, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Genre> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }


    public Task<SearchOutput<Genre>> SearchAsync(SearchInput input, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Genre aggregate, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }
}
