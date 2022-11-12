using System.Text.Json;

namespace InvoicinService.Processor;

internal class JsonSerializerConfiguration
{
    public static JsonSerializerOptions Default =>
        new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
}
