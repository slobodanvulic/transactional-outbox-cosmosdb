using System.Text.Json;

namespace InventoryService.Processor;

internal class JsonSerializerConfiguration
{
    public static JsonSerializerOptions Default =>
        new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
}
