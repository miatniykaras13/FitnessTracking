using FitnessTracking.Shared.Contracts;

namespace FitnessTracking.Application.Features.Commands.UpdateSets;

public record UpdateSetsResponse(IEnumerable<SetDto> Sets);
