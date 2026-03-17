using FluentValidation;
using Nebula.Application.DTOs;

namespace Nebula.Application.Validators;

public class SubmissionCreateValidator : AbstractValidator<SubmissionCreateDto>
{
    public SubmissionCreateValidator()
    {
        RuleFor(x => x.AccountId).NotEmpty();
        RuleFor(x => x.BrokerId).NotEmpty();
        RuleFor(x => x.CurrentStatus).NotEmpty().MaximumLength(30);
        RuleFor(x => x.PremiumEstimate).GreaterThanOrEqualTo(0);
        RuleFor(x => x.AssignedToUserId).NotEmpty();
        RuleFor(x => x.LineOfBusiness)
            .Must(LineOfBusinessValidation.IsValid)
            .WithMessage(LineOfBusinessValidation.ErrorMessage);
    }
}
