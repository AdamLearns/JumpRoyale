namespace Commands;

[TestFixture]
public class JumpCommandTests
{
    /// <summary>
    /// This test makes sure that both non-space and spaced formats result in the same angle
    /// and power applied to the jump.
    /// </summary>
    [Test]
    public void CanOmitSpaceInJumpCommands()
    {
        // Reminder, Angle input of 60 to the Left evaluates to 30
        List<string> commandInputs = ["l60 30", "l 60 30"];

        foreach (string input in commandInputs)
        {
            (JumpCommand jump, _, _) = GetJumpFromCommand(input);

            Assert.Multiple(() =>
            {
                Assert.That(jump.Angle, Is.EqualTo(30));
                Assert.That(jump.Power, Is.EqualTo(30));
            });
        }
    }

    /// <summary>
    /// This test makes sure that when no inputs are provided by the user, the command will use default values for
    /// angle and power.
    /// </summary>
    [Test]
    public void CanFallbackToDefaults()
    {
        List<string> commandInputs = ["j", "l", "r", "r r", "u"];

        foreach (string input in commandInputs)
        {
            (JumpCommand jump, _, string commandName) = GetJumpFromCommand(input);

            Assert.Multiple(() =>
            {
                Assert.That(jump.Angle, Is.EqualTo(90), $"Command executed: {commandName}");
                Assert.That(jump.Power, Is.EqualTo(100), $"Command executed: {commandName}");
            });
        }
    }

    /// <summary>
    /// This test makes sure that we will always jump up with any combination of parameters with <c>u</c> command.
    /// </summary>
    [Test]
    public void CanAlwaysJumpUpWithAmbiguousParameters()
    {
        List<string> commandInputs = ["u u", "u", "u45", "u 45", "u 70 70", "u 100 90"];

        foreach (string input in commandInputs)
        {
            (JumpCommand jump, _, string commandName) = GetJumpFromCommand(input);

            Assert.That(jump.Angle, Is.EqualTo(90), $"Executed command: {commandName}");
        }
    }

    /// <summary>
    /// This test ensures that we can adjust the jump power of <c>u</c> command when passing just one parameter, which
    /// normally is interpreted as <c>Angle</b> for other commands. This will also check if angle remains unchanged.
    /// </summary>
    [Test]
    public void CanAdjustPowerOfJumpUp()
    {
        // Mind the angle conversion for the given command! "u" returns 90, because we don't convert it
        List<string> commandInputs = ["u50", "u 50", "u50 50"];

        foreach (string input in commandInputs)
        {
            (JumpCommand jump, _, _) = GetJumpFromCommand(input);

            Assert.Multiple(() =>
            {
                Assert.That(jump.Angle, Is.EqualTo(90));
                Assert.That(jump.Power, Is.EqualTo(50));
            });
        }
    }

    /// <summary>
    /// Similarly to <see cref="CanAdjustPowerOfJumpUp"/>, but here we check if it still gets interpreted as UP and
    /// we will only affect the Power.
    /// </summary>
    [Test]
    public void CanAdjustPowerOfTypoedJumpUp()
    {
        // Mind the angle conversion for the given command! "u" returns 90, because we don't convert it
        List<string> commandInputs = ["ughk50", "uj 50", "ufjh50 50"];

        foreach (string input in commandInputs)
        {
            (JumpCommand jump, _, _) = GetJumpFromCommand(input);

            Assert.Multiple(() =>
            {
                Assert.That(jump.Angle, Is.EqualTo(90));
                Assert.That(jump.Power, Is.EqualTo(50));
            });
        }
    }

    /// <summary>
    /// This test makes sure that we can still make a typo when sending a chat command and get it to parse the provided
    /// value as Angle. Because the left side is being matched, the right side of the command can contain a typo as
    /// long as there is no space, because the right side will then get interpreted as a parameter.
    /// </summary>
    [Test]
    public void CanAdjustAngleWithTypoedCommands()
    {
        // "l l 5" will cause the test to fail, because the argument extractor will interpret
        // the first string as command name and the rest will be interpreted as arguments,
        // so the second string will default to "JUMP UP" (90 angle)
        List<string> commands = ["lk5", "rkl-5", "la 5", "jk -5", "l 5 l", "l5 ldfs"];

        foreach (string command in commands)
        {
            (JumpCommand jump, _, _) = GetJumpFromCommand(command);

            Assert.That(jump.Angle, Is.EqualTo(85));
        }
    }

    [Test]
    public void ExtraJumpAliasesProduceExpectedAngleResults()
    {
        Dictionary<string, int> commands =
            new()
            {
                { "rrr", 150 },
                { "jjj", 150 },
                { "rr", 120 },
                { "jj", 120 },
                { "lll", 30 },
                { "ll", 60 },
                { "u", 90 },
            };

        foreach (string command in commands.Keys)
        {
            (JumpCommand jump, _, _) = GetJumpFromCommand(command);

            Assert.That(jump.Angle, Is.EqualTo(commands[command]));
        }
    }

    /// <summary>
    /// Returns the processed Jump Command from the parsed chat command. The nullable is
    /// casted just for the sake of these tests.
    /// </summary>
    /// <returns>JumpCommand instance, numeric arguments from command, Command Name.</returns>
    private Tuple<JumpCommand, int[], string> GetJumpFromCommand(string commandInput)
    {
        ChatCommandParser command = new(commandInput);

        int?[] arguments = command.ArgumentsAsNumbers();

        JumpCommand jump = new(command.Name, arguments[0], arguments[1]);

        // Note: arguments are interpreted as Angle/Power values
        return new(jump, [arguments[0] ?? 0, arguments[1] ?? 100], command.Name);
    }
}
