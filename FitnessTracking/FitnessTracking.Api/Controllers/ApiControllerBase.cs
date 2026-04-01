using System.Security.Claims;
using System.Text.Json.Nodes;
using CSharpFunctionalExtensions;
using FitnessTracking.Api.Exceptions;
using FitnessTracking.Api.Extensions;
using FitnessTracking.Shared.Errors;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTracking.Api.Controllers;

public abstract class ApiControllerBase(ISender sender) : ControllerBase
{
    protected async Task<IActionResult> Send<TResponse>(
        IRequest<Result<TResponse, List<Error>>> request,
        CancellationToken ct,
        Func<TResponse, IActionResult>? onSuccess = null)
    {
        var response = await sender.Send(request, ct);
        return response.ToActionResult(this, onSuccess);
    }

    protected async Task<IActionResult> Send(
        IRequest<UnitResult<List<Error>>> request,
        CancellationToken ct,
        Func<IActionResult>? onSuccess = null)
    {
        var response = await sender.Send(request, ct);
        return response.ToActionResult(this, onSuccess);
    }

    protected bool TryGetCurrentUserId(out Guid userId)
    {
        var userIdRaw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdRaw is null || !Guid.TryParse(userIdRaw, out userId))
        {
            userId = Guid.Empty;
            return false;
        }
        return true;
    }

    protected static async Task<byte[]> ReadFileAsync(IFormFile file, CancellationToken ct)
    {
        await using var stream = file.OpenReadStream();
        await using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream, ct);
        return memoryStream.ToArray();
    }

    protected static JsonObject RequirePatch(JsonObject? patchObject)
    {

        if (patchObject is null)
        {
            throw new InvalidPatchDocumentException("Patch body must be a JsonObject.");
        }

        return patchObject;
    }
}



