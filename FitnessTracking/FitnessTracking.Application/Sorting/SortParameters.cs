using System.ComponentModel;
using System.Text.Json.Serialization;

namespace FitnessTracking.Application.Sorting;

public record SortParameters(
    string? OrderBy,
    [DefaultValue(SortDirection.Descending)] SortDirection? Direction);