using Grpc.Core;
using MediatR;
using Raga.Server.Features.Gacha.Commands.ClaimDailyReward;
using Raga.Server.Features.Gacha.Commands.PullGacha;
using Raga.Server.Features.Gacha.Commands.TradeItems;

namespace Raga.Server.Features.Gacha.Services;

public class GachaService(
    ILogger<GachaService> logger,
    IMediator mediator)
    : Server.GachaService.GachaServiceBase
{
    public override Task<GachaPullResponse> PullGacha(
        GachaPullRequest request, ServerCallContext context)
    {
        return GrpcRequestBuilder<GachaPullResponse>
            .CreateWithDefaults(request, logger, mediator, context)
            .MapTo(() => new PullGachaCommand { PlayerId = request.PlayerId })
            .ExecuteAsync();
    }

    public override Task<TradeResponse> TradeItems(
        TradeRequest request, ServerCallContext context)
    {
        return GrpcRequestBuilder<TradeResponse>
            .CreateWithDefaults(request, logger, mediator, context)
            .MapTo(() => new TradeItemsCommand
            {
                FromPlayerId = request.FromPlayerId,
                ToPlayerId = request.ToPlayerId,
                OfferedItemId = request.OfferedItemId,
                RequestedItemId = request.RequestedItemId
            })
            .ExecuteAsync();
    }

    public override Task<DailyRewardResponse> ClaimDailyReward(
        DailyRewardRequest request, ServerCallContext context)
    {
        return GrpcRequestBuilder<DailyRewardResponse>
            .CreateWithDefaults(request, logger, mediator, context)
            .MapTo(() => new ClaimDailyRewardCommand { PlayerId = request.PlayerId })
            .ExecuteAsync();
    }
}
