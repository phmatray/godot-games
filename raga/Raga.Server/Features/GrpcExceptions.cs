using Grpc.Core;

namespace Raga.Server.Features;

public class PlayerNotFoundException(string playerId)
    : RpcException(
        new Status(StatusCode.NotFound,
            $"Player with ID '{playerId}' was not found."));

public class InsufficientCurrencyException(string playerId)
    : RpcException(
        new Status(StatusCode.FailedPrecondition,
            $"Player with ID '{playerId}' does not have enough currency to perform the gacha pull."));

public class DailyRewardAlreadyClaimedException(DateTime nextRewardTime)
    : RpcException(
        new Status(StatusCode.AlreadyExists,
            $"Daily reward already claimed. Next reward available at {nextRewardTime}."));
