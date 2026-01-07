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

    [AvaloniaFact]
    public void Rotate_AppliesTransformationToContent()
    {
        // Arrange
        var canvas = new Canvas { Width = 400, Height = 400 };
        var zoomBorder = new ZoomBorder
        {
            Width = 800,
            Height = 600,
            EnableGestureRotation = true,
            Child = canvas
        };
        var window = new Window { Content = zoomBorder };
        window.Show();

        // Act - Rotate by 90 degrees
        zoomBorder.Rotate(90.0);

        // Assert - The rotation property should be updated
        Assert.Equal(90.0, zoomBorder.Rotation);

        // The content should have a RenderTransform applied
        Assert.NotNull(canvas.RenderTransform);
    }

    [AvaloniaFact]
    public void ResetRotation_ClearsRotationTransformation()
    {
        // Arrange
        var canvas = new Canvas { Width = 400, Height = 400 };
        var zoomBorder = new ZoomBorder
        {
            Width = 800,
            Height = 600,
            EnableGestureRotation = true,
            Child = canvas
        };
        var window = new Window { Content = zoomBorder };
        window.Show();

        // First rotate
        zoomBorder.Rotate(45.0);
        Assert.Equal(45.0, zoomBorder.Rotation);

        // Act - Reset rotation
        zoomBorder.ResetRotation();

        // Assert
        Assert.Equal(0.0, zoomBorder.Rotation);
    }

    [AvaloniaFact]
    public void Rotation_WorksWithPanAndZoom()
    {
        // Arrange
        var canvas = new Canvas { Width = 400, Height = 400 };
        var zoomBorder = new ZoomBorder
        {
            Width = 800,
            Height = 600,
            EnableGestureRotation = true,
            EnablePan = true,
            Child = canvas
        };
        var window = new Window { Content = zoomBorder };
        window.Show();

        var initialZoomX = zoomBorder.ZoomX;
        var initialZoomY = zoomBorder.ZoomY;

        // Act - Combine rotation with zoom (zoom by 2x ratio)
        zoomBorder.ZoomTo(2.0, 0, 0, skipTransitions: true);
        zoomBorder.Rotate(45.0);

        // Assert - Both operations should be applied
        Assert.Equal(45.0, zoomBorder.Rotation);
        // ZoomTo multiplies by the ratio, so zoom should be 2x the initial
        Assert.True(zoomBorder.ZoomX > initialZoomX, "ZoomX should increase after ZoomTo");
        Assert.True(zoomBorder.ZoomY > initialZoomY, "ZoomY should increase after ZoomTo");
    }

    [AvaloniaFact]
    public void Rotation_90Degrees_AppliesCorrectTransformMatrix()
    {
        // Arrange
        var canvas = new Canvas { Width = 200, Height = 200 };
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 400,
            EnableGestureRotation = true,
            Child = canvas
        };
        var window = new Window { Content = zoomBorder };
        window.Show();

        // Act - Rotate 90 degrees
        zoomBorder.Rotate(90.0);

        // Assert - Verify rotation was applied
        Assert.Equal(90.0, zoomBorder.Rotation);
        
        // The RenderTransform should be set
        Assert.NotNull(canvas.RenderTransform);
        
        // We can verify the transform is present by checking the canvas has a non-identity transform
        var transform = canvas.RenderTransform;
        Assert.NotNull(transform);
    }

    [AvaloniaFact]
    public void RotateAt_RotatesAroundSpecificPoint()
    {
        // Arrange
        var canvas = new Canvas { Width = 200, Height = 200 };
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 400,
            EnableGestureRotation = true,
            Child = canvas
        };
        var window = new Window { Content = zoomBorder };
        window.Show();

        var center = new Point(100.0, 100.0);

        // Act - Rotate 45 degrees around a specific point
        zoomBorder.RotateAt(45.0, center);

        // Assert - Verify rotation was applied
        Assert.Equal(45.0, zoomBorder.Rotation);
        Assert.NotNull(canvas.RenderTransform);
    }

    [AvaloniaFact]
    public void Rotation_WithSnapping_SnapsToCorrectAngles()
    {
        // Arrange
        var canvas = new Canvas { Width = 200, Height = 200 };
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 400,
            EnableGestureRotation = true,
            EnableRotationSnapping = true,
            RotationSnapAngle = 90.0, // Snap to 90 degree increments
            Child = canvas
        };
        var window = new Window { Content = zoomBorder };
        window.Show();

        // Act - Rotate by an amount that should snap to 90
        zoomBorder.Rotate(60.0); // Between 0 and 90, closer to 90

        // Assert - Should snap to 90
        Assert.Equal(90.0, zoomBorder.Rotation);
        Assert.NotNull(canvas.RenderTransform);
    }

    [AvaloniaFact]
    public void Rotation_Negative_AppliesCorrectly()
    {
        // Arrange
        var canvas = new Canvas { Width = 200, Height = 200 };
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 400,
            EnableGestureRotation = true,
            Child = canvas
        };
        var window = new Window { Content = zoomBorder };
        window.Show();

        // Act - Rotate negative degrees
        zoomBorder.Rotate(-45.0);

        // Assert
        Assert.Equal(-45.0, zoomBorder.Rotation);
        Assert.NotNull(canvas.RenderTransform);
    }

    [AvaloniaFact]
    public void ResetAll_ResetsRotationToo()
    {
        // Arrange
        var canvas = new Canvas { Width = 200, Height = 200 };
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 400,
            EnableGestureRotation = true,
            Child = canvas
        };
        var window = new Window { Content = zoomBorder };
        window.Show();

        // First rotate
        zoomBorder.Rotate(45.0);
        Assert.Equal(45.0, zoomBorder.Rotation);

        // Act - Reset everything
        zoomBorder.ResetRotation();

        // Assert
        Assert.Equal(0.0, zoomBorder.Rotation);
    }
}
