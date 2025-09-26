using Bookify.ArchitectureTests.Infrastructure;
using FluentAssertions;
using NetArchTest.Rules;

namespace Bookify.ArchitectureTests.Layers;

public class UsersLayerTests : BaseTest
{
	[Fact]
	public void DomainLayer_ShouldNotHaveDependencyOn_ApplicationLayer()
	{
		TestResult result = Types.InAssembly(Users.DomainAssembly)
			.Should()
			.NotHaveDependencyOn(Users.ApplicationAssembly.GetName().Name)
			.GetResult();

		result.IsSuccessful.Should().BeTrue();
	}

	[Fact]
	public void DomainLayer_ShouldNotHaveDependencyOn_InfrastructureLayer()
	{
		TestResult result = Types.InAssembly(Users.DomainAssembly)
			.Should()
			.NotHaveDependencyOn(Users.InfrastructureAssembly.GetName().Name)
			.GetResult();

		result.IsSuccessful.Should().BeTrue();
	}

	[Fact]
	public void ApplicationLayer_ShouldNotHaveDependencyOn_InfrastructureLayer()
	{
		TestResult result = Types.InAssembly(Users.ApplicationAssembly)
			.Should()
			.NotHaveDependencyOn(Users.InfrastructureAssembly.GetName().Name)
			.GetResult();

		result.IsSuccessful.Should().BeTrue();
	}

	[Fact]
	public void ApplicationLayer_ShouldNotHaveDependencyOn_PresentationLayer()
	{
		TestResult result = Types.InAssembly(Users.ApplicationAssembly)
			.Should()
			.NotHaveDependencyOn(Users.PresentationAssembly.GetName().Name)
			.GetResult();

		result.IsSuccessful.Should().BeTrue();
	}

	[Fact]
	public void InfrastructureLayer_ShouldNotHaveDependencyOn_PresentationLayer()
	{
		TestResult result = Types.InAssembly(Users.InfrastructureAssembly)
			.Should()
			.NotHaveDependencyOn(Users.PresentationAssembly.GetName().Name)
			.GetResult();

		result.IsSuccessful.Should().BeTrue();
	}
}
