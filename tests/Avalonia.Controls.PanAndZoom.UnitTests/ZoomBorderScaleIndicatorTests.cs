// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Xunit;

namespace Avalonia.Controls.PanAndZoom.UnitTests;

/// <summary>
/// Tests for ZoomBorder scale indicator functionality.
/// </summary>
public class ZoomBorderScaleIndicatorTests
{
    [AvaloniaFact]
    public void ShowZoomIndicator_DefaultValue_IsFalse()
    {
        // Arrange & Act
        var zoomBorder = new ZoomBorder();

        // Assert
        Assert.False(zoomBorder.ShowZoomIndicator);
    }

    [AvaloniaFact]
    public void ZoomIndicatorPosition_DefaultValue_IsBottomRight()
    {
        // Arrange & Act
        var zoomBorder = new ZoomBorder();

        // Assert
        Assert.Equal(ZoomIndicatorPosition.BottomRight, zoomBorder.ZoomIndicatorPosition);
    }

    [AvaloniaFact]
    public void ZoomIndicatorFormat_DefaultValue_IsPercentage()
    {
        // Arrange & Act
        var zoomBorder = new ZoomBorder();

        // Assert
        Assert.Equal("{0:P0}", zoomBorder.ZoomIndicatorFormat);
    }

    [AvaloniaFact]
    public void ZoomIndicatorAutoHideDuration_DefaultValue_Is2Seconds()
    {
        // Arrange & Act
        var zoomBorder = new ZoomBorder();

        // Assert
        Assert.Equal(TimeSpan.FromSeconds(2), zoomBorder.ZoomIndicatorAutoHideDuration);
    }

    [AvaloniaFact]
    public void ShowZoomIndicator_CanBeSet()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();

        // Act
        zoomBorder.ShowZoomIndicator = true;

        // Assert
        Assert.True(zoomBorder.ShowZoomIndicator);
    }

    [AvaloniaFact]
    public void ZoomIndicatorPosition_CanBeSet()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();

        // Act
        zoomBorder.ZoomIndicatorPosition = ZoomIndicatorPosition.TopLeft;

        // Assert
        Assert.Equal(ZoomIndicatorPosition.TopLeft, zoomBorder.ZoomIndicatorPosition);
    }

    [AvaloniaFact]
    public void ZoomIndicatorFormat_CanBeSet()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();

        // Act
        zoomBorder.ZoomIndicatorFormat = "{0:F2}x";

        // Assert
        Assert.Equal("{0:F2}x", zoomBorder.ZoomIndicatorFormat);
    }

    [AvaloniaFact]
    public void GetZoomIndicatorText_ReturnsFormattedZoomLevel()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300,
            ZoomIndicatorFormat = "{0:P0}"
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
        var text = zoomBorder.GetZoomIndicatorText();

        // Assert
        Assert.NotNull(text);
        Assert.NotEmpty(text);
    }

    [AvaloniaFact]
    public void GetZoomIndicatorText_UsesCustomFormat()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300,
            ZoomIndicatorFormat = "{0:F1}x"
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
        var text = zoomBorder.GetZoomIndicatorText();

        // Assert
        Assert.Contains("2", text);
        Assert.Contains("x", text);
    }
}
