using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace ShaREPL;

public class SharedEnvironmentDTO
{
    [Key]
    public Guid Guid { get; init; } = default;

    public string InputArchive { get; init; } = "";

    public string OutputArchive { get; init; } = "";

    public SharedEnvironmentDTO()
    {

    }

    public SharedEnvironmentDTO(SharedEnvironment se)
    {
        Guid = se.Guid;
        InputArchive = JsonSerializer.Serialize(se.InputArchive);
        OutputArchive = JsonSerializer.Serialize(se.Output);
    }

    public (Guid Guid, List<string> InputArchive, List<string> OutputArchive) Unpack()
    {
        var ia = JsonSerializer.Deserialize<List<string>>(InputArchive);
        var oa = JsonSerializer.Deserialize<List<string>>(OutputArchive);
        return (Guid, ia!, oa!);
    }

    public string ToJson()
    {
        return JsonSerializer.Serialize(this);
    }

    public override string ToString()
    {
        var oa = JsonSerializer.Deserialize<List<string>>(OutputArchive);
        var str = string.Join("", oa!);

        return $"{Guid}\n\n{str}";
    }

}