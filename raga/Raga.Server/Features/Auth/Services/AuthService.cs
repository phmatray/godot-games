using Grpc.Core;
using MediatR;

namespace Raga.Server.Features.Auth.Services;

public class AuthService(
    ILogger<Gacha.Services.GachaService> logger,
    IMediator mediator)
    : Server.AuthService.AuthServiceBase
{
    public override Task<LoginResponse> Login(
        LoginRequest request,
        ServerCallContext context)
    {
        throw new NotImplementedException();
    }

    public override Task<LogoutResponse> Logout(
        LogoutRequest request,
        ServerCallContext context)
    {
        throw new NotImplementedException();
    }

    public override Task<RegisterResponse> Register(
        RegisterRequest request,
        ServerCallContext context)
    {
        throw new NotImplementedException();
    }
}