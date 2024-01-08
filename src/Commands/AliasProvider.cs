using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Provides a set of methods for pattern matching of command names extracted from a chat message.
/// Each method accepts `isPrivileged` argument, which can eventually be used to determine if
/// the chatter can execute requested commands. True by default until specified otherwise
/// </summary>
internal class CommandAliasProvider
{
    public static readonly List<string> CharCommandAliases = new() { "char" };
    public static readonly List<string> GlowCommandAliases = new() { "glow" };
    public static readonly List<string> JoinCommandAliases = new() { "join" };
    public static readonly List<string> JumpCommandAliases = new() { "j", "l", "r", "u" };
    public static readonly List<string> UnglowCommandAliases = new() { "unglow" };

    public static bool MatchesCharacterChangeCommand(string commandName, bool isPrivileged = true)
    {
        return MatchesCommandAliasPattern(CharCommandAliases, commandName, isPrivileged);
    }

    public static bool MatchesGlowCommand(string commandName, bool isPrivileged = true)
    {
        return MatchesCommandAliasPattern(GlowCommandAliases, commandName, isPrivileged);
    }

    public static bool MatchesJoinCommand(string commandName, bool isPrivileged = true)
    {
        return MatchesCommandAliasPattern(JoinCommandAliases, commandName, isPrivileged);
    }

    public static bool MatchesJumpCommand(string commandName, bool isPrivileged = true)
    {
        return MatchesCommandAliasPattern(JumpCommandAliases, commandName, isPrivileged);
    }

    public static bool MatchesUnglowCommand(string commandName, bool isPrivileged = true)
    {
        return MatchesCommandAliasPattern(UnglowCommandAliases, commandName, isPrivileged);
    }

    private static bool MatchesCommandAliasPattern(List<string> aliases, string commandName, bool isPrivileged = true)
    {
        return aliases.Any(alias => commandName.StartsWith(alias)) && isPrivileged;
    }
}
