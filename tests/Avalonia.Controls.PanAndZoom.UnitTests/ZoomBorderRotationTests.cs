// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Xunit;

namespace Avalonia.Controls.PanAndZoom.UnitTests;

/// <summary>
/// Tests for ZoomBorder rotation functionality.
/// </summary>
public class ZoomBorderRotationTests
{
    [AvaloniaFact]
    public void Rotation_DefaultValue_IsZero()
    {
        // Arrange & Act
        var zoomBorder = new ZoomBorder();

        // Assert
        Assert.Equal(0.0, zoomBorder.Rotation);
    }

    [AvaloniaFact]
    public void MinRotation_DefaultValue_IsNegative180()
    {
        // Arrange & Act
        var zoomBorder = new ZoomBorder();

        // Assert
        Assert.Equal(-180.0, zoomBorder.MinRotation);
    }

    [AvaloniaFact]
    public void MaxRotation_DefaultValue_Is180()
    {
        // Arrange & Act
        var zoomBorder = new ZoomBorder();

        // Assert
        Assert.Equal(180.0, zoomBorder.MaxRotation);
    }

    [AvaloniaFact]
    public void EnableRotationSnapping_DefaultValue_IsFalse()
    {
        // Arrange & Act
        var zoomBorder = new ZoomBorder();

        // Assert
        Assert.False(zoomBorder.EnableRotationSnapping);
    }

    [AvaloniaFact]
    public void RotationSnapAngle_DefaultValue_Is45()
    {
        // Arrange & Act
        var zoomBorder = new ZoomBorder();

        // Assert
        Assert.Equal(45.0, zoomBorder.RotationSnapAngle);
    }

    [AvaloniaFact]
    public void Rotation_CanBeSet()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();

        // Act
        zoomBorder.Rotation = 90.0;

        // Assert
        Assert.Equal(90.0, zoomBorder.Rotation);
    }

    [AvaloniaFact]
    public void Rotate_IncreasesRotation()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            EnableGestureRotation = true
        };

        // Act
        zoomBorder.Rotate(45.0);

        // Assert
        Assert.Equal(45.0, zoomBorder.Rotation);
    }

    [AvaloniaFact]
    public void Rotate_WithSnapping_SnapsToNearestAngle()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            EnableGestureRotation = true,
            EnableRotationSnapping = true,
            RotationSnapAngle = 45.0
        };

        // Act
        zoomBorder.Rotate(30.0); // Should snap to 45.0

        // Assert
        Assert.Equal(45.0, zoomBorder.Rotation);
    }

    [AvaloniaFact]
    public void Rotate_RespectMinRotation()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            EnableGestureRotation = true,
            MinRotation = -90.0
        };

        // Act
        zoomBorder.Rotate(-180.0); // Should clamp to -90.0

        // Assert
        Assert.Equal(-90.0, zoomBorder.Rotation);
    }

    [AvaloniaFact]
    public void Rotate_RespectMaxRotation()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            EnableGestureRotation = true,
            MaxRotation = 90.0
        };

        // Act
        zoomBorder.Rotate(180.0); // Should clamp to 90.0

        // Assert
        Assert.Equal(90.0, zoomBorder.Rotation);
    }

    [AvaloniaFact]
    public void ResetRotation_SetsRotationToZero()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            Rotation = 90.0
        };

        // Act
        zoomBorder.ResetRotation();

        // Assert
        Assert.Equal(0.0, zoomBorder.Rotation);
    }

    [AvaloniaFact]
    public void SnapRotation_SnapsCurrentRotationToNearestAngle()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            Rotation = 32.0,
            EnableRotationSnapping = true,
            RotationSnapAngle = 45.0
        };

        // Act
        zoomBorder.SnapRotation();

        // Assert
        Assert.Equal(45.0, zoomBorder.Rotation);
    }

    [AvaloniaFact]
    public void Rotate_WhenGestureRotationDisabled_DoesNotChangeRotation()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            EnableGestureRotation = false
        };

        // Act
        zoomBorder.Rotate(45.0);

        // Assert
        Assert.Equal(0.0, zoomBorder.Rotation);
    }
}
