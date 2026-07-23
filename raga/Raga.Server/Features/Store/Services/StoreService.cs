using Grpc.Core;
using MediatR;

namespace Raga.Server.Features.Store.Services;

public class StoreService(
    ILogger<Gacha.Services.GachaService> logger,
    IMediator mediator)
    : Server.StoreService.StoreServiceBase
{
    public override Task<PurchaseItemResponse> PurchaseItem(
        PurchaseItemRequest request,
        ServerCallContext context)
    {
        throw new NotImplementedException();
    }

    public override Task<GetInventoryResponse> GetInventory(
        GetInventoryRequest request,
        ServerCallContext context)
    {
        throw new NotImplementedException();
    }
}