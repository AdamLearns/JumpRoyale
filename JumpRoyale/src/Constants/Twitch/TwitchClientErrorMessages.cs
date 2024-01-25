public static partial class TwitchConstants
{
    public const string MissingAccessTokenError =
        $"No access token found. Please run `dotnet user-secrets set {TwitchAccessTokenKey} <your access token>`";

    public const string MissingChannelNameError =
        $"Channel not found. Please run `dotnet user-secrets set {TwitchChannelNameKey} <your twitch channel name>`";

    public const string MissingChannelIdError =
        $"Channel Id not found. Please run `dotnet user-secrets set {TwitchChannelIdKey} <your twitch channel id>`";
}
