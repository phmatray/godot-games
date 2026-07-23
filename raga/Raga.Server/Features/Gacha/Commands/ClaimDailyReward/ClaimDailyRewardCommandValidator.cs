using FluentValidation;

namespace Raga.Server.Features.Gacha.Commands.ClaimDailyReward;

public class ClaimDailyRewardCommandValidator : AbstractValidator<ClaimDailyRewardCommand>
{
    public ClaimDailyRewardCommandValidator()
    {
        RuleFor(x => x.PlayerId)
            .NotEmpty().WithMessage("PlayerId is required.");
    }
}