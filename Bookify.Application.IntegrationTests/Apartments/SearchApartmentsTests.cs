using Bookify.Application.Apartments.SearchApartments;
using Bookify.Application.IntegrationTests.Infrastructure;
using FluentAssertions;

namespace Bookify.Application.IntegrationTests.Apartments;

public class SearchApartmentsTests : BaseIntegrationTest
{
	public SearchApartmentsTests ( IntegrationTestWebAppFactory factory )
		: base (
				factory : factory
			)
	{
	}

	[ Fact ]
	public async Task SearchApartments_ShouldReturnEmptyList_WhenDateRangeInvalid()
	{
		// Arrange
		var query = new SearchApartmentsQuery (
				StartDate : new DateOnly (
						year : 2024,
						month : 1,
						day : 10
					),
				EndDate : new DateOnly (
						year : 2024,
						month : 1,
						day : 1
					)
			);

		// Act
		var result = await Sender.Send (
							 request : query
						 );

		// Assert
		result.IsSuccess.Should().
			BeTrue();
		result.Value.Should().
			BeEmpty();
	}

	[ Fact ]
	public async Task SearchApartments_ShouldReturnApartments_WhenDateRangeIsValid()
	{
		// Arrange
		var query = new SearchApartmentsQuery (
				StartDate : new DateOnly (
						year : 2024,
						month : 1,
						day : 1
					),
				EndDate : new DateOnly (
						year : 2024,
						month : 1,
						day : 10
					)
			);

		// Act
		var result = await Sender.Send (
							 request : query
						 );

		// Assert
		result.IsSuccess.Should().
			BeTrue();
		result.Value.Should().
			NotBeEmpty();
	}
}
