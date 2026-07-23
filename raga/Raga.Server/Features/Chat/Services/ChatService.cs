using Grpc.Core;
using MediatR;

namespace Raga.Server.Features.Chat.Services;

public class ChatService(
    ILogger<Gacha.Services.GachaService> logger,
    IMediator mediator)
    : Server.ChatService.ChatServiceBase
{
    public override Task<SendMessageResponse> SendMessage(
        SendMessageRequest request,
        ServerCallContext context)
    {
        throw new NotImplementedException();
    }

    public override Task<GetMessagesResponse> GetMessages(
        GetMessagesRequest request,
        ServerCallContext context)
    {
        throw new NotImplementedException();
    }
}