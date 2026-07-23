using MediatR;
using Raga.Server.Common.Interfaces;

namespace Raga.Server.Features.Clans.Queries.GetClanMembers;

public class GetClanMembersHandler(
    IClanRepository clanRepository)
    : IRequestHandler<GetClanMembersQuery, GetClanMembersResponse>
{
    public async Task<GetClanMembersResponse> Handle(
        GetClanMembersQuery request,
        CancellationToken cancellationToken)
    {
        var members = await clanRepository.GetClanMembersAsync(request.ClanId);

        var response = new GetClanMembersResponse();
        
        foreach (var member in members)
        {
            var playerResponse = member.ToPlayerResponse();
            response.Members.Add(playerResponse);
        }
        
        return response;
    }
}