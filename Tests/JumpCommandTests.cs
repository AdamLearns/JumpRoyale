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
                (JumpCommand jump, int[] arguments, string commandName) = GetJumpFromCommand(input);

                int expectedAngle = commandName switch
                {
                    "u" => 90,
                    "ll" => 60,
                    "lll" => 30,
                    "rr" or "jj" => 120,
                    "rrr" or "jjj" => 150,
                    _ => throw new Exception()
                };

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
        /// Returns the processed Jump Command from the parsed chat command. The nullable is
        /// casted just for the sake of these tests
        /// </summary>
        private static Tuple<JumpCommand, int[], string> GetJumpFromCommand(string commandInput)
        {
            ChatCommandParser command = new(commandInput);

            int?[] arguments = command.ArgumentsAsNumbers();

            JumpCommand jump = new(command.Name, arguments[0], arguments[1]);

            return new(jump, new[] { arguments[0] ?? 0, arguments[1] ?? 100 }, command.Name);
        }
    }
}
