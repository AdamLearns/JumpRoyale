using System;
using System.Collections.Generic;

internal class JumpCommand
{
    private int _power;

    /// <summary>
    /// Specifies at what angle the player should jump, where 0 points up and the angles count
    /// Clockwise. Positive: right, negative: left.
    /// </summary>
    /// <remarks>
    /// This value is clamped between -90 and 90. See <see cref="AdjustAngle"/> for external
    /// clamp exceptions
    /// </remarks>
    public int Angle { get; private set; } = 0;

    /// <summary>
    /// Jump power in form of "percentage" the players can apply as an argument in the chat
    /// message. This value is clamped between `1` and `100`.
    /// </summary>
    public int Power
    {
        get { return _power; }
        private set { _power = Math.Clamp(value, 1, 100); }
    }

    private readonly List<string> _fixedAngleDirections = new() { "u", "ll", "lll", "jj", "rr", "jjj", "rrr" };

    internal JumpCommand(string direction, int? angle, int? power)
    {
        Angle = angle ?? 0;
        Power = power ?? 100;

        AdjustAngle(direction);
    }

    private void AdjustAngle(string direction)
    {
        /// There are commands, which imply in which direction we would like to jump without
        /// providing an angle. We don't want to explicitly specify an angle to jump
        /// straight up, which is inconvenient when avoiding duplicate messages
        if (_fixedAngleDirections.Contains(direction))
        {
            /// Players may still want to specify their own Power when jumping at fixed-angle, so
            /// we can get around this by interpreting the first parameter as Power without
            /// having to specify Angle, then Power to read the second parameter
            Power = Angle != 0 ? Angle : 100;
        }

        /// Important: the reason why this value is not clamped in the property itself is that we
        /// have commands, which have implied angle and only accept Power as parameter, which
        /// we read from Angle and we can't clamp this value, otherwise we lose 10 Power
        Angle = Math.Clamp(Angle, -90, 90);

        Angle = direction switch
        {
            "j" or "r" => Angle + 90,
            "l" => 90 - Angle,
            "u" => 90,
            "ll" => 60, /*           Warning:         */
            "lll" => 30, /*            Experimental   */
            "rr" or "jj" => 120, /*      Commands     */
            "rrr" or "jjj" => 150, /*            :)   */
            _ => 90 /* fall back to upwards */
        };

        // TODO: Consider testing other fixed-angle commands: Dash Left / Right (dl/dr)
        // Angle: 80 or 75
    }
}
