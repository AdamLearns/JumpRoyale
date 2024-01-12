#if TOOLS
using Godot;

namespace GdMUT;

/// <summary>
/// Entry point for the plugin.
/// </summary>
[Tool]
public partial class GDMUT : EditorPlugin
{
    private const string DOCK_SCENE = "res://addons/GDMUT/Dock.tscn";
    private Control _dock;

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _EnterTree()
    {
        base._EnterTree();
        _dock = GD.Load<PackedScene>(DOCK_SCENE).Instantiate<Control>();
        AddControlToDock(DockSlot.RightUl, _dock);
        GD.Print("Successfully loaded GDMUT");
    }

    /// <summary>
    /// Called when the node exits the scene tree.
    /// </summary>
    public override void _ExitTree()
    {
        base._ExitTree();
        RemoveControlFromDocks(_dock);
        _dock?.Free();
    }
}
#endif
