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
            Assert.DoesNotThrow(() =>
            {
                CommandNameMatcher(command);
            });
        }
    }

    [Test]
    public void CanRejectUnprivilegedUsers()
    {
        void TryMatchingUnprivileged()
        {
            foreach (string command in CommandMatcher.AvailableCommands)
            {
                CommandNameMatcher(command, false);
            }
        }

        Assert.Throws<Exception>(TryMatchingUnprivileged);
    }

    private bool CommandNameMatcher(string chatMessage, bool isPrivileged = true)
    {
        ChatCommandParser command = new(chatMessage.ToLower());

        // TODO: replace this with some external pattern matcher without hardcoding the cases...
        return command.Name switch
        {
            string when CommandMatcher.MatchesJoin(command.Name, isPrivileged) => true,
            string when CommandMatcher.MatchesUnglow(command.Name, isPrivileged) => true,
            string when CommandMatcher.MatchesJump(command.Name, isPrivileged) => true,
            string when CommandMatcher.MatchesCharacterChange(command.Name, isPrivileged) => true,
            string when CommandMatcher.MatchesGlow(command.Name, isPrivileged) => true,

            // The Exception is here just to check if there were cases when nothing was caught by
            // pattern matching
            _ => throw new Exception(),
        };
    }
}
