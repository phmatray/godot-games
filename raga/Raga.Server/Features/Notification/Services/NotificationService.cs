using Grpc.Core;
using MediatR;

namespace Raga.Server.Features.Notification.Services;

public class NotificationService(
    ILogger<Gacha.Services.GachaService> logger,
    IMediator mediator)
    : Server.NotificationService.NotificationServiceBase
{
    public override Task<SendNotificationResponse> SendNotification(
        SendNotificationRequest request,
        ServerCallContext context)
    {
        throw new NotImplementedException();
    }

    public override Task<GetNotificationsResponse> GetNotifications(
        GetNotificationsRequest request,
        ServerCallContext context)
    {
        throw new NotImplementedException();
    }
}