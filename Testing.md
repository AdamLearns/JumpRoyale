# Testing

-   [Testing](#testing)
    -   [Requirements](#requirements)
    -   [First Run](#first-run)
    -   [Running Tests](#running-tests)
    -   [Creating tests](#creating-tests)
        -   [Test Template](#test-template)

---

## Requirements

The game is locked at `.Net 6.0`. You can download it from dotnet downloads [here](https://dotnet.microsoft.com/en-us/download/dotnet/6.0).

---

## First Run

-   Run `dotnet restore`
-   **Rebuild** the solution (`.NET: Rebuild` task in VSCode, in Visual Studio: `Build -> Rebuild`)
-   If developing with VSCode, run `Reload Window` task from Command Palette, wait for projects to load, then run `.NET: Rebuild` task again. The test runner should discover tests and they should be ready to go

---

## Running Tests

Test Runner is immediately available in Visual Studio, in VSCode, you need `C# Dev Kit` extension, available in the `Solution Explorer`

-   Visual Studio:
    -   Run immediately through `CTRL + R, A` or `dotnet test`. This will also open the Test Explorer (also available through `CTRL + E, T`)
    -   Through CLI: `dotnet test`
-   Visual Studio Code:
    -   In the Command Palette, open `Test Explorer`, run tests through UI
    -   Through CLI: `dotnet test`

> [!note]
> In VSCode, sometimes a manual `.NET: Rebuild` is required if a test fails, I don't know why ツ

---

## Creating tests

General guideline is to match namespaces inside classes with the directory structure, ignoring the project name - it will be automatically used as root name:

```plaintext
// class name as same as file name
// `InventoryTests.cs`
[TestFixture]
public class InventoryTests {}

// file resides under:
Tests/Game/Player/Inventory

// Namespace would be:
namespace Game.Player.Inventory;
```

Namespaces are purely for ordering purposes inside the test runner, they give no benefits whatsoever. Example structure in the current project:

![img](https://github.com/DarkStoorM/JumpRoyale/assets/7021295/1ecc4540-37cf-4c02-b0bb-e9811b409685)

```plaintext
Tests                              // <- project name
    Commands                       // <- namespace Commands;
        ChatCommandDetectionTests  // <- \ class name
        ChatCommandParserTests     // <-   class name
        JumpCommandTests           // <-   class name
    Utils                          // <- namespace Utils;
        UtilsTests                 // <- \ class name
    Utils.Extensions               // <- namespace Utils.Extensions;
        ExtensionsTests            // <- \ class name
```

### Test Template

```csharp
namespace YourNameSpaceHere;

[TestFixture]
public class ExampleTests
{
    [Test]
    public void TestsSomething()
    {
        Assert.Pass();
    }
}
```
