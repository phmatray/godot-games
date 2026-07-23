using MediatR;
using Raga.Server.Common.Interfaces;

namespace Raga.Server.Features.Clans.Commands.JoinClan;

public class JoinClanHandler(
    IClanRepository clanRepository)
    : IRequestHandler<JoinClanCommand, JoinClanResponse>
{
    public async Task<JoinClanResponse> Handle(
        JoinClanCommand request,
        CancellationToken cancellationToken)
    {
        var memberCount = await clanRepository.GetClanMemberCountAsync(request.ClanId);

        if (memberCount >= 30)
        {
            return new JoinClanResponse
            {
                Success = false,
                Message = "Clan is full"
            };
        }

        var success = await clanRepository.JoinClanAsync(request.PlayerId, request.ClanId);
        var message = success
            ? "Joined clan successfully"
            : "Failed to join clan";

        return new JoinClanResponse
        {
            Success = success,
            Message = message
        };
    }
}