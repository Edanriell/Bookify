using Bookify.Application.Abstractions.Authentication;
using Bookify.Application.Abstractions.Messaging;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Users;

namespace Bookify.Application.Users.RegisterUser;

internal sealed class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, Guid>
{
	private readonly IAuthenticationService _authenticationService;
	private readonly IUnitOfWork _unitOfWork;
	private readonly IUserRepository _userRepository;

	public RegisterUserCommandHandler(
		IAuthenticationService authenticationService,
		IUserRepository userRepository,
		IUnitOfWork unitOfWork)
	{
		_authenticationService = authenticationService;
		_userRepository = userRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<Guid>> Handle(
		RegisterUserCommand request,
		CancellationToken cancellationToken)
	{
		// We are creating a new user by calling the user
		// create method. We are passing in our first name, last name and email to the value
		// object constructors, which are required to satisfy the create method. 
		// Static factory method
		var user = User.Create(
			new FirstName(request.FirstName),
			new LastName(request.LastName),
			new Email(request.Email));

		// Then we are calling the authentication service, which is
		// going to register our user with key cloak, and it is going to give
		// us back the identity ID, which we are going to set on the user
		var identityId = await _authenticationService.RegisterAsync(
							 user,
							 request.Password,
							 cancellationToken);

		// entity before persisting it in the database. 
		user.SetIdentityId(identityId);

		_userRepository.Add(user);

		await _unitOfWork.SaveChangesAsync();

		return user.Id;
	}
}