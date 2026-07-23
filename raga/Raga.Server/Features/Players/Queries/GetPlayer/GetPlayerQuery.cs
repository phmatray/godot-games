using MediatR;

namespace Raga.Server.Features.Players.Queries.GetPlayer;

public class GetPlayerQuery : IRequest<GetPlayerResponse>
{
    public required string PlayerId { get; set; }
}