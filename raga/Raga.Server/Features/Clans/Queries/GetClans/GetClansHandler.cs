using MediatR;
using Raga.Server.Common.Interfaces;

namespace Raga.Server.Features.Clans.Queries.GetClans;

public class GetClansHandler(
    IClanRepository clanRepository)
    : IRequestHandler<GetClansQuery, GetClansResponse>
{
    public async Task<GetClansResponse> Handle(
        GetClansQuery request,
        CancellationToken cancellationToken)
    {
        var clans = await clanRepository.GetClansByScoreAsync(request.Location);
        var response = new GetClansResponse();

        foreach (var clan in clans)
        {
            var clanResponse = clan.ToClanResponse();
            response.Clans.Add(clanResponse);
        }

        return response;
    }
}