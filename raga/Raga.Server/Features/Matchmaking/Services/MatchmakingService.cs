using Grpc.Core;
using MediatR;

namespace Raga.Server.Features.Matchmaking.Services;

public class MatchmakingService(
    ILogger<Gacha.Services.GachaService> logger,
    IMediator mediator)
    : Server.MatchmakingService.MatchmakingServiceBase
{
    public override Task<FindMatchResponse> FindMatch(
        FindMatchRequest request,
        ServerCallContext context)
    {
        throw new NotImplementedException();
    }

    public override Task<CancelMatchResponse> CancelMatch(
        CancelMatchRequest request,
        ServerCallContext context)
    {
        throw new NotImplementedException();
    }
}