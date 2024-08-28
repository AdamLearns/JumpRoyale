using Godot;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

/// <summary>
/// Base for drawable Arena objects, contains the atlas coordinates of sprites defined by provided arguments.
/// </summary>
/// <param name="Point">Represents a single point type, block or a cell, not visually connected to other sprites.</param>
/// <param name="Left">Represents the beginning of a platform.</param>
/// <param name="Middle">Represents the middle part of a platform - seamless extension drawn between left and right.</param>
/// <param name="Right">Represents the ending of a platform.</param>
public record BaseObject(Vector2I Point, Vector2I Left, Vector2I Middle, Vector2I Right) { }
