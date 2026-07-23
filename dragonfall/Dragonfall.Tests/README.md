# Dragonfall Unit Tests

Comprehensive test suite for the Dragonfall dungeon generation system using GdUnit4.

## Overview

This test project provides **230+ unit tests** covering all core dungeon generation components:

- **BSPNode Tests** (~50 tests) - Binary space partitioning tree structure
- **DungeonRoom Tests** (~60 tests) - Room data and spatial queries
- **Corridor Tests** (~18 tests) - Corridor path management
- **DungeonGenerator Tests** (~85 tests) - Main generation algorithm
- **DungeonTheme Tests** (~30 tests) - Visual configuration

## Test Framework

**GdUnit4** - Godot-native testing framework with full runtime support

### Why GdUnit4?

- Full Godot API access (no mocking needed for Godot types)
- Realistic test environment with scene tree
- Can test visual/rendering aspects
- Tests run inside Godot editor

## Installation & Setup

### 1. Install GdUnit4 Addon

**Option A: Via Godot Asset Library (Recommended)**

1. Open your Godot project
2. Go to `AssetLib` tab in Godot Editor
3. Search for "GdUnit4"
4. Click "Download" and then "Install"
5. Enable the plugin in `Project > Project Settings > Plugins`

**Option B: Manual Installation**

1. Download GdUnit4 from: https://github.com/MikeSchulze/gdUnit4
2. Extract to `addons/gdUnit4/` in your project root
3. Enable the plugin in `Project > Project Settings > Plugins`

### 2. Restore NuGet Packages

```bash
cd Dragonfall.Tests
dotnet restore
```

### 3. Build the Test Project

```bash
dotnet build Dragonfall.Tests.csproj
```

## Running Tests

### In Godot Editor

1. Open your Godot project
2. Look for the "GdUnit4" panel (usually bottom panel)
3. Click the "Scan" button to discover tests
4. Select test files to run
5. Click "Run Tests"

### From Command Line

```bash
# Run all tests
godot --path . --headless --run-gdunit4-tests

# Run specific test suite
godot --path . --headless --run-gdunit4-tests --test-suite=BSPNodeTests

# With verbose output
godot --path . --headless --run-gdunit4-tests --verbose
```

### From Visual Studio / Rider

GdUnit4 tests can also be run from your C# IDE:

1. Open `Dragonfall.sln`
2. Navigate to test files in `Dragonfall.Tests/`
3. Use the test runner interface in your IDE

## Project Structure

```
Dragonfall.Tests/
├── Dungeon/                      # Component tests
│   ├── BSPNodeTests.cs          # BSP tree structure tests
│   ├── DungeonRoomTests.cs      # Room data tests
│   ├── CorridorTests.cs         # Corridor tests
│   ├── DungeonGeneratorTests.cs # Main generation tests
│   └── DungeonThemeTests.cs     # Theme configuration tests
├── TestHelpers/                  # Test utilities
│   ├── GodotTestHelper.cs       # Factory methods
│   ├── DungeonAssertions.cs     # Custom assertions
│   ├── DungeonTestBuilder.cs    # Fluent builder
│   └── DungeonTestData.cs       # Test data/snapshots
├── Fixtures/                     # Test data
│   └── KnownSeeds.cs            # Predefined seeds
└── Dragonfall.Tests.csproj      # Project configuration
```

## Test Coverage Goals

| Component | Target Coverage | Status |
|-----------|----------------|---------|
| BSPNode | 100% | ✅ Implemented |
| DungeonRoom | 100% | ✅ Implemented |
| Corridor | 100% | ✅ Implemented |
| DungeonGenerator | 90%+ | ✅ Implemented |
| DungeonTheme | 100% | ✅ Implemented |
| DungeonMeshBuilder | 80%+ | ⏳ Pending |
| Integration Tests | Critical paths | ⏳ Pending |

## Writing New Tests

### Basic Test Structure

```csharp
using GdUnit4;
using static GdUnit4.Assertions;

namespace Dragonfall.Tests.Dungeon;

[TestSuite]
public class MyComponentTests
{
    [TestCase]
    public void MyTest_Condition_ExpectedResult()
    {
        // Arrange
        var component = CreateTestComponent();

        // Act
        var result = component.DoSomething();

        // Assert
        AssertThat(result).IsEqual(expectedValue);
    }
}
```

### Using Test Helpers

```csharp
// Create test data easily
var room = GodotTestHelper.CreateTestRoom(x: 10, y: 20, width: 30, height: 40);
var node = GodotTestHelper.CreateTestBSPNode();

// Use fluent builder
var generator = new DungeonTestBuilder()
    .WithSize(100, 100)
    .WithSeed(12345)
    .WithMaxDepth(4)
    .BuildAndGenerate();

// Custom assertions
generator.ShouldBeValidDungeon();
rooms.ShouldBeConnected(corridors);
rooms.ShouldNotOverlap();
```

### Using Known Seeds

```csharp
// Use predefined seeds for regression testing
gen.GenerateDungeon(KnownSeeds.SimpleDungeon);
gen.GenerateDungeon(KnownSeeds.ComplexDungeon);

// Get expected results
var snapshot = DungeonTestData.GetExpectedSnapshot(KnownSeeds.SimpleDungeon);
```

## Test Categories

### Unit Tests
Test individual components in isolation:
- Construction and initialization
- Property calculations
- Spatial queries (Contains, Intersects)
- Edge cases and boundary conditions

### Determinism Tests
Verify that generation is reproducible:
- Same seed produces identical dungeons
- Different seeds produce different dungeons
- Known seeds match expected snapshots

### Integration Tests *(Pending)*
Test complete generation pipeline:
- End-to-end dungeon generation
- Mesh building integration
- Room connectivity validation

## Continuous Testing

### Watch Mode

Use `dotnet watch` for automatic test runs during development:

```bash
cd Dragonfall.Tests
dotnet watch test
```

### Pre-commit Hook

Add to `.git/hooks/pre-commit`:

```bash
#!/bin/sh
echo "Running tests..."
godot --path . --headless --run-gdunit4-tests
if [ $? -ne 0 ]; then
    echo "Tests failed! Commit aborted."
    exit 1
fi
```

## Troubleshooting

### Tests Not Showing Up

1. Ensure GdUnit4 plugin is enabled
2. Click "Scan" in GdUnit4 panel
3. Rebuild the solution: `dotnet build`

### "Type not found" Errors

1. Check that `Dragonfall.csproj` is referenced in `Dragonfall.Tests.csproj`
2. Rebuild both projects
3. Restart Godot editor

### GdUnit4 Package Issues

```bash
# Clear NuGet cache
dotnet nuget locals all --clear

# Restore packages
dotnet restore Dragonfall.Tests.csproj
```

### Tests Timing Out

- Increase timeout in GdUnit4 settings
- Check for infinite loops in generation
- Verify performance tests have reasonable thresholds

## Contributing

When adding new features to dungeon generation:

1. Write tests first (TDD approach)
2. Ensure existing tests still pass
3. Aim for 80%+ code coverage on new code
4. Add integration tests for major features
5. Document complex test scenarios

## Resources

- [GdUnit4 Documentation](https://github.com/MikeSchulze/gdUnit4)
- [GdUnit4 API Reference](https://mikeschulze.github.io/gdUnit4/)
- [Godot C# Documentation](https://docs.godotengine.org/en/stable/tutorials/scripting/c_sharp/)

## License

Same license as main Dragonfall project.
