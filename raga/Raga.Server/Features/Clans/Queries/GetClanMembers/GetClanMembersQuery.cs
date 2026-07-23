using MediatR;

namespace Raga.Server.Features.Clans.Queries.GetClanMembers;

public class GetClanMembersQuery : IRequest<GetClanMembersResponse>
{
    public required string ClanId { get; set; }
}