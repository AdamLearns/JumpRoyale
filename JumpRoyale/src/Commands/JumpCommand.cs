using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

public class JumpCommand
{
    private readonly List<string> _fixedAngleDirections = new() { "u", "ll", "lll", "jj", "rr", "jjj", "rrr" };

    private int _power;

    public JumpCommand([NotNull] string direction, int? angle, int? power)
    {
        Angle = angle ?? 0;
        Power = power ?? 100;

        // Note on [NotNull]: we already know the direction is not null, because it was caught by the CommandMatcher,
        // so this should never happen (?) 🤔
        AdjustAngle(direction);
    }

    /// <summary>
    /// Gets the clamped angle from user inputs. Angle of 0 degrees points up and the angles count Clockwise.
    /// Positive: right, negative: left.
    /// </summary>
    /// <remarks>
    /// See <see cref="AdjustAngle"/> for details on external clamp of this value.
    /// </remarks>
    public int Angle { get; private set; }

    /// <summary>
    /// Gets the current jump power in form of "percentage" after the players can apply as an argument in the chat
    /// message. This value is clamped between `1` and `100`.
    /// </summary>
    public int Power
    {
        get { return _power; }
        private set { _power = Math.Clamp(value, 1, 100); }
    }

    private void AdjustAngle(string direction)
    {
        // There are commands, which imply in which direction we would like to jump without
        // providing an angle. We don't want to explicitly specify an angle to jump
        // straight up, which is inconvenient when avoiding duplicate messages
        if (MatchesFixedAngleAlias(direction))
        {
            // Players may still want to specify their own Power when jumping at fixed-angle, so
            // we can get around this by interpreting the first parameter as Power without
            // having to specify Angle, then Power to read the second parameter
            Power = Angle != 0 ? Angle : 100;
        }

        // Important: the reason why this value is not clamped in the property itself is that we
        // have commands, which have implied angle and only accept Power as parameter, which
        // we read from Angle and we can't clamp this value, otherwise we lose 10 Power
        Angle = Math.Clamp(Angle, -90, 90);

        Angle = direction switch
        {
            // Predefined set of angles for available jump commands. Experimental commands were
            // defined just as repeated letters for easier typing. The pattern matching has
            // been changed to allow typos and garbage and still allow changing the angle
            // Warning: experimental commands - rrr/rr/lll/ll :)
            // TODO: Change the format of double/triple char commands, because they can collide
            // with intentionally typo-ed commands for message duplication evasion
            string when direction.StartsWith("rrr") || direction.StartsWith("jjj") => 150,
            string when direction.StartsWith("rr") || direction.StartsWith("jj") => 120,
            string when direction.StartsWith("lll") => 30,
            string when direction.StartsWith("ll") => 60,
            string when direction.StartsWith('j') || direction.StartsWith('r') => Angle + 90,
            string when direction.StartsWith('l') => 90 - Angle,
            string when direction.StartsWith('u') => 90,
            _ => 90
        };
    }

    private bool MatchesFixedAngleAlias(string direction)
    {
        return _fixedAngleDirections.Exists(alias => direction.StartsWith(alias));
    }
}
