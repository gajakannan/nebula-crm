using FluentValidation;
using Nebula.Application.DTOs;

namespace Nebula.Application.Validators;

public class SubmissionUpdateValidator : AbstractValidator<SubmissionUpdateDto>
{
    public SubmissionUpdateValidator()
    {
        RuleFor(x => x.CurrentStatus)
            .MaximumLength(30)
            .When(x => !string.IsNullOrWhiteSpace(x.CurrentStatus));

        RuleFor(x => x.PremiumEstimate)
            .GreaterThanOrEqualTo(0)
            .When(x => x.PremiumEstimate.HasValue);

        RuleFor(x => x.AssignedToUserId)
            .NotEmpty()
            .When(x => x.AssignedToUserId.HasValue);

        RuleFor(x => x.LineOfBusiness)
            .Must(LineOfBusinessValidation.IsValid)
            .WithMessage(LineOfBusinessValidation.ErrorMessage);
    }
}
