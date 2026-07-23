using MediatR;

namespace Raga.Server.Features.Players.Queries.GetInventoryHandler;

public class GetInventoryQuery : IRequest<InventoryResponse>
{
    public required string PlayerId { get; set; }
}