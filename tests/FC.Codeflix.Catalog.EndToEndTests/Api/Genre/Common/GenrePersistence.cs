using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using Microsoft.EntityFrameworkCore;
using GenreEntity = FC.Codeflix.Catalog.Domain.Entity.Genre;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genre.Common;
public class GenrePersistence
{
    private readonly CodeflixCatelogDbContext _context;

    public GenrePersistence(CodeflixCatelogDbContext context)
        => _context = context;

    public async Task InsertListAsync(List<GenreEntity> categories)
    {
        await _context.Genres.AddRangeAsync(categories);
        await _context.SaveChangesAsync();
    }

    public async Task InsertGenresCategoriesRelationsListAsync(List<GenresCategories> relations)
    {
        await _context.GenresCategories.AddRangeAsync(relations);
        await _context.SaveChangesAsync();
    }

    public async Task<GenreEntity?> GetByIdAsync(Guid id) =>
       await _context.Genres
           .AsNoTracking()
           .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<List<GenresCategories>> GetGenresCategoriesRelationsByIdAsync(Guid id) =>
        await _context.GenresCategories
            .AsNoTracking()
            .Where(x => x.GenreId == id)
            .ToListAsync();
}
