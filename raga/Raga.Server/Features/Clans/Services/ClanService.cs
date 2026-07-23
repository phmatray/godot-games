using Grpc.Core;
using MediatR;
using Raga.Server.Features.Clans.Commands.CreateClan;
using Raga.Server.Features.Clans.Commands.JoinClan;
using Raga.Server.Features.Clans.Commands.LeaveClan;
using Raga.Server.Features.Clans.Queries.GetClan;
using Raga.Server.Features.Clans.Queries.GetClanMembers;
using Raga.Server.Features.Clans.Queries.GetClans;

namespace Raga.Server.Features.Clans.Services;

public class ClanService(
    ILogger<Gacha.Services.GachaService> logger,
    IMediator mediator)
    : Server.ClanService.ClanServiceBase
{
    public override Task<CreateClanResponse> CreateClan(
        CreateClanRequest request, ServerCallContext context)
    {
        return GrpcRequestBuilder<CreateClanResponse>
            .CreateWithDefaults(request, logger, mediator, context)
            .MapTo(() => new CreateClanCommand { Name = request.Name, Description = request.Description })
            .ExecuteAsync();
    }

    public override Task<GetClanResponse> GetClan(
        GetClanRequest request, ServerCallContext context)
    {
        return GrpcRequestBuilder<GetClanResponse>
            .CreateWithDefaults(request, logger, mediator, context)
            .MapTo(() => new GetClanQuery { ClanId = request.ClanId })
            .ExecuteAsync();
    }

    public override Task<JoinClanResponse> JoinClan(
        JoinClanRequest request, ServerCallContext context)
    {
        return GrpcRequestBuilder<JoinClanResponse>
            .CreateWithDefaults(request, logger, mediator, context)
            .MapTo(() => new JoinClanCommand { PlayerId = request.PlayerId, ClanId = request.ClanId })
            .ExecuteAsync();
    }

    public override Task<LeaveClanResponse> LeaveClan(
        LeaveClanRequest request, ServerCallContext context)
    {
        return GrpcRequestBuilder<LeaveClanResponse>
            .CreateWithDefaults(request, logger, mediator, context)
            .MapTo(() => new LeaveClanCommand { PlayerId = request.PlayerId, ClanId = request.ClanId })
            .ExecuteAsync();
    }

    public override Task<GetClanMembersResponse> GetClanMembers(
        GetClanMembersRequest request, ServerCallContext context)
    {
        return GrpcRequestBuilder<GetClanMembersResponse>
            .CreateWithDefaults(request, logger, mediator, context)
            .MapTo(() => new GetClanMembersQuery { ClanId = request.ClanId })
            .ExecuteAsync();
    }

    public override Task<GetClansResponse> GetClans(
        GetClansRequest request, ServerCallContext context)
    {
        return GrpcRequestBuilder<GetClansResponse>
            .CreateWithDefaults(request, logger, mediator, context)
            .MapTo(() => new GetClansQuery { Location = request.Location })
            .ExecuteAsync();
    }
}