using System.Text.Json;
using System.Text.Json.Nodes;

namespace FitnessTracking.Application.Abstractions.Helpers;

public interface IMergePatchHelper
{
    T ApplyMergePatch<T>(T currentDto, JsonObject patch, JsonSerializerOptions? options = null);
}
