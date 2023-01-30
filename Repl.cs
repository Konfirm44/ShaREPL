using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace ShaREPL;

public class Repl
{
    ScriptState<object>? state;

    public bool HasState { get => state is not null; }

    public async Task Init()
    {
        var opt = ScriptOptions.Default.WithImports("System", "System.Math");
        state = await CSharpScript.RunAsync("\"hello world\"", opt);
    }

    public async Task<string> Evaluate(string input)
    {
        var result = "";
        try
        {
            state = await state!.ContinueWithAsync(input);
            result = state.ReturnValue?.ToString() ?? "no return value";
        }
        catch (CompilationErrorException ex)
        {
            result = ex.Message;
            //state = null;
        }
        return result;
    }
}
