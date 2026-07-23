using MediatR;
using Raga.Server.Common.Interfaces;

namespace Raga.Server.Features.Players.Queries.GetPlayer;

public class GetPlayerHandler(
    IPlayerRepository playerRepository)
    : IRequestHandler<GetPlayerQuery, GetPlayerResponse>
{
    public async Task<GetPlayerResponse> Handle(
        GetPlayerQuery request,
        CancellationToken cancellationToken)
    {
        var player = await playerRepository.GetPlayerAsync(request.PlayerId);
        
        if (player == null)
        {
            return new GetPlayerResponse { Player = null };
        }
        
        return new GetPlayerResponse
        {
            Player = player.ToPlayerResponse()
        };
    }
}