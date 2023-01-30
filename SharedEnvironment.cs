﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace ShaREPL;

public class SharedEnvironment
{
    public Guid Guid { get; init; } = Guid.NewGuid();

    private List<string> InputArchive { get; init; } = new();
    public IReadOnlyList<string> Output { get; init; } = new List<string>();
    public string Input { get; set; } = "";

    private readonly Repl _repl = new();

    public event EventHandler? Update;

    public SharedEnvironment() 
    {
        AddToOutput("Enter C# code:\n");
    }

    public void OnInput(ChangeEventArgs e)
    {
        Input = e.Value?.ToString() ?? "";
        Update?.Invoke(this, EventArgs.Empty);
    }

    public async Task OnEnter(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            if (!_repl.HasState)
            {
               await _repl.Init();
            }

            var input = Input + "\n";
            Input = "";
            InputArchive.Add(input);
            AddToOutput(input);
            Update?.Invoke(this, EventArgs.Empty);

            var output = await ProcessLine(input);
            AddToOutput(output + "\n");
            
            Update?.Invoke(this, EventArgs.Empty);
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
