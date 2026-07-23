using System.Threading.Tasks;
using Grpc.Net.Client;

namespace Raga.Game.Libs;

public class GachaClient
{
    private readonly GachaService.GachaServiceClient _client;
    private readonly string _playerId;

    public GachaClient(string serverAddress, string playerId)
    {
        var channel = GrpcChannel.ForAddress(serverAddress);
        _client = new GachaService.GachaServiceClient(channel);
        _playerId = playerId;
    }

    public async Task<GachaPullResponse> PullGachaAsync()
    {
        var request = new GachaPullRequest { PlayerId = _playerId };
        return await _client.PullGachaAsync(request);
    }

    public async Task<TradeResponse> TradeItemsAsync(string toPlayerId, string offeredItemId, string requestedItemId)
    {
        var request = new TradeRequest
        {
            FromPlayerId = _playerId,
            ToPlayerId = toPlayerId,
            OfferedItemId = offeredItemId,
            RequestedItemId = requestedItemId
        };
        
        return await _client.TradeItemsAsync(request);
    }
}