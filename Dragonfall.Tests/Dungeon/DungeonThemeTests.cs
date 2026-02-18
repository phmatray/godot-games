using GdUnit4;
using Godot;
using Dragonfall.Dungeon;
using static GdUnit4.Assertions;

namespace Dragonfall.Tests.Dungeon;

/// <summary>
/// Tests for DungeonTheme class.
/// Tests cover factory methods, property initialization, and validation.
/// </summary>
[TestSuite]
public class DungeonThemeTests
{
	#region Factory Tests

	[TestCase]
	public void CreateDefault_ReturnsValidTheme()
	{
		// Act
		var theme = DungeonTheme.CreateDefault();

		// Assert
		AssertThat(theme).IsNotNull();
		AssertThat(theme.WallHeight).IsGreater(0);
		AssertThat(theme.WallThickness).IsGreater(0);
		AssertThat(theme.CorridorWidth).IsGreater(0);
	}

	[TestCase]
	public void CreateDefault_HasExpectedDefaults()
	{
		// Act
		var theme = DungeonTheme.CreateDefault();

		// Assert
		AssertThat(theme.WallHeight).IsEqual(3.0f);
		AssertThat(theme.WallThickness).IsEqual(0.2f);
		AssertThat(theme.CorridorWidth).IsEqual(2.0f);
	}

	[TestCase]
	public void CreateDefault_HasExpectedColors()
	{
		// Act
		var theme = DungeonTheme.CreateDefault();

		// Assert
		AssertThat(theme.FloorColor).IsEqual(new Color(0.3f, 0.3f, 0.3f));
		AssertThat(theme.WallColor).IsEqual(new Color(0.5f, 0.5f, 0.5f));
		AssertThat(theme.CeilingColor).IsEqual(new Color(0.2f, 0.2f, 0.2f));
		AssertThat(theme.CorridorColor).IsEqual(new Color(0.25f, 0.25f, 0.25f));
	}

	[TestCase]
	public void CreateCryptTheme_ReturnsValidTheme()
	{
		// Act
		var theme = DungeonTheme.CreateCryptTheme();

		// Assert
		AssertThat(theme).IsNotNull();
		AssertThat(theme.WallHeight).IsGreater(0);
		AssertThat(theme.WallThickness).IsGreater(0);
		AssertThat(theme.CorridorWidth).IsGreater(0);
	}

	[TestCase]
	public void CreateCryptTheme_DifferentFromDefault()
	{
		// Act
		var defaultTheme = DungeonTheme.CreateDefault();
		var cryptTheme = DungeonTheme.CreateCryptTheme();

		// Assert
		AssertThat(cryptTheme.WallHeight).IsNotEqual(defaultTheme.WallHeight);
		AssertThat(cryptTheme.WallThickness).IsNotEqual(defaultTheme.WallThickness);
		AssertThat(cryptTheme.CorridorWidth).IsNotEqual(defaultTheme.CorridorWidth);
	}

	[TestCase]
	public void CreateCryptTheme_HasExpectedDimensions()
	{
		// Act
		var theme = DungeonTheme.CreateCryptTheme();

		// Assert
		AssertThat(theme.WallHeight).IsEqual(4.0f);
		AssertThat(theme.WallThickness).IsEqual(0.3f);
		AssertThat(theme.CorridorWidth).IsEqual(2.5f);
	}

	[TestCase]
	public void CreateCryptTheme_HasExpectedColors()
	{
		// Act
		var theme = DungeonTheme.CreateCryptTheme();

		// Assert
		AssertThat(theme.FloorColor).IsEqual(new Color(0.2f, 0.2f, 0.25f));
		AssertThat(theme.WallColor).IsEqual(new Color(0.35f, 0.35f, 0.4f));
		AssertThat(theme.CeilingColor).IsEqual(new Color(0.15f, 0.15f, 0.2f));
		AssertThat(theme.CorridorColor).IsEqual(new Color(0.18f, 0.18f, 0.22f));
	}

	[TestCase]
	public void Themes_HaveDistinctColors()
	{
		// Arrange
		var defaultTheme = DungeonTheme.CreateDefault();
		var cryptTheme = DungeonTheme.CreateCryptTheme();

		// Assert
		AssertThat(defaultTheme.FloorColor).IsNotEqual(cryptTheme.FloorColor);
		AssertThat(defaultTheme.WallColor).IsNotEqual(cryptTheme.WallColor);
		AssertThat(defaultTheme.CeilingColor).IsNotEqual(cryptTheme.CeilingColor);
		AssertThat(defaultTheme.CorridorColor).IsNotEqual(cryptTheme.CorridorColor);
	}

