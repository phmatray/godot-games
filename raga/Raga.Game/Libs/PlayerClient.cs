using System.Threading.Tasks;
using Grpc.Net.Client;

namespace Raga.Game.Libs;

public class PlayerClient
{
    private readonly PlayerService.PlayerServiceClient _client;

    public PlayerClient(string serverAddress)
    {
        var channel = GrpcChannel.ForAddress(serverAddress);
        _client = new PlayerService.PlayerServiceClient(channel);
    }

    public async Task<GetPlayerResponse> GetPlayerAsync(string playerId)
    {
        var request = new GetPlayerRequest { PlayerId = playerId };
        return await _client.GetPlayerAsync(request);
    }

    public async Task<InventoryResponse> GetInventoryAsync(string playerId)
    {
        var request = new InventoryRequest { PlayerId = playerId };
        return await _client.GetInventoryAsync(request);
    }
}