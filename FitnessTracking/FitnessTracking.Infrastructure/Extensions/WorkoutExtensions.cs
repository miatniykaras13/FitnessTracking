using System.Linq.Expressions;
using FitnessTracking.Application.Filters;
using FitnessTracking.Application.Pagination;
using FitnessTracking.Application.Sorting;
using FitnessTracking.Domain.Enums;
using FitnessTracking.Domain.Models;

namespace FitnessTracking.Infrastructure.Extensions;

public static class WorkoutExtensions
{
    public static IQueryable<Workout> Filter(this IQueryable<Workout> query, WorkoutFilter filter)
    {
        if (!string.IsNullOrEmpty(filter.Type))
            query = query.Where(w => w.Type == Enum.Parse<WorkoutType>(filter.Type, true));

        if (filter.WorkoutDateFrom is not null)
            query = query.Where(w => w.WorkoutDate >= filter.WorkoutDateFrom);

        if (filter.WorkoutDateTo is not null)
            query = query.Where(w => w.WorkoutDate <= filter.WorkoutDateTo);

        if (filter.DurationFrom is not null)
            query = query.Where(w => w.Duration >= filter.DurationFrom);

        if (filter.DurationTo is not null)
            query = query.Where(w => w.Duration <= filter.DurationTo);

        return query;
    }

    public static IQueryable<Workout> Page(this IQueryable<Workout> query, PageParameters pageParameters) =>
        query.Skip((pageParameters.PageNumber - 1) * pageParameters.PageSize).Take(pageParameters.PageSize);

    public static IQueryable<Workout> Sort(this IQueryable<Workout> query, SortParameters sortParameters) =>
        sortParameters.Direction == SortDirection.Descending
            ? query.OrderByDescending(GetKeySelector(sortParameters.OrderBy))
            : query.OrderBy(GetKeySelector(sortParameters.OrderBy));


    private static Expression<Func<Workout, object>> GetKeySelector(string? orderBy)
    {
        if (string.IsNullOrEmpty(orderBy))
            return w => w.CreatedAt;
        return orderBy switch
        {
            nameof(Workout.CaloriesBurned) => w => w.CaloriesBurned,
            nameof(Workout.WorkoutDate) => w => w.WorkoutDate,
            _ => w => w.CreatedAt
        };
    }
}