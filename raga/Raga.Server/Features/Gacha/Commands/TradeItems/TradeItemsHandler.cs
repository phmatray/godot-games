using MediatR;
using Raga.Server.Common.Interfaces;

namespace Raga.Server.Features.Gacha.Commands.TradeItems;

public class TradeItemsHandler(
    IGachaItemRepository gachaItemRepository,
    IPlayerRepository playerRepository) :
    IRequestHandler<TradeItemsCommand, TradeResponse>
{
    public async Task<TradeResponse> Handle(TradeItemsCommand request, CancellationToken cancellationToken)
    {
        var fromInventory = await playerRepository.GetPlayerInventoryAsync(request.FromPlayerId);
        var toInventory = await playerRepository.GetPlayerInventoryAsync(request.ToPlayerId);

        if (fromInventory.Any(i => i.Id == request.OfferedItemId) && toInventory.Any(i => i.Id == request.RequestedItemId))
        {
            var offeredItem = fromInventory.First(i => i.Id == request.OfferedItemId);
            var requestedItem = toInventory.First(i => i.Id == request.RequestedItemId);

            fromInventory.Remove(offeredItem);
            toInventory.Remove(requestedItem);

            offeredItem.PlayerId = request.ToPlayerId;
            requestedItem.PlayerId = request.FromPlayerId;

            await gachaItemRepository.AddGachaItemAsync(offeredItem);
            await gachaItemRepository.AddGachaItemAsync(requestedItem);

            return new TradeResponse
            {
                Success = true,
                Message = "Trade successful"
            };
        }

        return new TradeResponse
        {
            Success = false,
            Message = "Trade failed"
        };
    }
}