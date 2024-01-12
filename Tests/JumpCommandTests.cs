using System;
using System.Collections.Generic;
using GdMUT;

namespace Tests
{
    public class JumpCommandTests
    {
        /// <summary>
        /// This test ensures that both non-space and spaced formats result in the same angle
        /// and power applied to the jump
        /// </summary>
        [CSTestFunction]
        public static Result CanProcessSimpleJumps()
        {
            List<string> commandInputs = new() { "l60 30", "l 60 30" };

            foreach (string input in commandInputs)
            {
                (JumpCommand jump, _, _) = GetJumpFromCommand(input);

                if (jump.Angle != 30 || jump.Power != 30)
                {
                    return new Result(
                        false,
                        $"Expected Angle and Power to be 30, returned: ({jump.Angle}, {jump.Power})"
                    );
                }
            }

            return Result.Success;
        }

        [CSTestFunction]
        public static Result CanFallbackToDefaults()
        {
            List<string> commandInputs = new() { "j", "l", "r", "r r" };

            foreach (string input in commandInputs)
            {
                (JumpCommand jump, _, string commandName) = GetJumpFromCommand(input);

                if (jump.Angle != 90)
                {
                    return new Result(
                        false,
                        $"Expected Angle to fallback to default 90. ({jump.Angle}) given (command: {commandName})."
                    );
                }

                if (jump.Power != 100)
                {
                    return new Result(
                        false,
                        $"Expected Power to fallback to default 100. ({jump.Power}) given (command: {commandName})."
                    );
                }
            }

            return Result.Success;
        }

        [CSTestFunction]
        public static Result CanUseFixedAngle()
        {
            List<string> commandInputs =
                new() { "u u", "u", "u45", "ll", "lll", "rr", "rr65", "rrr", "rrr 15", "ll50l" };

            foreach (string input in commandInputs)
            {
                (JumpCommand jump, _, string commandName) = GetJumpFromCommand(input);

                int expectedAngle = JumpCommand.AngleFromDirectionCommand(commandName, 0, true);

                if (jump.Angle != expectedAngle)
                {
                    return new Result(
                        false,
                        $"Expected Angle to fallback to default {expectedAngle}. ({jump.Angle}) given."
                    );
                }
            }

            return Result.Success;
        }

        /// <summary>
        /// Fixed-angle jumps always override the Angle value and it should be interpreted as Power
        /// instead.
        /// </summary>
        [CSTestFunction]
        public static Result CanAdjustPowerOfFixedAngleJumps()
        {
            (JumpCommand jump, _, _) = GetJumpFromCommand("ll30");

            if (jump.Power != 30)
            {
                return new Result(false, $"Expected Power to be 30, ({jump.Power}) given.");
            }

            // Mind the angle conversion for the given command!!!
            if (jump.Angle != 60)
            {
                return new Result(false, $"Expected Angle to be 60, ({jump.Angle}) given.");
            }

            return Result.Success;
        }

        /// <summary>
        /// This is only for fixed-angle commands
        /// </summary>
        [CSTestFunction]
        public static Result CanAdjustPowerOnTypoedCommands()
        {
            List<string> commands = new() { "ujk50", "rrl20", "lll20", "rrr100" };

            foreach (string command in commands)
            {
                ChatCommandParser parsedCommand = new(command);

                int?[] arguments = parsedCommand.ArgumentsAsNumbers();

                JumpCommand jump = new(command, arguments[0], arguments[1]);

                if (jump.Power != arguments[0])
                {
                    return new Result(
                        false,
                        $"Failed to adjust the power from typo-ed command: ({command}). Asked for: ({arguments[0]}), result: ({jump.Power})"
                    );
                }
            }

            return Result.Success;
        }

        /// <summary>
        /// This test makes sure that we can still make a typo when sending a chat command
        /// and still get it to parse the provided value as Angle. Because the left side
        /// is being matched, the right side of the command can contain a typo
        /// </summary>
        [CSTestFunction]
        public static Result CanAdjustAngleWithTypoedCommands()
        {
            // "l l 5" will cause the test to fail, because the argument extractor will interpret
            // the first string as command name and the rest will be interpreted as arguments,
            // so the second string will default to "JUMP UP" (90 angle)
            List<string> commands = new() { "lk5", "rkl-5", "la 5", "jk -5", "l 5 l", "l5 ldfs" };

            foreach (string command in commands)
            {
                (JumpCommand jump, _, _) = GetJumpFromCommand(command);

                if (jump.Angle != 85)
                {
                    return new Result(
                        false,
                        $"Expected jump angle to be ({85}), ({jump.Angle}) given. Message: ({command})"
                    );
                }
            }

            return Result.Success;
        }

        /// <summary>
        /// Returns the processed Jump Command from the parsed chat command. The nullable is
        /// casted just for the sake of these tests
        /// </summary>
        /// <returns>JumpCommand instance, numeric arguments from command, Command Name</returns>
        private static Tuple<JumpCommand, int[], string> GetJumpFromCommand(string commandInput)
        {
            ChatCommandParser command = new(commandInput);

            int?[] arguments = command.ArgumentsAsNumbers();

            JumpCommand jump = new(command.Name, arguments[0], arguments[1]);

            // Note: arguments are interpreted as Angle/Power values
            return new(jump, new[] { arguments[0] ?? 0, arguments[1] ?? 100 }, command.Name);
        }
    }
}
