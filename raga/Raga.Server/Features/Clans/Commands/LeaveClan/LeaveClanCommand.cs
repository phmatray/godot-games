using MediatR;

namespace Raga.Server.Features.Clans.Commands.LeaveClan;

public class LeaveClanCommand : IRequest<LeaveClanResponse>
{
    public required string PlayerId { get; set; }
    // TODO: No need of clan id (the clanId is already stored in the player object)
    public required string ClanId { get; set; }
}