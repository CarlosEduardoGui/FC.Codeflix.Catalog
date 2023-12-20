﻿using FC.Codeflix.Catalog.Infra.Data.EF;
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
}
