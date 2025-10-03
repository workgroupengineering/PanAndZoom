// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Xunit;

namespace Avalonia.Controls.PanAndZoom.UnitTests;

/// <summary>
/// Tests for ZoomBorder grid and snap functionality.
/// </summary>
public class ZoomBorderGridAndSnapTests
{
    [AvaloniaFact]
    public void ShowGrid_DefaultValue_IsFalse()
    {
        // Arrange & Act
        var zoomBorder = new ZoomBorder();

        // Assert
        Assert.False(zoomBorder.ShowGrid);
    }

    [AvaloniaFact]
    public void EnableSnapToGrid_DefaultValue_IsFalse()
    {
        // Arrange & Act
        var zoomBorder = new ZoomBorder();

        // Assert
        Assert.False(zoomBorder.EnableSnapToGrid);
    }

    [AvaloniaFact]
    public void GridSize_DefaultValue_Is50()
    {
        // Arrange & Act
        var zoomBorder = new ZoomBorder();

        // Assert
        Assert.Equal(50.0, zoomBorder.GridSize);
    }

    [AvaloniaFact]
    public void GridThickness_DefaultValue_Is1()
    {
        // Arrange & Act
        var zoomBorder = new ZoomBorder();

        // Assert
        Assert.Equal(1.0, zoomBorder.GridThickness);
    }

    [AvaloniaFact]
    public void GridOpacity_DefaultValue_Is03()
    {
        // Arrange & Act
        var zoomBorder = new ZoomBorder();

        // Assert
        Assert.Equal(0.3, zoomBorder.GridOpacity);
    }

    [AvaloniaFact]
    public void MajorGridInterval_DefaultValue_Is5()
    {
        // Arrange & Act
        var zoomBorder = new ZoomBorder();

        // Assert
        Assert.Equal(5, zoomBorder.MajorGridInterval);
    }

    [AvaloniaFact]
    public void MajorGridThickness_DefaultValue_Is2()
    {
        // Arrange & Act
        var zoomBorder = new ZoomBorder();

        // Assert
        Assert.Equal(2.0, zoomBorder.MajorGridThickness);
    }

    [AvaloniaFact]
    public void ShowGrid_CanBeSet()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();

        // Act
        zoomBorder.ShowGrid = true;

        // Assert
        Assert.True(zoomBorder.ShowGrid);
    }

    [AvaloniaFact]
    public void EnableSnapToGrid_CanBeSet()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();

        // Act
        zoomBorder.EnableSnapToGrid = true;

        // Assert
        Assert.True(zoomBorder.EnableSnapToGrid);
    }

    [AvaloniaFact]
    public void GridSize_CanBeSet()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();

        // Act
        zoomBorder.GridSize = 100.0;

        // Assert
        Assert.Equal(100.0, zoomBorder.GridSize);
    }

    [AvaloniaFact]
    public void GridBrush_CanBeSet()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();
        var brush = new SolidColorBrush(Colors.Blue);

        // Act
        zoomBorder.GridBrush = brush;

        // Assert
        Assert.Equal(brush, zoomBorder.GridBrush);
    }

    [AvaloniaFact]
    public void SnapToGrid_DoubleValue_SnapsToNearestGridPoint()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            EnableSnapToGrid = true,
            GridSize = 50.0
        };

        // Act
        var snapped1 = zoomBorder.SnapToGrid(123.0);
        var snapped2 = zoomBorder.SnapToGrid(176.0);

        // Assert
        Assert.Equal(100.0, snapped1);
        Assert.Equal(200.0, snapped2);
    }

    [AvaloniaFact]
    public void SnapToGrid_DoubleValue_ReturnsUnchangedWhenDisabled()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            EnableSnapToGrid = false,
            GridSize = 50.0
        };

        // Act
        var snapped = zoomBorder.SnapToGrid(123.0);

        // Assert
        Assert.Equal(123.0, snapped);
    }

    [AvaloniaFact]
    public void SnapToGrid_Point_SnapsToNearestGridPoint()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            EnableSnapToGrid = true,
            GridSize = 50.0
        };

        var point = new Point(123.0, 176.0);

        // Act
        var snapped = zoomBorder.SnapToGrid(point);

        // Assert
        Assert.Equal(100.0, snapped.X);
        Assert.Equal(200.0, snapped.Y);
    }

    [AvaloniaFact]
    public void SnapToGrid_Rect_SnapsToNearestGridPoints()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            EnableSnapToGrid = true,
            GridSize = 50.0
        };

        var rect = new Rect(123.0, 176.0, 100.0, 80.0);

        // Act
        var snapped = zoomBorder.SnapToGrid(rect);

        // Assert
        Assert.Equal(100.0, snapped.X);
        Assert.Equal(200.0, snapped.Y);
        // Width and height are derived from snapped corners
        Assert.True(snapped.Width > 0);
        Assert.True(snapped.Height > 0);
    }

    [AvaloniaFact]
    public void SnapToGrid_WithZeroGridSize_ReturnsOriginalValue()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            EnableSnapToGrid = true,
            GridSize = 0.0
        };

        // Act
        var snapped = zoomBorder.SnapToGrid(123.0);

        // Assert
        Assert.Equal(123.0, snapped);
    }

    [AvaloniaFact]
    public void SnapToGrid_WithNegativeGridSize_ReturnsOriginalValue()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            EnableSnapToGrid = true,
            GridSize = -50.0
        };

        // Act
        var snapped = zoomBorder.SnapToGrid(123.0);

        // Assert
        Assert.Equal(123.0, snapped);
    }
}
