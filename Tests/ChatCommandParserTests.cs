using System.Collections.Generic;
using GdMUT;

namespace Tests
{
    public class ChatCommandParserTests
    {
        [CSTestFunction]
        public static Result CanExtractCommandName()
        {
            List<string> commandInputs = new() { "j", "l", "u", "r", "l3", "l30 30", "r 30", "u 30 30" };

            foreach (string input in commandInputs)
            {
                ChatCommandParser command = new(input);

                if (command.Name is "")
                {
                    return new Result(false, $"Could not extract the name from: ({input})");
                }
            }

            return Result.Success;
        }

        /// <summary>
        /// Not all messages are "valid", game commands ideally should be [a-zA-Z0-9-], so
        /// as a fallback, there should be an empty string in return and sending just
        /// a number is still a valid message interpreted as a command
        /// </summary>
        [CSTestFunction]
        public static Result CanHandleGarbageMessage()
        {
            List<string> commandInputs = new() { "", " ", ";", "!", "[]", "[#@}=]" };

            foreach (string input in commandInputs)
            {
                ChatCommandParser command = new(input);

                if (command.Name is not "")
                {
                    return new Result(false, $"The input: ({input}) did not return an empty string");
                }
            }

            return Result.Success;
        }

        /// <summary>
        /// This tests ensures that both command formats work, where the initial space is omitted
        /// and the argument is extracted properly
        /// </summary>
        [CSTestFunction]
        public static Result CanExtractStringArguments()
        {
            List<string> commandInputs = new() { "l30 30", "l 30 30" };

            foreach (string input in commandInputs)
            {
                ChatCommandParser command = new(input);

                string[] arguments = command.ArgumentsAsStrings();

                foreach (string argument in arguments)
                {
                    if (argument != "30")
                    {
                        return new Result(
                            false,
                            $"Expected argument: 30 to be extracted from ({input}). Extracted ({argument})"
                        );
                    }
                }
            }

            return Result.Success;
        }

        [CSTestFunction]
        public static Result CanExtractNumericArguments()
        {
            List<string> commandInputs = new() { "l30 30", "l 30 30" };

            foreach (string input in commandInputs)
            {
                ChatCommandParser command = new(input);

                int?[] arguments = command.ArgumentsAsNumbers();

                foreach (int? argument in arguments)
                {
                    if (argument != 30)
                    {
                        return new Result(
                            false,
                            $"Expected argument: 30 to be extracted from ({input}). Extracted: ({argument})"
                        );
                    }
                }
            }

            return Result.Success;
        }

        /// <summary>
        /// This test ensures that when we send just a matching command, we won't get any
        /// extra arguments on the arguments list
        /// </summary>
        [CSTestFunction]
        public static Result CanPadStringArgumentsWithNulls()
        {
            ChatCommandParser command = new("l");

            string[] arguments = command.ArgumentsAsStrings();

            if (arguments[0] is not null || arguments[1] is not null)
            {
                return new Result(
                    false,
                    $"Expected both string arguments to be null: ({arguments[0]}, {arguments[1]})"
                );
            }

            return Result.Success;
        }

        [CSTestFunction]
        public static Result CanPadNumericArgumentsWithNulls()
        {
            ChatCommandParser command = new("l");

            int?[] arguments = command.ArgumentsAsNumbers();

            if (arguments[0] is not null || arguments[1] is not null)
            {
                return new Result(
                    false,
                    $"Expected both string arguments to be null: ({arguments[0]}, {arguments[1]})"
                );
            }

            return Result.Success;
        }

        /// <summary>
        /// There are commands, which take mixed type of input, apart from numbers or simple
        /// strings (just letters). For example, `glow` command can change the particle
        /// color and it takes Hex input from the user, which is a mixed type
        /// </summary>
        [CSTestFunction]
        public static Result CanExtractArgumentOfMixedType()
        {
            List<string> expectedOutputs = new() { "09B0D9", "BDFF00", "123456", "f02", "f02bc6" };

            foreach (string output in expectedOutputs)
            {
                // Just test for both non-space and space
                List<string> commandInputs = new() { $"glow {output}", $"glow{output}" };

                foreach (string input in commandInputs)
                {
                    ChatCommandParser command = new(input);

                    string[] arguments = command.ArgumentsAsStrings();

                    if (arguments[0] != output)
                    {
                        return new Result(
                            false,
                            $"Expected to extract ({output}) from ({input}), got 1:({arguments[0]}) / 2:({arguments[1]}) instead."
                        );
                    }
                }
            }

            return Result.Success;
        }

        [CSTestFunction]
        public static Result RejectsInvalidGlowColors()
        {
            List<string> invalidColors = new() { "1234", "90 8da", "ffdd1", "v90909", "8888888", "l", "2" };

            foreach (string color in invalidColors)
            {
                List<string> commandInputs = new() { $"glow {color}", $"glow{color}" };

                foreach (string input in commandInputs)
                {
                    ChatCommandParser command = new(input);

                    string[] arguments = command.ArgumentsAsStrings();

                    if (arguments[0] is not null)
                    {
                        return new Result(false, $"Expected invalid glow to become null, ({arguments[0]}) given");
                    }
                }
            }

            return Result.Success;
        }
    }
}
