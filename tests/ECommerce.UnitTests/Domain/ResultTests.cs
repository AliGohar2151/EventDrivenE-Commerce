using ECommerce.Domain.Primitives;
using FluentAssertions;
using Xunit;

namespace ECommerce.UnitTests.Domain;

public class ResultTests
{
    [Fact]
    public void Success_ShouldCreateSuccessfulResult()
    {
        var result = Result.Success();

        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().Be(Error.None);
    }

    [Fact]
    public void Failure_ShouldCreateFailureResult()
    {
        var error = Error.NotFound("User.NotFound", "The user was not found.");

        var result = Result.Failure(error);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void GenericSuccess_ShouldContainValue()
    {
        var value = "Test Value";

        var result = Result.Success(value);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(value);
    }

    [Fact]
    public void AccessingValueOnFailure_ShouldThrowInvalidOperationException()
    {
        var error = Error.Validation("Input.Invalid", "Invalid input.");
        var result = Result.Failure<string>(error);

        Action action = () => _ = result.Value;

        action.Should().Throw<InvalidOperationException>();
    }
}
