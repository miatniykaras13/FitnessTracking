namespace FitnessTracking.Shared.Exceptions;

public sealed class MissingConfigurationException(string key)
    : Exception($"Configuration value '{key}' is required.");

