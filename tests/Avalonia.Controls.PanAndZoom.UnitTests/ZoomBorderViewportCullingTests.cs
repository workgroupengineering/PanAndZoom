// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Xunit;

namespace Avalonia.Controls.PanAndZoom.UnitTests;

/// <summary>
/// Tests for ZoomBorder viewport culling functionality.
/// </summary>
public class ZoomBorderViewportCullingTests
{
    [AvaloniaFact]
    public void IsRectangleVisible_ReturnsTrueForVisibleRectangle()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300
        };

        var childElement = new Border
        {
            Width = 800,
            Height = 600,
            Background = Brushes.Red
        };

        zoomBorder.Child = childElement;
        var window = new Window { Content = zoomBorder };
        window.Show();

        var visibleRect = new Rect(50, 50, 100, 100);

        // Act
        var result = zoomBorder.IsRectangleVisible(visibleRect);

        // Assert
        Assert.True(result);
    }

    [AvaloniaFact]
    public void IsRectangleVisible_ReturnsFalseForNonVisibleRectangle()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300
        };

        var childElement = new Border
        {
            Width = 800,
            Height = 600,
            Background = Brushes.Red
        };

        zoomBorder.Child = childElement;
        var window = new Window { Content = zoomBorder };
        window.Show();

        var nonVisibleRect = new Rect(10000, 10000, 100, 100);

        // Act
        var result = zoomBorder.IsRectangleVisible(nonVisibleRect);

        // Assert
        Assert.False(result);
    }

    [AvaloniaFact]
    public void IsPointVisible_ReturnsTrueForVisiblePoint()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300
        };

        var childElement = new Border
        {
            Width = 800,
            Height = 600,
            Background = Brushes.Red
        };

        zoomBorder.Child = childElement;
        var window = new Window { Content = zoomBorder };
        window.Show();

        var visiblePoint = new Point(100, 100);

        // Act
        var result = zoomBorder.IsPointVisible(visiblePoint);

        // Assert
        Assert.True(result);
    }

    [AvaloniaFact]
    public void GetVisiblePortion_ReturnsIntersectionWithViewport()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300
        };

        var childElement = new Border
        {
            Width = 800,
            Height = 600,
            Background = Brushes.Red
        };

        zoomBorder.Child = childElement;
        var window = new Window { Content = zoomBorder };
        window.Show();

        var rect = new Rect(0, 0, 500, 400);

        // Act
        var visiblePortion = zoomBorder.GetVisiblePortion(rect);

        // Assert
        Assert.True(visiblePortion.Width > 0);
        Assert.True(visiblePortion.Height > 0);
        Assert.True(visiblePortion.Width <= rect.Width);
        Assert.True(visiblePortion.Height <= rect.Height);
    }
}
