using System.Text.Json.Serialization;

namespace FitnessTracking.Application.Sorting;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SortDirection
{
    Descending,
    Ascending
}
