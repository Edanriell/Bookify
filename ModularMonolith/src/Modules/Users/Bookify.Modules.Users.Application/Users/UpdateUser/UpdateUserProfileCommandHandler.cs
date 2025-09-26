using Bookify.Application.Abstractions.Messaging;
using Bookify.Domain.Abstractions;
using Bookify.Modules.Users.Domain.Users;

namespace Bookify.Modules.Users.Application.Users.UpdateUser;

internal sealed class UpdateUserProfileCommandHandler : ICommandHandler<UpdateUserProfileCommand>
{
	private readonly IUnitOfWork _unitOfWork;
	private readonly IUserRepository _userRepository;

	public UpdateUserProfileCommandHandler(
		IUserRepository userRepository,
		IUnitOfWork unitOfWork)
	{
		_userRepository = userRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
	{
		User? user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

		if (user is null)
		{
			return Result.Failure(UserErrors.NotFound);
		}

		user.Update(
			new FirstName(request.FirstName),
			new LastName(request.LastName));

		await _unitOfWork.SaveChangesAsync(cancellationToken);

		return Result.Success();
	}
}
