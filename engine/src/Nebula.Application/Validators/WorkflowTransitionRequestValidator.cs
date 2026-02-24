using FluentValidation;
using Nebula.Application.DTOs;

namespace Nebula.Application.Validators;

public class WorkflowTransitionRequestValidator : AbstractValidator<WorkflowTransitionRequestDto>
{
    public WorkflowTransitionRequestValidator()
    {
        RuleFor(x => x.ToState).NotEmpty().MaximumLength(30);
    }
}
