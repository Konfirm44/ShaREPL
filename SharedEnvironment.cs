using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace ShaREPL;

public class SharedEnvironment
{
    public Guid Guid { get; init; } = Guid.NewGuid();

    private List<string> InputArchive { get; init; } = new();
    public IReadOnlyList<string> Output { get; init; } = new List<string>();
    public string Input { get; set; } = "";

    public void OnInput(ChangeEventArgs e)
    {
        Input = e.Value?.ToString() ?? "";
        Update?.Invoke(default, EventArgs.Empty);
    }

    public void OnEnter(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            Input += "\n";
            InputArchive.Add(Input); 
            (Output as List<string>)!.Add(Input);
            Input = "";
            Update?.Invoke(default, EventArgs.Empty);
        }
    }

    public event EventHandler? Update;

}
