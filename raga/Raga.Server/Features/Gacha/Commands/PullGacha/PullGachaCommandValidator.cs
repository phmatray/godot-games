using FluentValidation;

namespace Raga.Server.Features.Gacha.Commands.PullGacha;

public class PullGachaCommandValidator : AbstractValidator<PullGachaCommand>
{
    public PullGachaCommandValidator()
    {
        RuleFor(x => x.PlayerId)
            .NotEmpty().WithMessage("PlayerId is required.");
    }
}