using FluentAssertions;
using NetArchTest.Rules;

namespace Bookify.ArchitectureTests.Infrastructure;

internal static class TestResultExtensions
{
	internal static void ShouldBeSuccessful(this TestResult testResult)
	{
		testResult.FailingTypes?.Should().BeEmpty();
	}
}
