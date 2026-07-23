using FluentValidation;

namespace Raga.Server.Features.Clans.Commands.LeaveClan;

public class LeaveClanCommandValidator : AbstractValidator<LeaveClanCommand>
{
    public LeaveClanCommandValidator()
    {
        RuleFor(x => x.PlayerId)
            .NotEmpty().WithMessage("PlayerId is required.");
        
        RuleFor(x => x.ClanId)
            .NotEmpty().WithMessage("ClanId is required.");
    }
}