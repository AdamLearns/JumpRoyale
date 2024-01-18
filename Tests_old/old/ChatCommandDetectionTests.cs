/* using System;
using System.Collections.Generic;
using GdMUT;
using Godot;

namespace Tests
{
    public class ChatCommandDetectionTests
    {
        private static Tuple<string, bool> CommandNameMatcher(string chatMessage, bool isPrivileged = true)
        {
            ChatCommandParser command = new(chatMessage.ToLower());

            // Arguments have already been tested by Command Parser

            return command.Name switch
            {
                string when CommandMatcher.MatchesJoin(command.Name, isPrivileged) => new(command.Name, true),
                string when CommandMatcher.MatchesUnglow(command.Name, isPrivileged) => new(command.Name, true),
                string when CommandMatcher.MatchesJump(command.Name, isPrivileged) => new(command.Name, true),
                string when CommandMatcher.MatchesCharacterChange(command.Name, isPrivileged)
                    => new(command.Name, true),
                string when CommandMatcher.MatchesGlow(command.Name, isPrivileged) => new(command.Name, true),
                // null is here just to check if there were cases when nothing was caught by
                // pattern matching
                _ => new(null, false),
            };
        }


        /// <summary>
        /// Note about this test, we are only watching the string part, where the chat message
        /// starts with the command name and leave it like that, so we can still get the
        /// player to execute the command. Glow is an exception: hex type arguments
        /// </summary>
        [CSTestFunction]
        public static Result CanMatchTypos()
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
                    "u uu7"
                };

            foreach (string command in commandsWithTypos)
            {
                (string caughtCommand, _) = CommandNameMatcher(command);

                if (caughtCommand is null)
                {
                    return new Result(false, $"Failed to detect a command with in this message: ({command})");
                }
            }

            return Result.Success;
        }


    }
}
 */
