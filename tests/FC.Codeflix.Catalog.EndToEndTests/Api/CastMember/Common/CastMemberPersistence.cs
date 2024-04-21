using FC.Codeflix.Catalog.Infra.Data.EF;
using Microsoft.EntityFrameworkCore;
using CastMemberEntity = FC.Codeflix.Catalog.Domain.Entity.CastMember;


namespace FC.Codeflix.Catalog.EndToEndTests.Api.CastMember.Common;
public class CastMemberPersistence
{
    private readonly CodeflixCatelogDbContext _context;

    public CastMemberPersistence(CodeflixCatelogDbContext context)
        => _context = context;

    public async Task InsertListAsync(List<CastMemberEntity> categories)
    {
        await _context.CastMembers.AddRangeAsync(categories);
        await _context.SaveChangesAsync();
    }

    public async Task<CastMemberEntity?> GetByIdAsync(Guid id) =>
       await _context.CastMembers
           .AsNoTracking()
           .FirstOrDefaultAsync(x => x.Id == id);
}
