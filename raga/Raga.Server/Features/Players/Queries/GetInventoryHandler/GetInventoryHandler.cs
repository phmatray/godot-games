using MediatR;
using Raga.Server.Common.Interfaces;

namespace Raga.Server.Features.Players.Queries.GetInventoryHandler;

public class GetInventoryHandler(
    IPlayerRepository playerRepository)
    : IRequestHandler<GetInventoryQuery, InventoryResponse>
{
    public async Task<InventoryResponse> Handle(
        GetInventoryQuery request,
        CancellationToken cancellationToken)
    {
        // Check if the player exists
        var player = await playerRepository.GetPlayerAsync(request.PlayerId);
        if (player == null)
        {
            throw new PlayerNotFoundException(request.PlayerId);
        }

        // Retrieve the player's inventory
        var inventory = await playerRepository.GetPlayerInventoryAsync(request.PlayerId);
        
        // Create and populate the response
        var response = new InventoryResponse();
        response.Items.AddRange(inventory.Select(i => i.ToGachaItemResponse()));

        return response;
    }
}