using System.Diagnostics;
using Google.Protobuf;
using Grpc.Core;
using MediatR;
using Serilog.Context;

namespace Raga.Server.Features;

public class GrpcRequestBuilder<TResponse>(IMessage request)
    where TResponse : notnull
{
    private ServerCallContext? _context;
    private IMediator? _mediator;
    private ILogger? _logger;
    private Func<IRequest<TResponse>>? _messageFactory;

    public static GrpcRequestBuilder<TResponse> Create(IMessage request)
    {
        return new GrpcRequestBuilder<TResponse>(request);
    }

    public static GrpcRequestBuilder<TResponse> CreateWithDefaults(
        IMessage request,
        ILogger logger,
        IMediator mediator,
        ServerCallContext context)
    {
        return Create(request).UseDependencies(logger, mediator, context);
    }
    
    public GrpcRequestBuilder<TResponse> UseDependencies(
        ILogger logger, IMediator mediator, ServerCallContext context)
    {
        return UseLogger(logger).UseMediator(mediator).UseGrpcContext(context);
    }

    public GrpcRequestBuilder<TResponse> UseLogger(ILogger logger)
    {
        _logger = logger;
        return this;
    }

    public GrpcRequestBuilder<TResponse> UseMediator(IMediator mediator)
    {
        _mediator = mediator;
        return this;
    }

    public GrpcRequestBuilder<TResponse> UseGrpcContext(ServerCallContext context)
    {
        _context = context;
        return this;
    }

    public GrpcRequestBuilder<TResponse> MapTo(Func<IRequest<TResponse>> messageFactory)
    {
        _messageFactory = messageFactory;
        return this;
    }

    public async Task<TResponse> ExecuteAsync()
    {
        ArgumentNullException.ThrowIfNull(_context);
        ArgumentNullException.ThrowIfNull(_mediator);
        ArgumentNullException.ThrowIfNull(_logger);
        ArgumentNullException.ThrowIfNull(_messageFactory);

        var stopwatch = Stopwatch.StartNew();
        var peer = _context.Peer;
        var method = _context.Method;

        using (LogContext.PushProperty("Method", method))
        using (LogContext.PushProperty("Peer", peer))
        {
            _logger.MethodCalled(method, request);

            try
            {
                var message = _messageFactory();
                var response = await _mediator.Send(message);
                stopwatch.Stop();
                _logger.MethodSucceeded(method, stopwatch.ElapsedMilliseconds, response);
                return response;
            }
            catch (RpcException rpcEx)
            {
                stopwatch.Stop();
                _logger.HandledRpcException(rpcEx, stopwatch.ElapsedMilliseconds, method);
                throw;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.UnhandledException(ex, stopwatch.ElapsedMilliseconds, method);
                throw new RpcException(new Status(StatusCode.Internal, "Internal server error"));
            }
        }
    }
}