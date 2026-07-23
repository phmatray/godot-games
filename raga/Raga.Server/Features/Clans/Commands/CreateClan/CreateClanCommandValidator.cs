using FluentValidation;

namespace Raga.Server.Features.Clans.Commands.CreateClan;

public class CreateClanCommandValidator : AbstractValidator<CreateClanCommand>
{
    public CreateClanCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Clan name is required.");
        
        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot be more than 500 characters.");
    }
}