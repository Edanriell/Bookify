using Bogus;
using Bookify.Application.Abstractions.Data;
using Dapper;

namespace Bookify.Api.Extensions;

public static class SeedDataExtensions
{
	public static void SeedData(this IApplicationBuilder app)
	{
		try
		{
			Console.WriteLine("Starting data seeding...");
			using var scope = app.ApplicationServices.CreateScope();

			var sqlConnectionFactory = scope.ServiceProvider.GetRequiredService<ISqlConnectionFactory>();
			using var connection = sqlConnectionFactory.CreateConnection();

			// Check if data already exists
			var count = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM apartments");

			if (count > 0)
			{
				Console.WriteLine($"Data already exists. Found {count} apartments.");
				return;
			}

			var faker = new Faker();

			List<object> apartments = new();
			for (var i = 0; i < 100; i++)
				apartments.Add(new
							   {
								   Id = Guid.NewGuid(),
								   Name = faker.Company.CompanyName(),
								   Description = "Amazing view",
								   Country = faker.Address.Country(),
								   State = faker.Address.State(),
								   ZipCode = faker.Address.ZipCode(),
								   City = faker.Address.City(),
								   Street = faker.Address.StreetAddress(),
								   PriceAmount = faker.Random.Decimal(50, 1000),
								   PriceCurrency = "USD",
								   CleaningFeeAmount = faker.Random.Decimal(25, 200),
								   CleaningFeeCurrency = "USD",
								   Amenities = new List<int> { 1, 3 }, // Using constants instead of enum values
								   LastBookedOn = DateTime.MinValue
							   });

			Console.WriteLine($"Prepared {apartments.Count} apartments for seeding.");

			const string sql = @"
            INSERT INTO public.apartments
            (id, name, description, address_country, address_state, address_zip_code, address_city, address_street, price_amount, price_currency, cleaning_fee_amount, cleaning_fee_currency, amenities, last_booked_on_utc)
            VALUES(@Id, @Name, @Description, @Country, @State, @ZipCode, @City, @Street, @PriceAmount, @PriceCurrency, @CleaningFeeAmount, @CleaningFeeCurrency, @Amenities, @LastBookedOn);
        ";

			var rowsAffected = connection.Execute(sql, apartments);
			Console.WriteLine($"Successfully seeded {rowsAffected} apartments.");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error during data seeding: {ex.Message}");
			Console.WriteLine($"Stack trace: {ex.StackTrace}");
			if (ex.InnerException != null) Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
		}
	}
}