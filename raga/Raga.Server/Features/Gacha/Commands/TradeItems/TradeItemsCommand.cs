using MediatR;

namespace Raga.Server.Features.Gacha.Commands.TradeItems;

public class TradeItemsCommand : IRequest<TradeResponse>
{
    public required string FromPlayerId { get; set; }
    public required string ToPlayerId { get; set; }
    public required string OfferedItemId { get; set; }
    public required string RequestedItemId { get; set; }
}