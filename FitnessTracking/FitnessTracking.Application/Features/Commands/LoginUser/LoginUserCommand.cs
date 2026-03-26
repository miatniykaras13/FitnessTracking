using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Features.Commands.LoginUser;

public record LoginUserCommand(
    string Email,
    string Password) : ICommand<Result<LoginUserResponse, List<Error>>>;

