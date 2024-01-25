using Microsoft.Extensions.Configuration;

namespace TwitchChat;

/// <summary>
/// Provides the configuration keys for Twitch channel.
/// </summary>
/// <param name="config">Builder template with already included data.</param>
public class ChannelConfiguration(ConfigurationBuilder config)
{
    private readonly IConfigurationRoot _configuration = config.Build();

    public string AccessToken
    {
        get => TryGetPropertyFromConfig(TwitchConstants.TwitchAccessTokenKey);
    }

    public string? ChannelName
    {
        get => TryGetPropertyFromConfig(TwitchConstants.TwitchChannelNameKey);
    }

    public string? ChannelId
    {
        get => TryGetPropertyFromConfig(TwitchConstants.TwitchChannelIdKey);
    }

    private string TryGetPropertyFromConfig(string index)
    {
        // When trying to access an unset configuration key, throw an appropriate exception type with custom message. If
        // more configuration keys are added and we don't have a specific exception for that, we will throw a generic
        // exception. This is only for readability purposes, messages are set through TwitchConstants
        string? value =
            _configuration[index]
            ?? throw index switch
            {
                TwitchConstants.TwitchAccessTokenKey => new MissingTwitchAccessTokenException(),
                TwitchConstants.TwitchChannelNameKey => new MissingTwitchChannelNameException(),
                TwitchConstants.TwitchChannelIdKey => new MissingTwitchChannelIdException(),
                _ => new System.Exception($"Missing user-secrets index: ({index})"),
            };

        return value;
    }
}
