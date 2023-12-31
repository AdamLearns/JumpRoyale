using System;
using System.Collections.Generic;

public class JumpCommand : BaseChatCommand
{
    private int _power = 100;

    /// <summary>
    /// Angle extracted from the chat command. If there was no Angle provided in the arguments,
    /// it will default to `0` and be overridden internally, based on the command alias.
    /// This value is clamped between `-90` and `90`.
    ///
    /// Note that this may get interpreted as power, which is why we don't clamp it here.
    /// </summary>
    public int Angle { get; set; } = 0;

    /// <summary>
    /// Jump power in form of "_percentage_" the players can apply as an argument in the chat
    /// message. This value is clamped between `1` and `100`.
    /// </summary>
    public int Power
    {
        get { return _power; }
        set { _power = Math.Clamp(value, 1, 100); }
    }

    private readonly List<string> _allowedCommandAliases =
        new() { "jump", "j", "l", "u", "r", "ul", "ur" };

    private readonly List<string> _fixedAngleCommands = new() { "u", "ur", "ul" };

    public JumpCommand(string chatMessage)
        : base(chatMessage)
    {
        string angleFromCommand = arguments.Length > 1 ? arguments[1] : "0";
        string powerFromCommand = arguments.Length > 2 ? arguments[2] : "100";

        if (int.TryParse(angleFromCommand, out int angle))
        {
            Angle = angle;
        }

        if (int.TryParse(powerFromCommand, out int power))
        {
            Power = power;
        }

        AdjustAngle();
    }

    public override bool IsValid()
    {
        return _allowedCommandAliases.Contains(Name);
    }

    private void AdjustAngle()
    {
        /// The command aliases are exceptions, where the angle is fixed, but we should be able
        /// to at least modify the power of those jump and we will do this by assigning the
        /// first argument as Power, so we don't want to force the Angle to be present
        if (_fixedAngleCommands.Contains(Name))
        {
            /// The default Angle value is always 0, so we will only assign the Angle if the user
            /// has actually provided something, otherwise jump at maximum power (100). At
            /// this point we can't have clamped angle, because we will lose 10 power
            Power = Angle != 0 ? Angle : 100;
        }

        Angle = Math.Clamp(Angle, -90, 90);

        Angle = Name switch
        {
            "jump" or "j" or "r" => Angle + 90,
            "l" => 90 - Angle,
            "u" => 90,
            "ul" => 45,
            "ur" => 135,
            _ => 90 /* fall back to upwards */
        };
    }
}
