// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Headless.XUnit;
using Xunit;

namespace Avalonia.Controls.PanAndZoom.UnitTests;

/// <summary>
/// Tests for double-click zoom functionality.
/// </summary>
public class ZoomBorderDoubleClickZoomTests
{
    [AvaloniaFact]
    public void EnableDoubleClickZoom_DefaultValue_IsTrue()
    {
        // Arrange & Act
        var zoomBorder = new ZoomBorder();

        // Assert
        Assert.True(zoomBorder.EnableDoubleClickZoom);
    }

    [AvaloniaFact]
    public void EnableDoubleClickZoom_CanBeSetToFalse()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();

        // Act
        zoomBorder.EnableDoubleClickZoom = false;

        // Assert
        Assert.False(zoomBorder.EnableDoubleClickZoom);
    }

    [AvaloniaFact]
    public void DoubleClickZoomMode_DefaultValue_IsZoomInOut()
    {
        // Arrange & Act
        var zoomBorder = new ZoomBorder();

        // Assert
        Assert.Equal(DoubleClickZoomMode.ZoomInOut, zoomBorder.DoubleClickZoomMode);
    }

    [AvaloniaFact]
    public void DoubleClickZoomMode_CanBeSetToZoomIn()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();

        // Act
        zoomBorder.DoubleClickZoomMode = DoubleClickZoomMode.ZoomIn;

        // Assert
        Assert.Equal(DoubleClickZoomMode.ZoomIn, zoomBorder.DoubleClickZoomMode);
    }

    [AvaloniaFact]
    public void DoubleClickZoomMode_CanBeSetToZoomOut()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();

        // Act
        zoomBorder.DoubleClickZoomMode = DoubleClickZoomMode.ZoomOut;

        // Assert
        Assert.Equal(DoubleClickZoomMode.ZoomOut, zoomBorder.DoubleClickZoomMode);
    }

    [AvaloniaFact]
    public void DoubleClickZoomMode_CanBeSetToZoomToFit()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();

        // Act
        zoomBorder.DoubleClickZoomMode = DoubleClickZoomMode.ZoomToFit;

        // Assert
        Assert.Equal(DoubleClickZoomMode.ZoomToFit, zoomBorder.DoubleClickZoomMode);
    }

    [AvaloniaFact]
    public void DoubleClickZoomMode_CanBeSetToNone()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();

        // Act
        zoomBorder.DoubleClickZoomMode = DoubleClickZoomMode.None;

        // Assert
        Assert.Equal(DoubleClickZoomMode.None, zoomBorder.DoubleClickZoomMode);
    }

    [AvaloniaFact]
    public void DoubleClickZoomFactor_DefaultValue_IsTwo()
    {
        // Arrange & Act
        var zoomBorder = new ZoomBorder();

        // Assert
        Assert.Equal(2.0, zoomBorder.DoubleClickZoomFactor);
    }

    [AvaloniaFact]
    public void DoubleClickZoomFactor_CanBeSetToCustomValue()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();

        // Act
        zoomBorder.DoubleClickZoomFactor = 3.5;

        // Assert
        Assert.Equal(3.5, zoomBorder.DoubleClickZoomFactor);
    }

    [AvaloniaFact]
    public void DoubleClickZoom_AllPropertiesCanBeSetTogether()
    {
        // Arrange & Act
        var zoomBorder = new ZoomBorder
        {
            EnableDoubleClickZoom = true,
            DoubleClickZoomMode = DoubleClickZoomMode.ZoomIn,
            DoubleClickZoomFactor = 2.5
        };

        // Assert
        Assert.True(zoomBorder.EnableDoubleClickZoom);
        Assert.Equal(DoubleClickZoomMode.ZoomIn, zoomBorder.DoubleClickZoomMode);
        Assert.Equal(2.5, zoomBorder.DoubleClickZoomFactor);
    }
}
