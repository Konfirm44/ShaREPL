using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace ShaREPL;

public class Repl
{
    ScriptState<object>? _state;

    public bool HasState { get => _state is not null; }

    public async Task Init()
    {
        var opt = ScriptOptions.Default.WithImports("System", "System.Math");
        _state = await CSharpScript.RunAsync("\"hello world\"", opt);
    }

    public async Task<string> Evaluate(string input)
    {
        var result = "";
        try
        {
            _state = await _state!.ContinueWithAsync(input);
            result = _state.ReturnValue?.ToString() ?? "";
        }
        catch (CompilationErrorException ex)
        {
            result = ex.Message;
        }
        return result;
    }
}
