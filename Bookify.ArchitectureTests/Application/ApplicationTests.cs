using Bookify.Application.Abstractions.Messaging;
using Bookify.ArchitectureTests.Infrastructure;
using FluentAssertions;
using FluentValidation;
using NetArchTest.Rules;

namespace Bookify.ArchitectureTests.Application;

public class ApplicationTests : BaseTest
{
	[ Fact ]
	public void CommandHandler_ShouldHave_NameEndingWith_CommandHandler()
	{
		var result = Types.InAssembly (
					assembly : ApplicationAssembly
				).
			That().
			ImplementInterface (
					interfaceType : typeof(ICommandHandler<>)
				).
			Or().
			ImplementInterface (
					interfaceType : typeof(ICommandHandler<,>)
				).
			Should().
			HaveNameEndingWith (
					end : "CommandHandler"
				).
			GetResult();

		result.IsSuccessful.Should().
			BeTrue();
	}

	[ Fact ]
	public void CommandHandler_Should_NotBePublic()
	{
		var result = Types.InAssembly (
					assembly : ApplicationAssembly
				).
			That().
			ImplementInterface (
					interfaceType : typeof(ICommandHandler<>)
				).
			Or().
			ImplementInterface (
					interfaceType : typeof(ICommandHandler<,>)
				).
			Should().
			NotBePublic().
			GetResult();

		result.IsSuccessful.Should().
			BeTrue();
	}

	[ Fact ]
	public void QueryHandler_ShouldHave_NameEndingWith_QueryHandler()
	{
		var result = Types.InAssembly (
					assembly : ApplicationAssembly
				).
			That().
			ImplementInterface (
					interfaceType : typeof(IQueryHandler<,>)
				).
			Should().
			HaveNameEndingWith (
					end : "QueryHandler"
				).
			GetResult();

		result.IsSuccessful.Should().
			BeTrue();
	}

	[ Fact ]
	public void QueryHandler_Should_NotBePublic()
	{
		var result = Types.InAssembly (
					assembly : ApplicationAssembly
				).
			That().
			ImplementInterface (
					interfaceType : typeof(IQueryHandler<,>)
				).
			Should().
			NotBePublic().
			GetResult();

		result.IsSuccessful.Should().
			BeTrue();
	}

	[ Fact ]
	public void Validator_ShouldHave_NameEndingWith_Validator()
	{
		var result = Types.InAssembly (
					assembly : ApplicationAssembly
				).
			That().
			Inherit (
					type : typeof(AbstractValidator<>)
				).
			Should().
			HaveNameEndingWith (
					end : "Validator"
				).
			GetResult();

		result.IsSuccessful.Should().
			BeTrue();
	}

	[ Fact ]
	public void Validator_Should_NotBePublic()
	{
		var result = Types.InAssembly (
					assembly : ApplicationAssembly
				).
			That().
			Inherit (
					type : typeof(AbstractValidator<>)
				).
			Should().
			NotBePublic().
			GetResult();

		result.IsSuccessful.Should().
			BeTrue();
	}
}
