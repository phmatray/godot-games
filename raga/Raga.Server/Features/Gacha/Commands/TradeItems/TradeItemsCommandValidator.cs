using FluentValidation;

namespace Raga.Server.Features.Gacha.Commands.TradeItems;

public class TradeItemsCommandValidator : AbstractValidator<TradeItemsCommand>
{
    public TradeItemsCommandValidator()
    {
        RuleFor(x => x.FromPlayerId)
            .NotEmpty().WithMessage("FromPlayerId is required.");
        
        RuleFor(x => x.ToPlayerId)
            .NotEmpty().WithMessage("ToPlayerId is required.");
        
        RuleFor(x => x.OfferedItemId)
            .NotEmpty().WithMessage("OfferedItemId is required.");
        
        RuleFor(x => x.RequestedItemId)
            .NotEmpty().WithMessage("RequestedItemId is required.");
    }
}