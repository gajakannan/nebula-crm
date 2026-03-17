using FluentValidation;
using Nebula.Application.DTOs;

namespace Nebula.Application.Validators;

public class RenewalUpdateValidator : AbstractValidator<RenewalUpdateDto>
{
    public RenewalUpdateValidator()
    {
        RuleFor(x => x.CurrentStatus)
            .MaximumLength(30)
            .When(x => !string.IsNullOrWhiteSpace(x.CurrentStatus));

        RuleFor(x => x.AssignedToUserId)
            .NotEmpty()
            .When(x => x.AssignedToUserId.HasValue);

        RuleFor(x => x.LineOfBusiness)
            .Must(LineOfBusinessValidation.IsValid)
            .WithMessage(LineOfBusinessValidation.ErrorMessage);
    }
}
