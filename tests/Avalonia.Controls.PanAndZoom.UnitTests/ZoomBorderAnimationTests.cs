// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Avalonia.Headless.XUnit;
using Xunit;

namespace Avalonia.Controls.PanAndZoom.UnitTests;

/// <summary>
/// Tests for animation functionality.
/// </summary>
public class ZoomBorderAnimationTests
{
    [AvaloniaFact]
    public void EnableAnimations_DefaultValue_IsFalse()
    {
        // Arrange & Act
        var zoomBorder = new ZoomBorder();

        // Assert
        Assert.False(zoomBorder.EnableAnimations);
    }

    [AvaloniaFact]
    public void EnableAnimations_CanBeSetToTrue()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();

        // Act
        zoomBorder.EnableAnimations = true;

        // Assert
        Assert.True(zoomBorder.EnableAnimations);
    }

    [AvaloniaFact]
    public void AnimationDuration_DefaultValue_Is300Milliseconds()
    {
        // Arrange & Act
        var zoomBorder = new ZoomBorder();

        // Assert
        Assert.Equal(TimeSpan.FromMilliseconds(300), zoomBorder.AnimationDuration);
    }

    [AvaloniaFact]
    public void AnimationDuration_CanBeSetToCustomValue()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();

        // Act
        zoomBorder.AnimationDuration = TimeSpan.FromMilliseconds(500);

        // Assert
        Assert.Equal(TimeSpan.FromMilliseconds(500), zoomBorder.AnimationDuration);
    }

    [AvaloniaFact]
    public void Animation_AllPropertiesCanBeSetTogether()
    {
        // Arrange & Act
        var zoomBorder = new ZoomBorder
        {
            EnableAnimations = true,
            AnimationDuration = TimeSpan.FromMilliseconds(250)
        };

        // Assert
        Assert.True(zoomBorder.EnableAnimations);
        Assert.Equal(TimeSpan.FromMilliseconds(250), zoomBorder.AnimationDuration);
    }

    [AvaloniaFact]
    public void AnimationDuration_CanBeSetToZero()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();

        // Act
        zoomBorder.AnimationDuration = TimeSpan.Zero;

        // Assert
        Assert.Equal(TimeSpan.Zero, zoomBorder.AnimationDuration);
    }

    [AvaloniaFact]
    public void AnimationDuration_CanBeSetToLongDuration()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();

        // Act
        zoomBorder.AnimationDuration = TimeSpan.FromSeconds(2);

        // Assert
        Assert.Equal(TimeSpan.FromSeconds(2), zoomBorder.AnimationDuration);
    }
}
