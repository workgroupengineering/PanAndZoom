// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Linq;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Xunit;

namespace Avalonia.Controls.PanAndZoom.UnitTests;

/// <summary>
/// Tests for ZoomBorder discrete zoom levels functionality.
/// </summary>
public class ZoomBorderDiscreteZoomLevelsTests
{
    [AvaloniaFact]
    public void EnableDiscreteZoomLevels_DefaultValue_IsFalse()
    {
        // Arrange & Act
        var zoomBorder = new ZoomBorder();

        // Assert
        Assert.False(zoomBorder.EnableDiscreteZoomLevels);
    }

    [AvaloniaFact]
    public void DiscreteZoomLevels_DefaultValue_HasStandardLevels()
    {
        // Arrange & Act
        var zoomBorder = new ZoomBorder();

        // Assert
        Assert.NotNull(zoomBorder.DiscreteZoomLevels);
        Assert.Contains(1.0, zoomBorder.DiscreteZoomLevels!);
        Assert.Contains(2.0, zoomBorder.DiscreteZoomLevels!);
    }

    [AvaloniaFact]
    public void DiscreteZoomLevels_CanBeSetToCustomLevels()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();
        var customLevels = new[] { 0.5, 1.0, 1.5, 2.0, 3.0 };

        // Act
        zoomBorder.DiscreteZoomLevels = customLevels;

        // Assert
        Assert.Equal(customLevels, zoomBorder.DiscreteZoomLevels);
    }

    [AvaloniaFact]
    public void GetNextDiscreteZoomLevel_ReturnsNextLevel()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300,
            EnableDiscreteZoomLevels = true,
            DiscreteZoomLevels = new[] { 0.5, 1.0, 2.0, 4.0 },
            Stretch = StretchMode.None
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

        // Verify current zoom and get next level
        var currentZoom = zoomBorder.ZoomX;

        // Act
        var nextLevel = zoomBorder.GetNextDiscreteZoomLevel();

        // Assert - next level should be the first level > currentZoom
        var sorted = new[] { 0.5, 1.0, 2.0, 4.0 }.OrderBy(z => z).ToArray();
        var expected = sorted.FirstOrDefault(z => z > currentZoom);
        if (expected == 0) expected = sorted.Last();

        Assert.Equal(expected, nextLevel);
    }

    [AvaloniaFact]
    public void GetPreviousDiscreteZoomLevel_ReturnsPreviousLevel()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300,
            EnableDiscreteZoomLevels = true,
            DiscreteZoomLevels = new[] { 0.5, 1.0, 2.0, 4.0 },
            Stretch = StretchMode.None
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
        var currentZoom = zoomBorder.ZoomX;

        // Act
        var previousLevel = zoomBorder.GetPreviousDiscreteZoomLevel();

        // Assert - previous level should be the first level < currentZoom
        var sorted = new[] { 0.5, 1.0, 2.0, 4.0 }.OrderByDescending(z => z).ToArray();
        var expected = sorted.FirstOrDefault(z => z < currentZoom);
        if (expected == 0) expected = sorted.Last();

        Assert.Equal(expected, previousLevel);
    }

    [AvaloniaFact]
    public void ZoomToLevel_ZoomsToSpecificLevel()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300,
            EnableDiscreteZoomLevels = true,
            DiscreteZoomLevels = new[] { 0.5, 1.0, 2.0, 4.0 }
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
        zoomBorder.ZoomToLevel(2.0, 100, 75);

        // Assert
        Assert.Equal(2.0, zoomBorder.ZoomX, 2);
    }
}
