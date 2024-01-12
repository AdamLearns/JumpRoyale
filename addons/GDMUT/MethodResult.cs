#if TOOLS
using Godot;

namespace GdMUT.Components;

/// <summary>
/// A single test result.
/// </summary>
[Tool]
public partial class MethodResult : Control
{
    [Export]
    private RichTextLabel _methodName;

    [Export]
    private RichTextLabel _result;
    private TestFunction _function;

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _EnterTree()
    {
        base._EnterTree();
    }

    /// <summary>
    /// Sets the method result to display.
    /// </summary>
    /// <param name="function">The test function.</param>
    public void SetMethodResult(TestFunction function)
    {
        _function = function;
        _methodName.Text = function.Method.Name;
        Reset();
    }

    /// <summary>
    /// Updates the result of the test function.
    /// </summary>
    public void Update()
    {
        SetSuccess(_function.Result.IsSuccess, _function.Result.Message);
    }

    /// <summary>
    /// Resets the result to the default state.
    /// </summary>
    public void Reset()
    {
        _result.Text = string.Empty;
        SelfModulate = new Color(1, 1, 1);
    }

    /// <summary>
    /// Sets the status of the test function.
    /// </summary>
    /// <param name="isSuccess">Whether the test was a success or not.</param>
    /// <param name="result">The result string.</param>
    public void SetSuccess(bool isSuccess, string result = "")
    {
        _result.Text = (isSuccess ? "Success: " : "Failure: ") + result;
        Modulate = isSuccess ? new Color(0, 1, 0) : new Color(1, 0, 0);
        GD.Print($"{result} {isSuccess}");
    }
}
#endif
