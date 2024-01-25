using Microsoft.Extensions.Configuration;

namespace TwitchChat;

public class ChannelConfiguration
{
    private readonly IConfigurationRoot _configuration;

    public ChannelConfiguration()
    {
        _configuration = new ConfigurationBuilder().AddUserSecrets<TwitchChatClient>().Build();
    }

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
        string? value =
            _configuration[index]
            ?? throw index switch
            {
                TwitchConstants.MissingAccessTokenError => new MissingTwitchAccessTokenException(),
                TwitchConstants.MissingChannelNameError => new MissingTwitchChannelNameException(),
                TwitchConstants.MissingChannelIdError => new MissingTwitchChannelIdException(),
                _ => new System.Exception($"Unhandled user-secrets index: ({index})"),
            };

        return value;
    }
}
