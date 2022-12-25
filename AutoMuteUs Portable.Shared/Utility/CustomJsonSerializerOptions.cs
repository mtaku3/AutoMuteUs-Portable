using System.Text.Json;
using System.Text.Json.Serialization;

namespace AutoMuteUs_Portable.Shared.Utility;

public static partial class Utils
{
    public static JsonSerializerOptions CustomJsonSerializerOptions = new()
    {
        Converters =
        {
            new JsonStringEnumConverter()
        },
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
}