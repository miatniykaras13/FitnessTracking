using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Features.Commands.RegisterUser;

public record RegisterUserCommand(
    string Email,
    string Password) : ICommand<Result<RegisterUserResponse, List<Error>>>;

