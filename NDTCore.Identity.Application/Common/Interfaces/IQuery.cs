using MediatR;
using NDTCore.Identity.Contracts.Common.Results;

namespace NDTCore.Identity.Application.Common.Interfaces;

/// <summary>
/// Marker interface for queries (read operations)
/// </summary>
/// <typeparam name="TResponse">The response type</typeparam>
public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}

