// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Xunit;

namespace Avalonia.Controls.PanAndZoom.UnitTests;

/// <summary>
/// Tests for resize behavior functionality.
/// </summary>
public class ZoomBorderResizeBehaviorTests
{
    [AvaloniaFact]
    public void ResizeBehavior_DefaultValue_IsNone()
    {
        // Arrange & Act
        var zoomBorder = new ZoomBorder();

        // Assert
        Assert.Equal(ResizeBehaviorMode.None, zoomBorder.ResizeBehavior);
    }

    [AvaloniaFact]
    public void ResizeBehavior_CanBeSetToNone()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();

        // Act
        zoomBorder.ResizeBehavior = ResizeBehaviorMode.None;

        // Assert
        Assert.Equal(ResizeBehaviorMode.None, zoomBorder.ResizeBehavior);
    }

    [AvaloniaFact]
    public void ResizeBehavior_CanBeSetToMaintainCenter()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();

        // Act
        zoomBorder.ResizeBehavior = ResizeBehaviorMode.MaintainCenter;

        // Assert
        Assert.Equal(ResizeBehaviorMode.MaintainCenter, zoomBorder.ResizeBehavior);
    }

    [AvaloniaFact]
    public void ResizeBehavior_CanBeSetToMaintainTopLeft()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();

        // Act
        zoomBorder.ResizeBehavior = ResizeBehaviorMode.MaintainTopLeft;

        // Assert
        Assert.Equal(ResizeBehaviorMode.MaintainTopLeft, zoomBorder.ResizeBehavior);
    }

    [AvaloniaFact]
    public void ResizeBehavior_CanBeSetToMaintainZoom()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();

        // Act
        zoomBorder.ResizeBehavior = ResizeBehaviorMode.MaintainZoom;

        // Assert
        Assert.Equal(ResizeBehaviorMode.MaintainZoom, zoomBorder.ResizeBehavior);
    }

    [AvaloniaFact]
    public void ResizeBehavior_CanBeSetToReapplyStretch()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();

        // Act
        zoomBorder.ResizeBehavior = ResizeBehaviorMode.ReapplyStretch;

        // Assert
        Assert.Equal(ResizeBehaviorMode.ReapplyStretch, zoomBorder.ResizeBehavior);
    }

    [AvaloniaFact]
    public void ResizeBehavior_CanBeSetToCustom()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();

        // Act
        zoomBorder.ResizeBehavior = ResizeBehaviorMode.Custom;

        // Assert
        Assert.Equal(ResizeBehaviorMode.Custom, zoomBorder.ResizeBehavior);
    }

    [AvaloniaFact]
    public void ResizeBehavior_None_MaintainsCurrentOffsetOnResize()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300,
            ResizeBehavior = ResizeBehaviorMode.None
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

        var initialOffsetX = zoomBorder.OffsetX;
        var initialOffsetY = zoomBorder.OffsetY;

        // Act - Resize the control
        zoomBorder.Width = 600;
        zoomBorder.Height = 450;

        // Assert - Offset should remain the same with None mode
        Assert.Equal(initialOffsetX, zoomBorder.OffsetX);
        Assert.Equal(initialOffsetY, zoomBorder.OffsetY);
    }

    [AvaloniaFact]
    public void ResizeBehavior_MaintainZoom_PreservesZoomLevelOnResize()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300,
            ResizeBehavior = ResizeBehaviorMode.MaintainZoom
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

        // Pan to a specific location
        zoomBorder.Pan(50, 50);
        var initialZoomX = zoomBorder.ZoomX;

        // Act - Resize the control
        zoomBorder.Width = 800; // Double the width
        zoomBorder.Height = 600; // Double the height

        // Assert - Zoom should remain the same
        Assert.Equal(initialZoomX, zoomBorder.ZoomX);
    }
}
