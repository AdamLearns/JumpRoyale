#if TOOLS
using System;
using System.Collections.Generic;
using System.Reflection;
using Godot;

namespace GdMUT;

/// <summary>
/// Utility class for loading test functions.
/// </summary>
public static class TestLoader
{
    /// <summary>
    /// Search for all tests in the project.
    /// </summary>
    /// <returns>List of found test functions.</returns>
    public static List<TestFunction> SearchForAllTests()
    {
        List<TestFunction> tests = new();

        // get all functions with MonoTestFunctionAttribute
        ReadOnlySpan<Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies();
        for (int assemblyIndex = 0; assemblyIndex < assemblies.Length; assemblyIndex++)
        {
            Assembly assembly = assemblies[assemblyIndex];
            if (
                assembly.FullName.StartsWith("System.")
                || assembly.FullName.Equals("System")
                || assembly.FullName.StartsWith("Microsoft.")
                || assembly.FullName.StartsWith("GodotSharp")
                || assembly.FullName.StartsWith("GodotTools")
                || assembly.FullName.StartsWith("GodotPlugins")
                || assembly.FullName.StartsWith("JetBrains")
                || assembly.FullName.Equals("netstandard")
            )
            {
                continue;
            }

            GD.Print($"Loading tests from {assembly.FullName}");
            LoadFunctionsFromAssembly(tests, assembly);
        }

        return tests;
    }

    private static void LoadFunctionsFromAssembly(List<TestFunction> tests, Assembly assembly)
    {
        ReadOnlySpan<Type> types = assembly.GetTypes();
        for (int typeIndex = 0; typeIndex < types.Length; typeIndex++)
        {
            LoadFunctionsFromType(tests, types[typeIndex]);
        }
    }

    private static void LoadFunctionsFromType(List<TestFunction> tests, Type type)
    {
        ReadOnlySpan<MethodInfo> methods = type.GetMethods();
        foreach (var method in methods)
        {
            var attribute = method.GetCustomAttributes(typeof(CSTestFunctionAttribute), false);

            if (attribute.Length > 0)
            {
                if (method.ReturnType != typeof(Result))
                {
                    GD.PushError(
                        $"Method {method.Name} in {method.DeclaringType} does not return Result. Skipping it..."
                    );
                    continue;
                }
                else if (!method.IsStatic)
                {
                    GD.PushError(
                        $"Method {method.Name} in {method.DeclaringType} is not static. Skipping it..."
                    );
                    continue;
                }

                tests.Add(
                    new TestFunction()
                    {
                        Name = method.Name,
                        Type = method.DeclaringType,
                        Method = method,
                    }
                );
            }
        }
    }
}

/// <summary>
/// Attribute for marking a method as a test function.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class CSTestFunctionAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CSTestFunctionAttribute"/> class.
    /// </summary>
    public CSTestFunctionAttribute() { }
}
#endif
