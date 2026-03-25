using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Application.Filters;
using FitnessTracking.Application.Paging;
using FitnessTracking.Application.Responses;
using FitnessTracking.Application.Sorting;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Features.Queries.GetWorkoutsByUserId;

public record GetWorkoutsByUserIdQuery(
    Guid UserId,
    WorkoutFilter Filter,
    SortParameters SortParameters,
    PageParameters PageParameters) : IQuery<Result<WorkoutListResponse, Error>>;
