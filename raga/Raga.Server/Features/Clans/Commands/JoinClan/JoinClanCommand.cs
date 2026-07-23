using MediatR;

namespace Raga.Server.Features.Clans.Commands.JoinClan;

public class JoinClanCommand : IRequest<JoinClanResponse>
{
    public required string PlayerId { get; set; }
    public required string ClanId { get; set; }
}