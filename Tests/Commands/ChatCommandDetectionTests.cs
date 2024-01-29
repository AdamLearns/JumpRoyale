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

    /// <summary>
    /// This test makes sure that when unprivileged user tries to execute a command, it won't get matched at all
    /// despite providing a valid alias. This is because we want to separate some commands to be available for
    /// channel Subscribers or possibly Mods in the future.
    /// </summary>
    [Test]
    public void CanRejectUnprivilegedUsers()
    {
        foreach (string command in new List<string>() { "glow", "namecolor" })
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
        [
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
        ];

        foreach (string command in commandsWithTypos)
        {
            (_, string commandName, bool wasMatched) = CommandNameMatcher(command, true);

            Assert.Multiple(() =>
            {
                Assert.That(command, Does.StartWith(commandName));
                Assert.That(wasMatched, Is.True, commandName);
            });
        }
    }

    /// <summary>
    /// Temporary command matcher, this roughly matches the existing pattern matcher in the <c>Arena</c> until the
    /// logic gets separated, so it can be tested properly.
    /// </summary>
    /// <param name="chatMessage">Supposed chat message sent by the user.</param>
    /// <param name="isPrivileged">Privileged state of this user privileged, allows executing the matched command when <c>true</c>.</param>
    private Tuple<string, string, bool> CommandNameMatcher(string chatMessage, bool isPrivileged = true)
    {
        CommandHandler commandHandler =
            new(chatMessage.ToLower(), string.Empty, string.Empty, string.Empty, isPrivileged);

        // Returning a callable method means that out chat message managed to match appropriate command
        CommandHandler.CallableCommand? command = commandHandler.TryGetCommandFromChatMessage();

        return command is not null
            ? new(chatMessage, commandHandler.ExecutedCommand.Name, true)
            : new(chatMessage, commandHandler.ExecutedCommand.Name, false);
    }
}
