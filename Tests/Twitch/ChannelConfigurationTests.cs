using Microsoft.Extensions.Configuration;
using TwitchChat;

namespace Twitch;

[TestFixture]
public class ChannelConfigurationTests
{
    [Test]
    public void CanThrowAppropriateException()
    {
        ConfigurationBuilder builder = new();

        // Not an ideal example, but it's a good enough workaround to force empty keys before we inject the builder
        builder.Properties[TwitchConstants.TwitchAccessTokenKey] = new object();
        builder.Properties[TwitchConstants.TwitchChannelNameKey] = new object();
        builder.Properties[TwitchConstants.TwitchChannelIdKey] = new object();

        ChannelConfiguration config = new(builder);

        Assert.Multiple(() =>
        {
            Assert.That(() => config.AccessToken, Throws.InstanceOf<MissingTwitchAccessTokenException>());
            Assert.That(() => config.ChannelName, Throws.InstanceOf<MissingTwitchChannelNameException>());
            Assert.That(() => config.ChannelId, Throws.InstanceOf<MissingTwitchChannelIdException>());
        });
    }
}
