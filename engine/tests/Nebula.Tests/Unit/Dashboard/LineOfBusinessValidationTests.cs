using FluentAssertions;
using Nebula.Application.DTOs;
using Nebula.Application.Validators;

namespace Nebula.Tests.Unit.Dashboard;

public class LineOfBusinessValidationTests
{
    [Fact]
    public void SubmissionCreateValidator_AcceptsKnownLineOfBusiness()
    {
        var validator = new SubmissionCreateValidator();
        var model = ValidModel() with { LineOfBusiness = "Property" };

        var result = validator.Validate(model);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void SubmissionCreateValidator_AcceptsNullLineOfBusiness()
    {
        var validator = new SubmissionCreateValidator();
        var model = ValidModel() with { LineOfBusiness = null };

        var result = validator.Validate(model);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void SubmissionCreateValidator_RejectsInvalidLineOfBusiness()
    {
        var validator = new SubmissionCreateValidator();
        var model = ValidModel() with { LineOfBusiness = "Aviation" };

        var result = validator.Validate(model);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.PropertyName == "LineOfBusiness");
    }

    private static SubmissionCreateDto ValidModel() => new(
        Guid.NewGuid(),
        Guid.NewGuid(),
        Guid.NewGuid(),
        "Property",
        "Received",
        DateTime.UtcNow,
        150000m,
        Guid.NewGuid());
}
