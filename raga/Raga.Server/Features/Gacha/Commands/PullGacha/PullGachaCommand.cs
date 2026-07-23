using MediatR;

namespace Raga.Server.Features.Gacha.Commands.PullGacha;

public class PullGachaCommand : IRequest<GachaPullResponse>
{
    public required string PlayerId { get; set; }
}