// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Xunit;

namespace Avalonia.Controls.PanAndZoom.UnitTests;

/// <summary>
/// Tests for ZoomBorder accessibility functionality.
/// </summary>
public class ZoomBorderAccessibilityTests
{
    [AvaloniaFact]
    public void ZoomLevelDescription_DefaultValue_IsEmpty()
    {
        // Arrange & Act
        var zoomBorder = new ZoomBorder();

        // Assert
        Assert.Equal(string.Empty, zoomBorder.ZoomLevelDescription);
    }

    [AvaloniaFact]
    public void PanPositionDescription_DefaultValue_IsEmpty()
    {
        // Arrange & Act
        var zoomBorder = new ZoomBorder();

        // Assert
        Assert.Equal(string.Empty, zoomBorder.PanPositionDescription);
    }

    [AvaloniaFact]
    public void UseHighContrastMode_DefaultValue_IsFalse()
    {
        // Arrange & Act
        var zoomBorder = new ZoomBorder();

        // Assert
        Assert.False(zoomBorder.UseHighContrastMode);
    }

    [AvaloniaFact]
    public void ZoomLevelDescription_CanBeSet()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();

        // Act
        zoomBorder.ZoomLevelDescription = "Zoom level: 100%";

        // Assert
        Assert.Equal("Zoom level: 100%", zoomBorder.ZoomLevelDescription);
    }

    [AvaloniaFact]
    public void PanPositionDescription_CanBeSet()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();

        // Act
        zoomBorder.PanPositionDescription = "Pan position: X=0, Y=0";

        // Assert
        Assert.Equal("Pan position: X=0, Y=0", zoomBorder.PanPositionDescription);
    }

    [AvaloniaFact]
    public void UseHighContrastMode_CanBeSet()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();

        // Act
        zoomBorder.UseHighContrastMode = true;

        // Assert
        Assert.True(zoomBorder.UseHighContrastMode);
    }

    [AvaloniaFact]
    public void UpdateAccessibilityDescriptions_UpdatesZoomLevelDescription()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300
        };

        var childElement = new Border
        {
            Width = 200,
            Height = 150,
            Background = Brushes.Red
        };

        zoomBorder.Child = childElement;
        var window = new Window { Content = zoomBorder };
        window.Show();

        zoomBorder.Zoom(2.0, 100, 75);

        // Act
        zoomBorder.UpdateAccessibilityDescriptions();

        // Assert
        Assert.NotEmpty(zoomBorder.ZoomLevelDescription);
        Assert.Contains("Zoom level", zoomBorder.ZoomLevelDescription);
        Assert.Contains("200", zoomBorder.ZoomLevelDescription); // 2.0 * 100 = 200%
    }

    [AvaloniaFact]
    public void UpdateAccessibilityDescriptions_UpdatesPanPositionDescription()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300
        };

        var childElement = new Border
        {
            Width = 200,
            Height = 150,
            Background = Brushes.Red
        };

        zoomBorder.Child = childElement;
        var window = new Window { Content = zoomBorder };
        window.Show();

        // Act
        zoomBorder.UpdateAccessibilityDescriptions();

        // Assert
        Assert.NotEmpty(zoomBorder.PanPositionDescription);
        Assert.Contains("Pan position", zoomBorder.PanPositionDescription);
    }

    [AvaloniaFact]
    public void GetAccessibilityDescription_ReturnsCombinedDescription()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300
        };

        var childElement = new Border
        {
            Width = 200,
            Height = 150,
            Background = Brushes.Red
        };

        zoomBorder.Child = childElement;
        var window = new Window { Content = zoomBorder };
        window.Show();

        // Act
        var description = zoomBorder.GetAccessibilityDescription();

        // Assert
        Assert.NotEmpty(description);
        Assert.Contains("Zoom level", description);
        Assert.Contains("Pan position", description);
    }
}
