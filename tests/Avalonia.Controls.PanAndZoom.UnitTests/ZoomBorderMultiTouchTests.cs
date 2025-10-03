// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Xunit;

namespace Avalonia.Controls.PanAndZoom.UnitTests;

/// <summary>
/// Tests for ZoomBorder multi-touch functionality.
/// </summary>
public class ZoomBorderMultiTouchTests
{
    [AvaloniaFact]
    public void EnableSimultaneousPanZoom_DefaultValue_IsTrue()
    {
        // Arrange & Act
        var zoomBorder = new ZoomBorder();

        // Assert
        Assert.True(zoomBorder.EnableSimultaneousPanZoom);
    }

    [AvaloniaFact]
    public void MinimumTouchPoints_DefaultValue_Is1()
    {
        // Arrange & Act
        var zoomBorder = new ZoomBorder();

        // Assert
        Assert.Equal(1, zoomBorder.MinimumTouchPoints);
    }

    [AvaloniaFact]
    public void MaximumTouchPoints_DefaultValue_Is2()
    {
        // Arrange & Act
        var zoomBorder = new ZoomBorder();

        // Assert
        Assert.Equal(2, zoomBorder.MaximumTouchPoints);
    }

    [AvaloniaFact]
    public void GestureRecognitionDelay_DefaultValue_Is50Milliseconds()
    {
        // Arrange & Act
        var zoomBorder = new ZoomBorder();

        // Assert
        Assert.Equal(TimeSpan.FromMilliseconds(50), zoomBorder.GestureRecognitionDelay);
    }

    [AvaloniaFact]
    public void EnableSimultaneousPanZoom_CanBeSet()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();

        // Act
        zoomBorder.EnableSimultaneousPanZoom = false;

        // Assert
        Assert.False(zoomBorder.EnableSimultaneousPanZoom);
    }

    [AvaloniaFact]
    public void MinimumTouchPoints_CanBeSet()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();

        // Act
        zoomBorder.MinimumTouchPoints = 2;

        // Assert
        Assert.Equal(2, zoomBorder.MinimumTouchPoints);
    }

    [AvaloniaFact]
    public void MaximumTouchPoints_CanBeSet()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();

        // Act
        zoomBorder.MaximumTouchPoints = 3;

        // Assert
        Assert.Equal(3, zoomBorder.MaximumTouchPoints);
    }

    [AvaloniaFact]
    public void GestureRecognitionDelay_CanBeSet()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();

        // Act
        zoomBorder.GestureRecognitionDelay = TimeSpan.FromMilliseconds(100);

        // Assert
        Assert.Equal(TimeSpan.FromMilliseconds(100), zoomBorder.GestureRecognitionDelay);
    }
}
