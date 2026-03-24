using FitnessTracking.Application.Filters;
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
}