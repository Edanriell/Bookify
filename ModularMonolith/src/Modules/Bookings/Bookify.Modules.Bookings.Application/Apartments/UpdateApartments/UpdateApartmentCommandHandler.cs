using Bookify.Application.Abstractions.Messaging;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Shared;
using Bookify.Modules.Bookings.Domain.Apartments;

namespace Bookify.Modules.Bookings.Application.Apartments.UpdateApartments;

internal sealed class UpdateApartmentCommandHandler : ICommandHandler<UpdateApartmentCommand>
{
	private readonly IApartmentRepository _apartmentRepository;
	private readonly IUnitOfWork _unitOfWork;

	public UpdateApartmentCommandHandler(
		IApartmentRepository apartmentRepository,
		IUnitOfWork unitOfWork)
	{
		_apartmentRepository = apartmentRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(UpdateApartmentCommand request, CancellationToken cancellationToken)
	{
		Apartment? apartment = await _apartmentRepository.GetByIdAsync(request.ApartmentId, cancellationToken);

		if (apartment is null)
		{
			return Result.Failure(ApartmentErrors.NotFound);
		}

		var price = new Money(
			request.PriceAmount,
			Currency.FromCode(request.PriceAmountCurrency));

		var cleaningFee = new Money(
			request.CleaningFeeAmount,
			Currency.FromCode(request.PriceAmountCurrency));

		apartment.Update(price, cleaningFee, request.Amenities);

		await _unitOfWork.SaveChangesAsync(cancellationToken);

		return Result.Success();
	}
}
