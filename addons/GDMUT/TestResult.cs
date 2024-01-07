#if TOOLS
using System.Collections.Generic;
using Godot;

namespace GdMUT.Components;

/// <summary>
/// A test result object within the test result list.
/// </summary>
[Tool]
public partial class TestResult : Control
{
    private const string TYPE_NAME_FORMAT = "[b][font_size=24][center]{0}[/center][/font_size][/b]";
    private const string METHOD_RESULT_SCENE = "res://addons/GDMUT/MethodResult.tscn";

    [Export]
    private RichTextLabel _typeName;

    [Export]
    private VBoxContainer _methodList;

    private List<(MethodResult, TestFunction)> _functions = new();
    private string _typeNameStr;

    /// <inheritdoc/>
    public override void _EnterTree()
    {
        base._EnterTree();
        foreach (Node child in _methodList.GetChildren())
        {
            child.QueueFree();
        }

        _functions.Clear();
    }

    /// <summary>
    /// Set the type name of the test result.
    /// </summary>
    /// <param name="typeName">Type name.</param>
    public void SetTypeName(string typeName)
    {
        _typeNameStr = typeName;
        _typeName.Text = string.Format(TYPE_NAME_FORMAT, typeName);
    }

    /// <summary>
    /// Update the test result.
    /// </summary>
    public void UpdateResult()
    {
        int numSuccess = 0;
        foreach (var (methodResult, function) in _functions)
        {
            methodResult.Update();
            numSuccess += function.Result.IsSuccess ? 1 : 0;
        }

        _typeName.Text = string.Format(
            TYPE_NAME_FORMAT,
            _typeNameStr + $" ({numSuccess}/{_functions.Count})"
        );
        if (numSuccess == _functions.Count)
        {
            _typeName.Modulate = new Color(0, 1, 0);
        }
        else if (numSuccess == 0)
        {
            _typeName.Modulate = new Color(1, 0, 0);
        }
        else
        {
            _typeName.Modulate = new Color(1, 0.9f, 0);
        }
    }

    /// <summary>
    /// Adds a method result to the test result.
    /// </summary>
    /// <param name="function">The test function.</param>
    public void AddMethodResult(TestFunction function)
    {
        var methodResultScene = GD.Load<PackedScene>(METHOD_RESULT_SCENE);
        var methodResult = methodResultScene.Instantiate<MethodResult>();
        methodResult.SetMethodResult(function);
        _methodList.AddChild(methodResult);
        _functions.Add((methodResult, function));
    }
}
#endif
