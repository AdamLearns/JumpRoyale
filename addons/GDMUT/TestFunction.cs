#if TOOLS
using System;
using System.Reflection;

namespace GdMUT;

/// <summary>
/// A single test function.
/// </summary>
public class TestFunction
{
    /// <summary>
    /// The name of the function.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The type of the function.
    /// </summary>
    public Type Type { get; set; }

    /// <summary>
    /// The method info of the function.
    /// </summary>
    public MethodInfo Method { get; set; }

    /// <summary>
    /// The result of the test function.
    /// </summary>
    public Result Result { get; set; }
}
#endif
