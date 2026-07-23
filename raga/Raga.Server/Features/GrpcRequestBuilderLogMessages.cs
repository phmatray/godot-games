using Grpc.Core;

namespace Raga.Server.Features;

internal static partial class GrpcRequestBuilderLogMessages
{
    [LoggerMessage(
        EventId = 1000,
        Level = LogLevel.Information,
        Message = "{Method} called with request {@Request}")]
    public static partial void MethodCalled(
        this ILogger logger, string method, object request);

    [LoggerMessage(
        EventId = 1001,
        Level = LogLevel.Information,
        Message = "{Method} succeeded in {ElapsedMilliseconds}ms {@Response}")]
    public static partial void MethodSucceeded(
        this ILogger logger, string method, long elapsedMilliseconds, object response);

    [LoggerMessage(
        EventId = 1002,
        Level = LogLevel.Warning,
        Message = "Handled RpcException in {ElapsedMilliseconds}ms for {Method}")]
    public static partial void HandledRpcException(
        this ILogger logger, RpcException rpcEx, long elapsedMilliseconds, string method);

    [LoggerMessage(
        EventId = 1003,
        Level = LogLevel.Error,
        Message = "Unhandled exception occurred in {ElapsedMilliseconds}ms for {Method}")]
    public static partial void UnhandledException(
        this ILogger logger, Exception exception, long elapsedMilliseconds, string method);
}
