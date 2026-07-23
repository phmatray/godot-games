using Grpc.Core;
using MediatR;
using Raga.Server.Features.Players.Queries.GetInventoryHandler;
using Raga.Server.Features.Players.Queries.GetPlayer;

namespace Raga.Server.Features.Players.Services;

public class PlayerService(
    ILogger<PlayerService> logger,
    IMediator mediator)
    : Server.PlayerService.PlayerServiceBase
{
    public override Task<GetPlayerResponse> GetPlayer(
        GetPlayerRequest request, ServerCallContext context)
    {
        return GrpcRequestBuilder<GetPlayerResponse>
            .CreateWithDefaults(request, logger, mediator, context)
            .MapTo(() => new GetPlayerQuery { PlayerId = request.PlayerId })
            .ExecuteAsync();
    }

    public override Task<UpdatePlayerResponse> UpdatePlayer(
        UpdatePlayerRequest request, ServerCallContext context)
    {
        throw new NotImplementedException();
    }

    public override Task<DeletePlayerResponse> DeletePlayer(
        DeletePlayerRequest request, ServerCallContext context)
    {
        throw new NotImplementedException();
    }

    public override Task<ListPlayersResponse> ListPlayers(
        ListPlayersRequest request, ServerCallContext context)
    {
        throw new NotImplementedException();
    }

    public override Task<InventoryResponse> GetInventory(
        InventoryRequest request, ServerCallContext context)
    {
        return GrpcRequestBuilder<InventoryResponse>
            .CreateWithDefaults(request, logger, mediator, context)
            .MapTo(() => new GetInventoryQuery { PlayerId = request.PlayerId })
            .ExecuteAsync();
    }
}