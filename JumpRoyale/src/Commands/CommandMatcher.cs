using System.Collections.Immutable;

/// <summary>
/// Provides a set of methods for pattern matching of command names extracted from a chat message.
/// Each method accepts <c>isPrivileged</c> argument, which can eventually be used to determine if
/// the chatter can execute requested commands. True by default until specified otherwise.
/// </summary>
/// <remarks>
/// The accepted <c>isPrivileged</c> checked during the command matching is only used to determine if
/// the chatter can execute the command. This argument can also be passed to command methods
/// in order to execute a separate part of the logic reserved for privileged users only.
/// </remarks>
public static class CommandMatcher
{
    public static readonly ImmutableList<string> CharCommandAliases = ImmutableList.Create("char");
    public static readonly ImmutableList<string> GlowCommandAliases = ImmutableList.Create("glow");
    public static readonly ImmutableList<string> JoinCommandAliases = ImmutableList.Create("join");
    public static readonly ImmutableList<string> JumpCommandAliases = ImmutableList.Create("j", "l", "r", "u");
    public static readonly ImmutableList<string> UnglowCommandAliases = ImmutableList.Create("unglow");

    public static bool MatchesCharacterChange(string commandName, bool isPrivileged = true)
    {
        return MatchesCommandAliasPattern(CharCommandAliases, commandName, isPrivileged);
    }

    public static bool MatchesGlow(string commandName, bool isPrivileged = true)
    {
        return MatchesCommandAliasPattern(GlowCommandAliases, commandName, isPrivileged);
    }

    public static bool MatchesJoin(string commandName, bool isPrivileged = true)
    {
        return MatchesCommandAliasPattern(JoinCommandAliases, commandName, isPrivileged);
    }

    public static bool MatchesJump(string commandName, bool isPrivileged = true)
    {
        return MatchesCommandAliasPattern(JumpCommandAliases, commandName, isPrivileged);
    }

    public static bool MatchesUnglow(string commandName, bool isPrivileged = true)
    {
        return MatchesCommandAliasPattern(UnglowCommandAliases, commandName, isPrivileged);
    }

    private static bool MatchesCommandAliasPattern(
        ImmutableList<string> aliases,
        string commandName,
        bool isPrivileged = true
    )
    {
        return aliases.Exists(alias => commandName.StartsWith(alias)) && isPrivileged;
    }
}
