#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

/// <summary>
/// Represents a fake player that will be jumping on its own to visualize the jumping instructions.
/// </summary>
/// <param name="UserId">Matches the twitch user id from <c>PlayerData</c>. This argument uses a fake id for easier
/// lookup.</param>
/// <param name="DisplayName">Matches the twitch user display name from <c>PlayerData</c>. Should contain the jump
/// command example.</param>
/// <param name="XSpawnPosition">Should be set to appropriate position on x-axis based on the angle jump to not make the
/// bot jump at the wall.</param>
/// <param name="JumpAngle">From 0 to 180 (automatically converted in the jump command to appropriate angle).</param>
public record InstructionBot(string UserId, string DisplayName, int XSpawnPosition, int JumpAngle);
