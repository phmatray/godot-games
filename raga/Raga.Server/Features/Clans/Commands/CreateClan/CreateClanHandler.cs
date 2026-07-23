using MediatR;
using Raga.Server.Common.Interfaces;
using Raga.Server.Data.Models;

namespace Raga.Server.Features.Clans.Commands.CreateClan;

public class CreateClanHandler(
    IClanRepository clanRepository)
    : IRequestHandler<CreateClanCommand, CreateClanResponse>
{
    public async Task<CreateClanResponse> Handle(
        CreateClanCommand request,
        CancellationToken cancellationToken)
    {
        if (await clanRepository.ClanNameExistsAsync(request.Name))
        {
            return new CreateClanResponse
            {
                Success = false,
                Message = "Clan name already exists"
            };
        }

        var clan = new Clan
        {
            Id = Guid.NewGuid().ToString(),
            Name = request.Name,
            Description = request.Description
        };
        
        clan = await clanRepository.CreateClanAsync(clan);

        return new CreateClanResponse
        {
            Success = true,
            Message = "Clan created successfully",
            Clan = clan.ToClanResponse()
        };
    }
}