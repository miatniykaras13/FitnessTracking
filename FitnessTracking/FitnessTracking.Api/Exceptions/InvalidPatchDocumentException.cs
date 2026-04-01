namespace FitnessTracking.Api.Exceptions;

public sealed class InvalidPatchDocumentException(string message) : Exception(message);

