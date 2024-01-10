// Copyright (c) Spencer (Spycemyster) Chang, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
namespace GdMUT.Components;

using Godot;
using System;
using System.Diagnostics;
using System.Threading;

#if TOOLS
/// <summary>
/// The dock UI that contains all the test results and the controls to run tests.
/// </summary>
[Tool]
public partial class Dock : Control
{
    private const string TEST_RESULT_SCENE = "res://addons/GDMUT/TestResult.tscn";
    private readonly System.Collections.Generic.Dictionary<
        Type,
        System.Collections.Generic.List<TestFunction>
    > _testDictionary = new();
    private readonly System.Collections.Generic.Dictionary<Type, TestResult> _testResultDictionary =
        new();

    [Export]
    private LineEdit _filter;

    [Export]
    private CheckBox _multithreadedEnabled;

    [Export]
    private LineEdit _numThreads;

    [Export]
    private Button _runTests;

    [Export]
    private Button _loadTests;

    [Export]
    private VBoxContainer _testList;

    private System.Collections.Generic.List<TestFunction> _tests = new();

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _EnterTree()
    {
        base._EnterTree();
        _runTests.Pressed += RunTests;
        _loadTests.Pressed += LoadTests;
    }

    private void LoadTests()
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        foreach (Node node in _testList.GetChildren())
        {
            node.QueueFree();
        }

        _tests?.Clear();
        _tests = TestLoader.SearchForAllTests();
        _testDictionary.Clear();
        for (int testIndex = 0; testIndex < _tests.Count; testIndex++)
        {
            TestFunction function = _tests[testIndex];
            if (!function.Name.Contains(_filter.Text))
            {
                continue;
            }

            if (
                _testDictionary.TryGetValue(
                    function.Type,
                    out System.Collections.Generic.List<TestFunction> testList
                )
            )
            {
                testList.Add(function);
            }
            else
            {
                _testDictionary.Add(
                    function.Type,
                    new System.Collections.Generic.List<TestFunction>() { function }
                );
            }
        }

        _testResultDictionary.Clear();
        var testResultScene = GD.Load<PackedScene>(TEST_RESULT_SCENE);
        foreach (Type type in _testDictionary.Keys)
        {
            var functions = _testDictionary[type];
            var testResult = testResultScene.Instantiate<TestResult>();
            testResult.SetTypeName(type.Name);
            _testList.AddChild(testResult);
            _testResultDictionary.Add(type, testResult);
            foreach (TestFunction function in functions)
            {
                testResult.AddMethodResult(function);
            }
        }

        stopwatch.Stop();
        GD.Print($"Loading tests took {stopwatch.ElapsedMilliseconds}ms");
    }

    private void RunTestsInRange(int startIndex, int endIndex)
    {
        for (int testIndex = startIndex; testIndex < endIndex; testIndex++)
        {
            var test = _tests[testIndex];
            GD.Print(test.Name);
            Result testResult;
            try
            {
                testResult = (Result)test.Method.Invoke(null, null);
            }
            catch (Exception e)
            {
                testResult = new Result(false, $"Exception thrown: {e.Message}");
            }

            test.Result = testResult;
        }
    }

    private void RunTests()
    {
        if (_tests.Count == 0)
        {
            GD.Print("No tests loaded");
            return;
        }

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        if (
            _multithreadedEnabled.ButtonPressed
            && int.TryParse(_numThreads.Text, out int numThreads)
            && numThreads > 0
        )
        {
            GD.Print("Run Tests multithreaded");
            Thread[] threads = new Thread[numThreads];
            int testsPerThread =
                (_tests.Count / numThreads) + (_tests.Count % numThreads > 0 ? 1 : 0);
            for (int threadIndex = 0; threadIndex < numThreads; threadIndex++)
            {
                int startIndex = threadIndex * testsPerThread;
                int endIndex = Math.Min((threadIndex + 1) * testsPerThread, _tests.Count);
                threads[threadIndex] = new Thread(() => RunTestsInRange(startIndex, endIndex));
                threads[threadIndex].Start();
            }

            foreach (Thread thread in threads)
            {
                thread.Join();
            }
        }
        else
        {
            GD.Print("Run Tests singlethreaded");
            RunTestsInRange(0, _tests.Count);
        }

        stopwatch.Stop();
        UpdateUIWithResults();
        GD.Print($"Tests took {stopwatch.ElapsedMilliseconds}ms");
    }

    private void UpdateUIWithResults()
    {
        foreach (TestResult result in _testResultDictionary.Values)
        {
            result.UpdateResult();
        }
    }
}
#endif
