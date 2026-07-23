using FluentValidation;

namespace Raga.Server.Features.Clans.Commands.JoinClan;

public class JoinClanCommandValidator : AbstractValidator<JoinClanCommand>
{
    public JoinClanCommandValidator()
    {
        RuleFor(x => x.PlayerId)
            .NotEmpty().WithMessage("PlayerId is required.");
        
        RuleFor(x => x.ClanId)
            .NotEmpty().WithMessage("ClanId is required.");
    }
}