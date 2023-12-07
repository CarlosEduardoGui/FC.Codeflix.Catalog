using FC.Codeflix.Catalog.Application.Exceptions;
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
        _genresCategories.RemoveRange(
            _genresCategories.Where(x => x.GenreId == aggregate.Id)
        );

        _genres.Remove(aggregate);

        return Task.CompletedTask;
    }

    public async Task<Genre> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var genre = await _genres
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        NotFoundException.ThrowIfNull(
            genre,
            string.Format(ConstantsMessages.GenreIdNotFound, id)
        );

        var categoryIds = await _genresCategories
            .Where(relation => relation.Genre!.Id == genre!.Id)
            .Select(categoryId => categoryId.CategoryId)
            .ToListAsync(cancellationToken);

        categoryIds.ForEach(genre!.AddCategory);

        return genre;
    }


    public Task<SearchOutput<Genre>> SearchAsync(SearchInput input, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateAsync(Genre aggregate, CancellationToken cancellation)
    {
        _genres.Update(aggregate);
        _genresCategories.RemoveRange(
            _genresCategories.Where(x => x.GenreId == aggregate.Id)
        );
        if (aggregate.Categories.Count > 0)
        {
            var relations = aggregate.Categories
                .Select(categoryId =>
                    new GenresCategories(categoryId, aggregate.Id)
                );

            await _genresCategories.AddRangeAsync(relations);
        }
    }
}