	[TestCase]
	public void Themes_HaveDistinctDimensions()
	{
		// Arrange
		var defaultTheme = DungeonTheme.CreateDefault();
		var cryptTheme = DungeonTheme.CreateCryptTheme();

		// Assert - Crypt should be taller and thicker
		AssertThat(cryptTheme.WallHeight).IsGreater(defaultTheme.WallHeight);
		AssertThat(cryptTheme.WallThickness).IsGreater(defaultTheme.WallThickness);
		AssertThat(cryptTheme.CorridorWidth).IsGreater(defaultTheme.CorridorWidth);
	}

	#endregion

	#region Property Tests

	[TestCase]
	public void Constructor_InitializesWithDefaults()
	{
		// Act
		var theme = new DungeonTheme();

		// Assert
		AssertThat(theme.WallHeight).IsEqual(3.0f);
		AssertThat(theme.WallThickness).IsEqual(0.2f);
		AssertThat(theme.CorridorWidth).IsEqual(2.0f);
	}

	[TestCase]
	public void WallHeight_CanBeSet()
	{
		// Arrange
		var theme = new DungeonTheme();
		var newHeight = 5.5f;

		// Act
		theme.WallHeight = newHeight;

		// Assert
		AssertThat(theme.WallHeight).IsEqual(newHeight);
	}

	[TestCase]
	public void WallThickness_CanBeSet()
	{
		// Arrange
		var theme = new DungeonTheme();
		var newThickness = 0.5f;

		// Act
		theme.WallThickness = newThickness;

		// Assert
		AssertThat(theme.WallThickness).IsEqual(newThickness);
	}

	[TestCase]
	public void CorridorWidth_CanBeSet()
	{
		// Arrange
		var theme = new DungeonTheme();
		var newWidth = 3.5f;

		// Act
		theme.CorridorWidth = newWidth;

		// Assert
		AssertThat(theme.CorridorWidth).IsEqual(newWidth);
	}

	[TestCase]
	public void FloorColor_CanBeSet()
	{
		// Arrange
		var theme = new DungeonTheme();
		var newColor = new Color(1.0f, 0.0f, 0.0f);

		// Act
		theme.FloorColor = newColor;

		// Assert
		AssertThat(theme.FloorColor).IsEqual(newColor);
	}

	[TestCase]
	public void WallColor_CanBeSet()
	{
		// Arrange
		var theme = new DungeonTheme();
		var newColor = new Color(0.0f, 1.0f, 0.0f);

		// Act
		theme.WallColor = newColor;

		// Assert
		AssertThat(theme.WallColor).IsEqual(newColor);
	}

	[TestCase]
	public void CeilingColor_CanBeSet()
	{
		// Arrange
		var theme = new DungeonTheme();
		var newColor = new Color(0.0f, 0.0f, 1.0f);

		// Act
		theme.CeilingColor = newColor;

		// Assert
		AssertThat(theme.CeilingColor).IsEqual(newColor);
	}

	[TestCase]
	public void CorridorColor_CanBeSet()
	{
		// Arrange
		var theme = new DungeonTheme();
		var newColor = new Color(1.0f, 1.0f, 0.0f);

		// Act
		theme.CorridorColor = newColor;

		// Assert
		AssertThat(theme.CorridorColor).IsEqual(newColor);
	}

	[TestCase]
	public void AllProperties_CanBeSetIndependently()
	{
		// Arrange
		var theme = new DungeonTheme();

		// Act
		theme.WallHeight = 10.0f;
		theme.WallThickness = 1.0f;
		theme.CorridorWidth = 5.0f;
		theme.FloorColor = new Color(1, 0, 0);
		theme.WallColor = new Color(0, 1, 0);
		theme.CeilingColor = new Color(0, 0, 1);
		theme.CorridorColor = new Color(1, 1, 0);

		// Assert
		AssertThat(theme.WallHeight).IsEqual(10.0f);
		AssertThat(theme.WallThickness).IsEqual(1.0f);
		AssertThat(theme.CorridorWidth).IsEqual(5.0f);
		AssertThat(theme.FloorColor).IsEqual(new Color(1, 0, 0));
		AssertThat(theme.WallColor).IsEqual(new Color(0, 1, 0));
		AssertThat(theme.CeilingColor).IsEqual(new Color(0, 0, 1));
		AssertThat(theme.CorridorColor).IsEqual(new Color(1, 1, 0));
	}

	#endregion

	#region Validation Tests

	[TestCase]
	public void WallHeight_AcceptsPositiveValues()
	{
		// Arrange
		var theme = new DungeonTheme();

		// Act & Assert
		theme.WallHeight = 0.1f;
		AssertThat(theme.WallHeight).IsEqual(0.1f);

		theme.WallHeight = 1.0f;
		AssertThat(theme.WallHeight).IsEqual(1.0f);

		theme.WallHeight = 100.0f;
		AssertThat(theme.WallHeight).IsEqual(100.0f);
	}

