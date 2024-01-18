namespace Commands;

[TestFixture]
public class ChatCommandDetectionTests
{
    /// <summary>
    /// This test checks if the available alias matchers in the CommandMatcher can interpret incoming input
    /// as a potential command. Potential, meaning that commands are partially matched, e.g. <c>Left</c> will
    /// still match the <c>l</c> alias, otherwise that's just unnecessary restriction.
    /// <para>
    /// Assumption: command matching in the game is implemented based on the string length, meaning that full
    /// command name is matched first, then eventual alias, but ideally there should be no collision between
    /// command names and aliases of different logic, e.g. <c>Join</c> and <c>j</c> (jump).
    /// </para>
    /// <para>
    /// This means that full command name should always have the highest priority during pattern matching.
    /// </para>
    /// </summary>
    [Test]
    public void CanMatchAvailableCommands()
    {
        foreach (string command in CommandMatcher.AvailableCommands)
        {
            (string message, string commandName, bool wasMatched) = CommandNameMatcher(command);

            Assert.That(
                wasMatched,
                Is.True,
                $"Tried to match a command in the following chat message: {message}. Command parsed as: {commandName}. No match returned."
            );
        }
    }

    [Test]
    public void CanRejectUnprivilegedUsers()
    {
        foreach (string command in CommandMatcher.AvailableCommands)
        {
            (_, string commandName, bool wasMatched) = CommandNameMatcher(command, false);

            Assert.Multiple(() =>
            {
                // The following is probably unnecessary, but better be sure that we test the correct command
                Assert.That(command, Does.StartWith(commandName));
                Assert.That(wasMatched, Is.False);
            });
        }
    }

    /// <summary>
    /// Note about this test, we are only watching the string part, where the chat message
    /// starts with the command name and leave it like that, so we can still get the
    /// player to execute the command. Glow is an exception: hex type arguments.
    /// </summary>
    [Test]
    public void CanMatchTypos()
    {
        // Note: we allow slight typos in the chat i.e. extra character, because we wouldn't
        // want to force the player to type again when something like "lk 30" was sent.
        // Typo-ed Glow will ditch the argument, if no space was provided
        List<string> commandsWithTypos =
            new()
            {
                "chars",
                "char 3 3",
                "gloww",
                "glow uuuuu",
                "glowf0f",
                "glow aaa",
                "llll 234432",
                "joinerino",
                "jumperoni 30 50",
                "uwu 45",
                "jjjj",
                "rrrr",
                "llll",
                "rrl42",
                "u u",
                "u uu7",
            };

        foreach (string command in commandsWithTypos)
        {
            (_, string commandName, bool wasMatched) = CommandNameMatcher(command, false);

            Assert.Multiple(() =>
            {
                Assert.That(command, Does.StartWith(commandName));
                Assert.That(wasMatched, Is.False);
            });
        }
    }

    private Tuple<string, string, bool> CommandNameMatcher(string chatMessage, bool isPrivileged = true)
    {
        ChatCommandParser command = new(chatMessage.ToLower());

        // TODO: replace this with some external pattern matcher without hardcoding the cases...
        return command.Name switch
        {
            string when CommandMatcher.MatchesJoin(command.Name, isPrivileged) => new(chatMessage, command.Name, true),
            string when CommandMatcher.MatchesUnglow(command.Name, isPrivileged)
                => new(chatMessage, command.Name, true),
            string when CommandMatcher.MatchesJump(command.Name, isPrivileged) => new(chatMessage, command.Name, true),
            string when CommandMatcher.MatchesCharacterChange(command.Name, isPrivileged)
                => new(chatMessage, command.Name, true),
            string when CommandMatcher.MatchesGlow(command.Name, isPrivileged) => new(chatMessage, command.Name, true),

            // The below return is here just to check if there were cases when nothing was caught by
            // pattern matching
            _ => new(chatMessage, command.Name, false),
        };
    }
}
