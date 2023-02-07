using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace ShaREPL;

public class SharedEnvironment
{
    public const string Prompt = "> ";

    public Guid Guid { get; init; } = Guid.NewGuid();

    public List<string> InputArchive { get; init; } = new();

    public IReadOnlyList<string> Output { get; init; } = new List<string>();

    public string Input { get; set; } = "";

    public event EventHandler? Update;

    private readonly Repl _repl = new();

    public SharedEnvironment()
    {
        //AddToOutput("Enter C# code:\n");
    }

    public void OnInput(ChangeEventArgs e)
    {
        Input = e.Value?.ToString() ?? "";
        Update?.Invoke(this, EventArgs.Empty);
    }

    public async Task OnKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            if (!_repl.HasState)
            {
               await _repl.Init();
            }

            if (Input.Last() is not '\n')
            {
                Input += "\n";
            }
            var input = Input;
            Input = "";
            InputArchive.Add(input);
            AddToOutput($"{Prompt}{input}");
            Update?.Invoke(this, EventArgs.Empty);

            var output = await ProcessLine(input);
            if (output.Length != 0)
            {
                AddToOutput(output + "\n");
            }
            
            Update?.Invoke(this, EventArgs.Empty);
        }
        if (e.Key == "ArrowUp")
        {
            OnInput(new ChangeEventArgs { Value = InputArchive.Last()});
        }
    }

    private async Task<string> ProcessLine(string line)
    {
        return await _repl.Evaluate(line);
    }

    private void AddToOutput(string text)
    {
        (Output as List<string>)!.Add(text);
    }
}
