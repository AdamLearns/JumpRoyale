using System;
using System.Collections.Generic;

public class JumpCommand : CommandParser
{
    private int _power = 100;

    public int Angle { get; set; } = 0;
    public int Power
    {
        get { return _power; }
        set { _power = Math.Clamp(value, 1, 100); }
    }

    private readonly List<string> _allowedCommandAliases = new() { "jump", "j", "l", "u", "r" };

    public JumpCommand(string chatMessage)
        : base(chatMessage)
    {
        string angleFromCommand = _arguments.Length > 1 ? _arguments[1] : "0";
        string powerFromCommand = _arguments.Length > 2 ? _arguments[2] : "100";

        if (int.TryParse(angleFromCommand, out int angle))
        {
            /// We only have to clamp the user value at this point, because we override
            /// that value to match the direction we want to jump in
            Angle = Math.Clamp(angle, -90, 90);
        }

        if (int.TryParse(powerFromCommand, out int power))
        {
            Power = power;
        }

        AdjustAngle();
    }

    public override bool IsValid()
    {
        return _allowedCommandAliases.Contains(GetCommandName());
    }

    public void AdjustAngle()
    {
        /// The "u" alias is an exception, where the angle is always UP, but we should be able
        /// to at least modify the power of upwards jump and we will do this by assigning
        /// the first argument as Power, since we are overriding Angle (Git: Issue #22)
        if (GetCommandName() == "u")
        {
            /// The default Angle value is always 0, so we will only assign the Angle if user
            /// has actually provided something, otherwise jump at maximum power (100)
            Power = Angle != 0 ? Angle : 100;
        }

        Angle = GetCommandName() switch
        {
            "jump" or "j" or "r" => Angle + 90,
            "l" => 90 - Angle,
            "u" => 90,
            _ => 90 /* fall back to upwards */
        };
    }
}
