using Grpc.Core;
using MediatR;
using Raga.Server.Features.Info.Queries.GetInfo;

namespace Raga.Server.Features.Info.Services;

public class InfoService(
    ILogger<InfoService> logger,
    IMediator mediator)
    : Server.InfoService.InfoServiceBase
{
    public override async Task<GetInfoResponse> GetInfo(
        GetInfoRequest request,
        ServerCallContext context)
    {
        return await GrpcRequestBuilder<GetInfoResponse>
            .CreateWithDefaults(request, logger, mediator, context)
            .MapTo(() => new GetInfoQuery())
            .ExecuteAsync();
    }
}