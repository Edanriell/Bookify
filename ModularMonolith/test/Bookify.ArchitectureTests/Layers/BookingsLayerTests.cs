using Bookify.ArchitectureTests.Infrastructure;
using FluentAssertions;
using NetArchTest.Rules;

namespace Bookify.ArchitectureTests.Layers;

public class BookingsLayerTests : BaseTest
{
	[Fact]
	public void DomainLayer_ShouldNotHaveDependencyOn_ApplicationLayer()
	{
		TestResult result = Types.InAssembly(Bookings.DomainAssembly)
			.Should()
			.NotHaveDependencyOn(Bookings.ApplicationAssembly.GetName().Name)
			.GetResult();

		result.IsSuccessful.Should().BeTrue();
	}

	[Fact]
	public void DomainLayer_ShouldNotHaveDependencyOn_InfrastructureLayer()
	{
		TestResult result = Types.InAssembly(Bookings.DomainAssembly)
			.Should()
			.NotHaveDependencyOn(Bookings.InfrastructureAssembly.GetName().Name)
			.GetResult();

		result.IsSuccessful.Should().BeTrue();
	}

	[Fact]
	public void ApplicationLayer_ShouldNotHaveDependencyOn_InfrastructureLayer()
	{
		TestResult result = Types.InAssembly(Bookings.ApplicationAssembly)
			.Should()
			.NotHaveDependencyOn(Bookings.InfrastructureAssembly.GetName().Name)
			.GetResult();

		result.IsSuccessful.Should().BeTrue();
	}

	[Fact]
	public void ApplicationLayer_ShouldNotHaveDependencyOn_PresentationLayer()
	{
		TestResult result = Types.InAssembly(Bookings.ApplicationAssembly)
			.Should()
			.NotHaveDependencyOn(Bookings.PresentationAssembly.GetName().Name)
			.GetResult();

		result.IsSuccessful.Should().BeTrue();
	}

	[Fact]
	public void InfrastructureLayer_ShouldNotHaveDependencyOn_PresentationLayer()
	{
		TestResult result = Types.InAssembly(Bookings.InfrastructureAssembly)
			.Should()
			.NotHaveDependencyOn(Bookings.PresentationAssembly.GetName().Name)
			.GetResult();

		result.IsSuccessful.Should().BeTrue();
	}
}
