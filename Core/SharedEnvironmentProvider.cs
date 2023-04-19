using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;

namespace ShaREPL.Core;

public class SharedEnvironmentProvider
{
    private readonly IDbContextFactory<Db> _dbFactory;

    private readonly Dictionary<Guid, SharedEnvironment> _aliveEnvironments = new();

    public SharedEnvironmentProvider(IDbContextFactory<Db> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<SharedEnvironment> Get(Guid? guid = null)
    {
        SharedEnvironment? se;

        if (guid is null)
        {
            return await Create();
        }

        se = _aliveEnvironments.GetValueOrDefault(guid.Value);
        if (se is not null)
        {
            return se;
        }

        using var db = await _dbFactory.CreateDbContextAsync();
        se = await db.FirstOrNull(guid.Value);
        if (se is not null)
        {
            _aliveEnvironments.Add(se.Guid, se);
            return se;
        }

        se = new SharedEnvironment { Guid = guid.Value };
        _aliveEnvironments.Add(se.Guid, se);
        await Save(se, true);
        return se;
    }

    private async Task<SharedEnvironment> Create()
    {
        var se = new SharedEnvironment();
        _aliveEnvironments.Add(se.Guid, se);
        await Save(se, true);
        return se;
    }

    public async Task Save(SharedEnvironment se, bool addNotUpdate = false)
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        var dto = new SharedEnvironmentDTO(se);
        if (addNotUpdate)
        {
            db.Environments.Add(dto);
        }
        else
        {
            db.Environments.Update(dto);
        }
        await db.SaveChangesAsync();
    }

    public async Task OnKeyDown(KeyboardEventArgs args, SharedEnvironment se)
    {
        if (args.Key == "Enter")
        {
            await Save(se);
        }
    }
}
