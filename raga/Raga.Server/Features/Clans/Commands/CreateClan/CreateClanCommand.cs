using MediatR;

namespace Raga.Server.Features.Clans.Commands.CreateClan;

public class CreateClanCommand : IRequest<CreateClanResponse>
{
    public required string Name { get; set; }
    public required string Description { get; set; }
}