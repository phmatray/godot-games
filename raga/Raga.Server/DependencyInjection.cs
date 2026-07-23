namespace Raga.Server;

public static class DependencyInjection
{
    public static void MapGrpcAuthService(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGrpcService<Features.Auth.Services.AuthService>();
    }
    
    public static void MapGrpcChatService(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGrpcService<Features.Chat.Services.ChatService>();
    }
    
    public static void MapGrpcClanService(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGrpcService<Features.Clans.Services.ClanService>();
    }
    
    public static void MapGrpcGachaService(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGrpcService<Features.Gacha.Services.GachaService>();
    }
    
    public static void MapGrpcInfoService(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGrpcService<Features.Info.Services.InfoService>();
    }
    
    public static void MapGrpcLeaderboardService(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGrpcService<Features.Leaderboard.Services.LeaderboardService>();
    }
    
    public static void MapGrpcMatchmakingService(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGrpcService<Features.Matchmaking.Services.MatchmakingService>();
    }
    
    public static void MapGrpcNotificationService(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGrpcService<Features.Notification.Services.NotificationService>();
    }
    
    public static void MapGrpcPlayerService(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGrpcService<Features.Players.Services.PlayerService>();
    }
    
    public static void MapGrpcStoreService(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGrpcService<Features.Store.Services.StoreService>();
    }
}