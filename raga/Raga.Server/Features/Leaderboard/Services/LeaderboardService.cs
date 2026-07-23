using Grpc.Core;
using MediatR;

namespace Raga.Server.Features.Leaderboard.Services;

public class LeaderboardService(
    ILogger<Gacha.Services.GachaService> logger,
    IMediator mediator)
    : Server.LeaderboardService.LeaderboardServiceBase
{
    public override Task<GetLeaderboardResponse> GetGlobalLeaderboard(
        GetLeaderboardRequest request,
        ServerCallContext context)
    {
        throw new NotImplementedException();
    }

    public override Task<GetLeaderboardResponse> GetClanLeaderboard(
        GetClanLeaderboardRequest request,
        ServerCallContext context)
    {
        throw new NotImplementedException();
    }
}