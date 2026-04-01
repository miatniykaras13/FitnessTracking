namespace FitnessTracking.Shared.Errors;

public enum ErrorType
{
    /// <summary>
    /// An unknown or unexpected error.
    /// </summary>
    Unknown,

    /// <summary>
    /// The requested resource was not found.
    /// </summary>
    NotFound,

    /// <summary>
    /// An internal server error occurred.
    /// </summary>
    Internal,

    /// <summary>
    /// A conflict with the current resource state.
    /// </summary>
    Conflict,

    /// <summary>
    /// Input data failed validation.
    /// </summary>
    Validation,

    /// <summary>
    /// The operation is forbidden for the current user.
    /// </summary>
    Forbidden,

    /// <summary>
    /// The user is unauthorized.
    /// </summary>
    Unauthorized,
}
