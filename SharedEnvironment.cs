namespace ShaREPL;

public class SharedEnvironment
{
    public Guid Guid { get; init; } = new();

    public List<string> Input { get; init; } = new();
    public List<string> Output { get; init; } = new();

}
