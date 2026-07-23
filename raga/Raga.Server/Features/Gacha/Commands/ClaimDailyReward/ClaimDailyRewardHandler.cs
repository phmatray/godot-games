using MediatR;
using Raga.Server.Common.Interfaces;
using Raga.Server.Data.Models;

namespace Raga.Server.Features.Gacha.Commands.ClaimDailyReward;

public class ClaimDailyRewardHandler(
    IPlayerRepository playerRepository,
    IFakeDataGenerator<GachaItem> gachaItemFaker)
    : IRequestHandler<ClaimDailyRewardCommand, DailyRewardResponse>
{
    private const int DailyRewardAmount = 25;
    
    public async Task<DailyRewardResponse> Handle(
        ClaimDailyRewardCommand request,
        CancellationToken cancellationToken)
    {
        var playerId = request.PlayerId;
        var player = await playerRepository.GetPlayerAsync(playerId);

        // Check if the player exists
        if (player == null)
        {
            throw new PlayerNotFoundException(playerId);
        }
        
        // Check if player has already claimed the daily reward
        if (DateTime.UtcNow.Date == player.LastDailyRewardClaim.Date)
        {
            var nextRewardTime = player.LastDailyRewardClaim.AddDays(1);
            throw new DailyRewardAlreadyClaimedException(nextRewardTime);
        }

        // Update the player's reward claim
        player.LastDailyRewardClaim = DateTime.UtcNow;
        player.TotalCurrency += DailyRewardAmount;
        
        // Generate a reward item and associate it with the player
        var rewardItem = gachaItemFaker.Generate();
        rewardItem.PlayerId = playerId;

        // Update the player in the database
        await playerRepository.UpdatePlayerAsync(player);
        
        // Return a successful response with the reward details
        return new DailyRewardResponse
        {
            Success = true,
            Message = "Daily reward claimed",
            RewardCurrency = DailyRewardAmount,
            RewardItem = rewardItem.ToGachaItemResponse()
        };
    }
}
