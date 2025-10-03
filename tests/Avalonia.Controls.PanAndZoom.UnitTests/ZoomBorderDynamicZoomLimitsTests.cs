// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Xunit;

namespace Avalonia.Controls.PanAndZoom.UnitTests;

/// <summary>
/// Tests for ZoomBorder dynamic zoom limits functionality.
/// </summary>
public class ZoomBorderDynamicZoomLimitsTests
{
    [AvaloniaFact]
    public void AutoCalculateMinZoom_DefaultValue_IsFalse()
    {
        // Arrange & Act
        var zoomBorder = new ZoomBorder();

        // Assert
        Assert.False(zoomBorder.AutoCalculateMinZoom);
    }

    [AvaloniaFact]
    public void AutoCalculateMaxZoom_DefaultValue_IsFalse()
    {
        // Arrange & Act
        var zoomBorder = new ZoomBorder();

        // Assert
        Assert.False(zoomBorder.AutoCalculateMaxZoom);
    }

    [AvaloniaFact]
    public void MaxZoomPixelSize_DefaultValue_Is4()
    {
        // Arrange & Act
        var zoomBorder = new ZoomBorder();

        // Assert
        Assert.Equal(4.0, zoomBorder.MaxZoomPixelSize);
    }

    [AvaloniaFact]
    public void AutoCalculateMinZoom_CanBeSet()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();

        // Act
        zoomBorder.AutoCalculateMinZoom = true;

        // Assert
        Assert.True(zoomBorder.AutoCalculateMinZoom);
    }

    [AvaloniaFact]
    public void AutoCalculateMaxZoom_CanBeSet()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();

        // Act
        zoomBorder.AutoCalculateMaxZoom = true;

        // Assert
        Assert.True(zoomBorder.AutoCalculateMaxZoom);
    }

    [AvaloniaFact]
    public void MaxZoomPixelSize_CanBeSet()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();

        // Act
        zoomBorder.MaxZoomPixelSize = 8.0;

        // Assert
        Assert.Equal(8.0, zoomBorder.MaxZoomPixelSize);
    }

    [AvaloniaFact]
    public void AutoCalculateMinZoom_PreventsZoomOutBeyondContentFit()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300,
            AutoCalculateMinZoom = true,
            Stretch = StretchMode.None
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

        // Act - Try to zoom out very far
        for (int i = 0; i < 20; i++)
        {
            zoomBorder.ZoomOut();
        }

        // Assert - Should not zoom below the minimum (content fit)
        Assert.True(zoomBorder.ZoomX >= 0.4, "Zoom should be limited by auto-calculated minimum");
    }

    [AvaloniaFact]
    public void AutoCalculateMaxZoom_PreventsZoomInBeyondPixelSize()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300,
            AutoCalculateMaxZoom = true,
            MaxZoomPixelSize = 2.0,
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

        // Act - Try to zoom in very far
        for (int i = 0; i < 20; i++)
        {
            zoomBorder.ZoomIn();
        }

        // Assert - Should not zoom above the maximum pixel size
        Assert.True(zoomBorder.ZoomX <= 2.1, "Zoom should be limited by MaxZoomPixelSize");
    }
}
