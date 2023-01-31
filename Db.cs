using Microsoft.EntityFrameworkCore;

namespace ShaREPL;

public class Db : DbContext
{
    public DbSet<SharedEnvironmentDTO> Environments { get; set; }

	public Db(DbContextOptions<Db> options) : base(options)
    {

	}

    public async Task<SharedEnvironment?> FirstOrNull(Guid guid)
    {
        var dto = await Environments
           .FirstOrDefaultAsync(se => se.Guid == guid);

        if (dto is null)
        {
            return null;
        }

        var (id, ia, oa) = dto.Unpack();
        var se = new SharedEnvironment()
        {
            Guid = id,
            InputArchive = ia,
            Output = oa
        };
        return se;
    }
}
