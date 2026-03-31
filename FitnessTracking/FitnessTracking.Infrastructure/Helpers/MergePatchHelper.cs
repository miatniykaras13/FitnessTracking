using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using FitnessTracking.Application.Abstractions.Helpers;
using FitnessTracking.Infrastructure.Exceptions;

namespace FitnessTracking.Infrastructure.Helpers;

public class MergePatchHelper : IMergePatchHelper
{
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        WriteIndented = true
    };

    public T ApplyMergePatch<T>(T dto, JsonObject patch, JsonSerializerOptions? options = null)
    {
        options ??= _serializerOptions;

        var target = JsonSerializer.SerializeToNode(dto, options) as JsonObject ?? new JsonObject();

        Merge(patch, target);

        var patchedDto = target.Deserialize<T>(options) ??
                         throw new MergePatchDeserializationException("Failed to deserialize merged JSON to DTO.");

        return patchedDto;
    }

    private void Merge(JsonObject patch, JsonObject target)
    {
        foreach (var item in patch)
        {
            var key = item.Key;
            var jsonNode = item.Value;

            if (jsonNode is null || (jsonNode is JsonValue jv && jv.GetValue<object?>() is null))
            {
                target.Remove(key);
                continue;
            }

            if (jsonNode is JsonObject patchChild && target[key] is JsonObject targetChild)
            {
                Merge(patchChild, targetChild);
            }
            else
            {
                target[key] = jsonNode.DeepClone();
            }
        }
    }
}