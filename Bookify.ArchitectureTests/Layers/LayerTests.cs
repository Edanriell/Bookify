using Bookify.ArchitectureTests.Infrastructure;
using FluentAssertions;
using NetArchTest.Rules;

namespace Bookify.ArchitectureTests.Layers;

public class LayerTests : BaseTest
{
	[ Fact ]
	public void DomainLayer_ShouldNotHaveDependencyOn_ApplicationLayer()
	{
		var result = Types.InAssembly (
					assembly : DomainAssembly
				).
			Should().
			NotHaveDependencyOn (
					dependency : ApplicationAssembly.GetName().
						Name
				).
			GetResult();

		result.IsSuccessful.Should().
			BeTrue();
	}

	[ Fact ]
	public void DomainLayer_ShouldNotHaveDependencyOn_InfrastructureLayer()
	{
		var result = Types.InAssembly (
					assembly : DomainAssembly
				).
			Should().
			NotHaveDependencyOn (
					dependency : InfrastructureAssembly.GetName().
						Name
				).
			GetResult();

		result.IsSuccessful.Should().
			BeTrue();
	}

	[ Fact ]
	public void ApplicationLayer_ShouldNotHaveDependencyOn_InfrastructureLayer()
	{
		var result = Types.InAssembly (
					assembly : ApplicationAssembly
				).
			Should().
			NotHaveDependencyOn (
					dependency : InfrastructureAssembly.GetName().
						Name
				).
			GetResult();

		result.IsSuccessful.Should().
			BeTrue();
	}

	[ Fact ]
	public void ApplicationLayer_ShouldNotHaveDependencyOn_PresentationLayer()
	{
		var result = Types.InAssembly (
					assembly : ApplicationAssembly
				).
			Should().
			NotHaveDependencyOn (
					dependency : PresentationAssembly.GetName().
						Name
				).
			GetResult();

		result.IsSuccessful.Should().
			BeTrue();
	}

	[ Fact ]
	public void InfrastructureLayer_ShouldNotHaveDependencyOn_PresentationLayer()
	{
		var result = Types.InAssembly (
					assembly : InfrastructureAssembly
				).
			Should().
			NotHaveDependencyOn (
					dependency : PresentationAssembly.GetName().
						Name
				).
			GetResult();

		result.IsSuccessful.Should().
			BeTrue();
	}
}
