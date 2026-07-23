using MediatR;

namespace Raga.Server.Features.Clans.Queries.GetClan;

public class GetClanQuery : IRequest<GetClanResponse>
{
    public required string ClanId { get; set; }
}