using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;

namespace ShaREPL;

public class SharedEnvironmentProvider
{
    private readonly IDbContextFactory<Db> dbFactory;

    private Dictionary<Guid, SharedEnvironment> aliveEnvironments = new();

    public SharedEnvironmentProvider(IDbContextFactory<Db> dbFactory)
    {
        this.dbFactory = dbFactory;
    }

    public async Task<SharedEnvironment> Get(Guid? guid = null)
    {
        SharedEnvironment? se;

        if (guid is null)
        {
            return await Create();
        }

        se = aliveEnvironments.GetValueOrDefault(guid.Value);
        if (se is not null)
        {
            return se;
        }

        using var db = await dbFactory.CreateDbContextAsync();
        se = await db.FirstOrNull(guid.Value);
        if (se is not null)
        {
            aliveEnvironments.Add(se.Guid, se);
            return se;
        }

        se = new SharedEnvironment { Guid = guid.Value };
        aliveEnvironments.Add(se.Guid, se);
        await Save(se, true);
        return se;
    }

    private async Task<SharedEnvironment> Create()
    {
        var se = new SharedEnvironment();
        aliveEnvironments.Add(se.Guid, se);
        await Save(se, true);
        return se;
    }

    public async Task Save(SharedEnvironment se, bool addNotUpdate = false)
    {
        using var db = await dbFactory.CreateDbContextAsync();
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
