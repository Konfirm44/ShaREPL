namespace ShaREPL;

public class SharedEnvironmentProvider
{
    private List<SharedEnvironment> SharedEnvironments { get; } = new();

    public SharedEnvironment GetSharedEnvironment(Guid? guid = null)
    {
        var se = SharedEnvironments.FirstOrDefault(se => se.Guid == guid);
        if (se is null)
        {
            se = new SharedEnvironment();
            SharedEnvironments.Add(se);
        }
        return se;
    }
}
