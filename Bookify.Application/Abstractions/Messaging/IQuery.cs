using Bookify.Domain.Abstractions;
using MediatR;

namespace Bookify.Application.Abstractions.Messaging;

// Generic interface with a TResponse argument, TResponse should specify what is the
// type returned by this query. 
// Essentially query is going to be a mediator request returning a result of TResponse object.
public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}