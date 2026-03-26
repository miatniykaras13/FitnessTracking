namespace FitnessTracking.Application.Pagination;

public record PageParameters(int PageNumber = 1, int PageSize = 10);