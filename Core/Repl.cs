using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace ShaREPL.Core;

public class Repl
{
    ScriptState<object>? _state;

    public bool HasState { get => _state is not null; }

    public async Task Init()
    {
        var opt = ScriptOptions.Default
            .WithReferences(
                typeof(Enumerable).Assembly,
                typeof(System.Text.Json.JsonSerializer).Assembly
            )
            .WithImports(
            "System",
            "System.Math",
            "System.Collections.Generic",
            "System.Linq",
            "System.Text",
            "System.Text.Json"
            );
        var print = @"string print(object o) => JsonSerializer.Serialize(o);";
        _state = await CSharpScript.RunAsync(print, opt);
    }

    public async Task<string> Evaluate(string input)
    {
        var result = "";
        try
        {
            _state = await _state!.ContinueWithAsync(input);
            result = _state.ReturnValue?.ToString() ?? "";
        }
        catch (Exception ex)
        {
            result = ex.Message;
        }
        return result;
    }
}
