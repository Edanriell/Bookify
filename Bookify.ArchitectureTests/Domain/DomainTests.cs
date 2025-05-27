using System.Reflection;
using Bookify.ArchitectureTests.Infrastructure;
using Bookify.Domain.Abstractions;
using FluentAssertions;
using NetArchTest.Rules;

namespace Bookify.ArchitectureTests.Domain;

public class DomainTests : BaseTest
{
	[ Fact ]
	public void DomainEvents_Should_BeSealed()
	{
		var result = Types.InAssembly (
					assembly : DomainAssembly
				).
			That().
			ImplementInterface (
					interfaceType : typeof(IDomainEvent)
				).
			Should().
			BeSealed().
			GetResult();

		result.IsSuccessful.Should().
			BeTrue();
	}

	[ Fact ]
	public void DomainEvent_ShouldHave_DomainEventPostfix()
	{
		var result = Types.InAssembly (
					assembly : DomainAssembly
				).
			That().
			ImplementInterface (
					interfaceType : typeof(IDomainEvent)
				).
			Should().
			HaveNameEndingWith (
					end : "DomainEvent"
				).
			GetResult();

		result.IsSuccessful.Should().
			BeTrue();
	}

	[ Fact ]
	public void Entities_ShouldHave_PrivateParameterlessConstructor()
	{
		IEnumerable<Type> entityTypes = Types.InAssembly (
					assembly : DomainAssembly
				).
			That().
			Inherit (
					type : typeof(Entity)
				).
			GetTypes();

		var failingTypes = new List<Type>();
		foreach ( var entityType in entityTypes )
		{
			var constructors = entityType.GetConstructors (
					bindingAttr : BindingFlags.NonPublic | BindingFlags.Instance
				);

			if ( !constructors.Any (
						 predicate : c => c.IsPrivate
									   && c.GetParameters().
											  Length
									   == 0
					 ) )
			{
				failingTypes.Add (
						item : entityType
					);
			}
		}

		failingTypes.Should().
			BeEmpty();
	}
}