	[TestCase]
	public void WallThickness_AcceptsPositiveValues()
	{
		// Arrange
		var theme = new DungeonTheme();

		// Act & Assert
		theme.WallThickness = 0.01f;
		AssertThat(theme.WallThickness).IsEqual(0.01f);

		theme.WallThickness = 0.5f;
		AssertThat(theme.WallThickness).IsEqual(0.5f);

		theme.WallThickness = 10.0f;
		AssertThat(theme.WallThickness).IsEqual(10.0f);
	}

	[TestCase]
	public void CorridorWidth_AcceptsPositiveValues()
	{
		// Arrange
		var theme = new DungeonTheme();

		// Act & Assert
		theme.CorridorWidth = 0.5f;
		AssertThat(theme.CorridorWidth).IsEqual(0.5f);

		theme.CorridorWidth = 5.0f;
		AssertThat(theme.CorridorWidth).IsEqual(5.0f);

		theme.CorridorWidth = 50.0f;
		AssertThat(theme.CorridorWidth).IsEqual(50.0f);
	}

	[TestCase]
	public void Colors_AcceptAnyValidColor()
	{
		// Arrange
		var theme = new DungeonTheme();
		var testColors = new[]
		{
			new Color(0, 0, 0),        // Black
			new Color(1, 1, 1),        // White
			new Color(1, 0, 0),        // Red
			new Color(0, 1, 0),        // Green
			new Color(0, 0, 1),        // Blue
			new Color(0.5f, 0.5f, 0.5f) // Gray
		};

		// Act & Assert
		foreach (var color in testColors)
		{
			theme.FloorColor = color;
			AssertThat(theme.FloorColor).IsEqual(color);

			theme.WallColor = color;
			AssertThat(theme.WallColor).IsEqual(color);

			theme.CeilingColor = color;
			AssertThat(theme.CeilingColor).IsEqual(color);

			theme.CorridorColor = color;
			AssertThat(theme.CorridorColor).IsEqual(color);
		}
	}

	[TestCase]
	public void Dimensions_VerySmallValues_Handled()
	{
		// Arrange
		var theme = new DungeonTheme();

		// Act
		theme.WallHeight = 0.01f;
		theme.WallThickness = 0.01f;
		theme.CorridorWidth = 0.01f;

		// Assert
		AssertThat(theme.WallHeight).IsEqual(0.01f);
		AssertThat(theme.WallThickness).IsEqual(0.01f);
		AssertThat(theme.CorridorWidth).IsEqual(0.01f);
	}

	[TestCase]
	public void Dimensions_VeryLargeValues_Handled()
	{
		// Arrange
		var theme = new DungeonTheme();

		// Act
		theme.WallHeight = 1000.0f;
		theme.WallThickness = 100.0f;
		theme.CorridorWidth = 500.0f;

		// Assert
		AssertThat(theme.WallHeight).IsEqual(1000.0f);
		AssertThat(theme.WallThickness).IsEqual(100.0f);
		AssertThat(theme.CorridorWidth).IsEqual(500.0f);
	}

	#endregion

	#region Edge Cases

	[TestCase]
	public void Theme_CanBeClonedViaPropertyCopy()
	{
		// Arrange
		var original = DungeonTheme.CreateCryptTheme();
		var clone = new DungeonTheme
		{
			WallHeight = original.WallHeight,
			WallThickness = original.WallThickness,
			CorridorWidth = original.CorridorWidth,
			FloorColor = original.FloorColor,
			WallColor = original.WallColor,
			CeilingColor = original.CeilingColor,
			CorridorColor = original.CorridorColor
		};

		// Assert
		AssertThat(clone.WallHeight).IsEqual(original.WallHeight);
		AssertThat(clone.WallThickness).IsEqual(original.WallThickness);
		AssertThat(clone.CorridorWidth).IsEqual(original.CorridorWidth);
		AssertThat(clone.FloorColor).IsEqual(original.FloorColor);
		AssertThat(clone.WallColor).IsEqual(original.WallColor);
		AssertThat(clone.CeilingColor).IsEqual(original.CeilingColor);
		AssertThat(clone.CorridorColor).IsEqual(original.CorridorColor);
	}

	[TestCase]
	public void Theme_PropertiesIndependentBetweenInstances()
	{
		// Arrange
		var theme1 = DungeonTheme.CreateDefault();
		var theme2 = DungeonTheme.CreateDefault();

		// Act
		theme1.WallHeight = 10.0f;
		theme1.FloorColor = new Color(1, 0, 0);

		// Assert
		AssertThat(theme2.WallHeight).IsNotEqual(10.0f);
		AssertThat(theme2.FloorColor).IsNotEqual(new Color(1, 0, 0));
	}

	#endregion
}
