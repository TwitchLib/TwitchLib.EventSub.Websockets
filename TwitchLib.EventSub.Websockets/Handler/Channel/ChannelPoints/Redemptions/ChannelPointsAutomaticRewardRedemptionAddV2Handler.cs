using TwitchLib.EventSub.Core.SubscriptionTypes.Channel;
using TwitchLib.EventSub.Websockets.Core.EventArgs.Channel;
using TwitchLib.EventSub.Websockets.Core.Models;

namespace TwitchLib.EventSub.Websockets.Handler.Channel.ChannelPoints.Redemptions;

internal class ChannelPointsAutomaticRewardRedemptionAddV2Handler : NotificationHandler<ChannelPointsAutomaticRewardRedemptionAddV2Args, EventSubNotification<ChannelPointsAutomaticRewardRedemptionV2>>
{
    public override string SubscriptionType => "channel.channel_points_automatic_reward_redemption.add";

    public override string SubscriptionVersion => "2";

    public override string EventName => nameof(EventSubWebsocketClient.ChannelPointsAutomaticRewardRedemptionAddV2);
}