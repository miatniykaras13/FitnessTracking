using FitnessTracking.Application.Constants;

namespace FitnessTracking.Application.Pagination;

public record PageParameters(
    int PageNumber = ValidationConstants.DefaultPageNumber,
    int PageSize = ValidationConstants.DefaultPageSize);
