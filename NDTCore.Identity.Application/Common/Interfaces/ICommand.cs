using MediatR;
using NDTCore.Identity.Contracts.Common.Results;

namespace NDTCore.Identity.Application.Common.Interfaces;

/// <summary>
/// Marker interface for commands (write operations)
/// </summary>
public interface ICommand : IRequest<Result>
{
}

/// <summary>
/// Marker interface for commands with return value
/// </summary>
/// <typeparam name="TResponse">The response type</typeparam>
public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{
}

