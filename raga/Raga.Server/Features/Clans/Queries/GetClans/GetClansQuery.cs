using MediatR;

namespace Raga.Server.Features.Clans.Queries.GetClans;

public class GetClansQuery : IRequest<GetClansResponse>
{
    public string? Location { get; set; } // Optional location filter
}