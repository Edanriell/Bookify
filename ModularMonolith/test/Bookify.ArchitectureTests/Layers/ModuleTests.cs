using System.Reflection;
using Bookify.ArchitectureTests.Infrastructure;
using NetArchTest.Rules;

namespace Bookify.ArchitectureTests.Layers;

public class ModuleTests : BaseTest
{
	[Fact]
	public void UsersModule_ShouldNotHaveDependencyOn_AnyOtherModule()
	{
		string[] otherModules = [BookingsNamespace];
		string[] publicApiModules = [BookingsPublicApiNamespace, ApiNamespace];

		List<Assembly> usersAssemblies =
		[
			Users.DomainAssembly,
			Users.ApplicationAssembly,
			Users.InfrastructureAssembly,
			Users.PresentationAssembly
		];

		Types.InAssemblies(usersAssemblies)
			.That()
			.DoNotHaveDependencyOnAny(publicApiModules)
			.Should()
			.NotHaveDependencyOnAny(otherModules)
			.GetResult()
			.ShouldBeSuccessful();
	}

	[Fact]
	public void BookingsModule_ShouldNotHaveDependencyOn_AnyOtherModule()
	{
		string[] otherModules = [UsersNamespace];
		string[] publicApiModules = [UsersPublicApiNamespace, ApiNamespace];

		List<Assembly> bookingsAssemblies =
		[
			Bookings.DomainAssembly,
			Bookings.ApplicationAssembly,
			Bookings.InfrastructureAssembly,
			Bookings.PresentationAssembly
		];

		Types.InAssemblies(bookingsAssemblies)
			.That()
			.DoNotHaveDependencyOnAny(publicApiModules)
			.Should()
			.NotHaveDependencyOnAny(otherModules)
			.GetResult()
			.ShouldBeSuccessful();
	}
}
