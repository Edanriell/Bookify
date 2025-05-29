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

	public RegisterUserCommandHandler ( IAuthenticationService authenticationService,
										IUserRepository userRepository,
										IUnitOfWork unitOfWork )
	{
		_authenticationService = authenticationService;
		_userRepository = userRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<Guid>> Handle ( RegisterUserCommand request,
											 CancellationToken cancellationToken )
	{
		var user = User.Create (
				firstName : new FirstName (
						Value : request.FirstName
					),
				lastName : new LastName (
						Value : request.LastName
					),
				email : new Email (
						Value : request.Email
					)
			);

		var identityId = await _authenticationService.RegisterAsync (
								 user : user,
								 password : request.Password,
								 cancellationToken : cancellationToken
							 );

		user.SetIdentityId (
				identityId : identityId
			);

		_userRepository.Add (
				user : user
			);

		await _unitOfWork.SaveChangesAsync (
				cancellationToken : cancellationToken
			);

		return user.Id;
	}
}
