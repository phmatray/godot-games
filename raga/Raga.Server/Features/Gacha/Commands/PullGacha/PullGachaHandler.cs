using Grpc.Core;
using MediatR;
using Raga.Server.Common.Interfaces;
using Raga.Server.Data.Models;

namespace Raga.Server.Features.Gacha.Commands.PullGacha;

public class PullGachaHandler(
    IPlayerRepository playerRepository,
    IGachaItemRepository gachaItemRepository,
    IFakeDataGenerator<GachaItem> gachaItemFaker)
    : IRequestHandler<PullGachaCommand, GachaPullResponse>
{
    private const int PullCost = 10;

    public async Task<GachaPullResponse> Handle(
        PullGachaCommand request,
        CancellationToken cancellationToken)
    {
        var playerId = request.PlayerId;
        var player = await playerRepository.GetPlayerAsync(playerId);
            
        // If Player doesn't exist, create it
        if (player == null)
        {
            player = new Player { PlayerId = playerId };
            await playerRepository.AddPlayerAsync(player);
        }
        
        // Check if the player has enough currency
        if (player.TotalCurrency < PullCost)
        {
            throw new InsufficientCurrencyException(playerId);
        }
        
        // Perform the gacha pull
        var item = gachaItemFaker.Generate();
        item.PlayerId = playerId;
        await gachaItemRepository.AddGachaItemAsync(item);
        
        // Deduct the cost and update player's stats
        player.TotalPulls++;
        player.TotalCurrency -= PullCost;

        // Update the player in the database
        await playerRepository.UpdatePlayerAsync(player);

        return new GachaPullResponse
        {
            Item = item.ToGachaItemResponse(),
            RemainingCurrency = player.TotalCurrency
        };
    }
}
