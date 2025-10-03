// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Xunit;

namespace Avalonia.Controls.PanAndZoom.UnitTests;

/// <summary>
/// Tests for ZoomBorder zoom to rectangle functionality.
/// </summary>
public class ZoomBorderZoomToRectangleTests
{
    [AvaloniaFact]
    public void ZoomToRectangle_ZoomsToFitRectangle()
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

        var rect = new Rect(100, 100, 200, 150);

        // Act
        zoomBorder.ZoomToRectangle(rect);

        // Assert - Should zoom to fit the rectangle
        Assert.True(zoomBorder.ZoomX >= 1.0, "Should zoom in to fit rectangle");
    }

    [AvaloniaFact]
    public void ZoomToRectangleExact_ZoomsToSpecificViewportRect()
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

        var contentRect = new Rect(100, 100, 200, 150);
        var viewportRect = new Rect(50, 50, 300, 200);

        // Act
        zoomBorder.ZoomToRectangleExact(contentRect, viewportRect);

        // Assert
        Assert.True(zoomBorder.ZoomX > 1.0);
    }
}
