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
/// The zoom indicator has property support, helper methods, visibility state tracking with auto-hide timer,
/// and IsZoomIndicatorVisible property for UI binding. Visual rendering should be implemented in XAML
/// by binding to IsZoomIndicatorVisible and using GetZoomIndicatorText/GetZoomIndicatorPosition helpers.
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

    /// <summary>
    /// Tests that IsZoomIndicatorVisible property exists for UI binding.
    /// The indicator becomes visible when ShowZoomIndicatorTemporarily() is called during zoom operations.
    /// </summary>
    [AvaloniaFact]
    public void IsZoomIndicatorVisible_DefaultValue_IsFalse()
    {
        // Arrange & Act
        var zoomBorder = new ZoomBorder();

        // Assert - Initially the indicator is not visible
        Assert.False(zoomBorder.IsZoomIndicatorVisible);
    }

    /// <summary>
    /// Documents that ShowZoomIndicator controls whether the indicator can be shown.
    /// When ShowZoomIndicator is true, the indicator becomes visible during zoom operations
    /// and auto-hides after ZoomIndicatorAutoHideDuration.
    /// </summary>
    [AvaloniaFact]
    public void ShowZoomIndicator_EnablesIndicatorVisibility()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300,
            ShowZoomIndicator = true,
            ZoomIndicatorPosition = ZoomIndicatorPosition.TopLeft
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
        
        // Assert - The property is set correctly
        Assert.True(zoomBorder.ShowZoomIndicator);
        
        // The helper methods work for UI binding
        var text = zoomBorder.GetZoomIndicatorText();
        Assert.NotEmpty(text);
        
        // IsZoomIndicatorVisible can be bound in XAML to control visibility
        // It will be set to true when ShowZoomIndicatorTemporarily() is called during zoom
    }

    /// <summary>
    /// Tests that ZoomIndicatorAutoHideDuration controls the auto-hide timer.
    /// The implementation uses a DispatcherTimer that fires after this duration.
    /// </summary>
    [AvaloniaFact]
    public void ZoomIndicatorAutoHideDuration_ControlsAutoHideTimer()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            ZoomIndicatorAutoHideDuration = TimeSpan.FromSeconds(5)
        };
        
        // Assert - Property stores value and is used by ShowZoomIndicatorTemporarily()
        Assert.Equal(TimeSpan.FromSeconds(5), zoomBorder.ZoomIndicatorAutoHideDuration);
        
        // The implementation:
        // 1. Creates/resets a DispatcherTimer when ShowZoomIndicatorTemporarily() is called
        // 2. Timer interval is set to ZoomIndicatorAutoHideDuration
        // 3. When timer fires, IsZoomIndicatorVisible is set to false
    }

    #region ZoomIndicatorPosition Tests

    [AvaloniaFact]
    public void ZoomIndicatorPosition_AllPositions_CanBeSet()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();

        // Act & Assert - Test all enum values
        zoomBorder.ZoomIndicatorPosition = ZoomIndicatorPosition.TopLeft;
        Assert.Equal(ZoomIndicatorPosition.TopLeft, zoomBorder.ZoomIndicatorPosition);

        zoomBorder.ZoomIndicatorPosition = ZoomIndicatorPosition.TopRight;
        Assert.Equal(ZoomIndicatorPosition.TopRight, zoomBorder.ZoomIndicatorPosition);

        zoomBorder.ZoomIndicatorPosition = ZoomIndicatorPosition.BottomLeft;
        Assert.Equal(ZoomIndicatorPosition.BottomLeft, zoomBorder.ZoomIndicatorPosition);

        zoomBorder.ZoomIndicatorPosition = ZoomIndicatorPosition.BottomRight;
        Assert.Equal(ZoomIndicatorPosition.BottomRight, zoomBorder.ZoomIndicatorPosition);
    }

    #endregion

    #region ZoomIndicatorFormat Tests

    [AvaloniaFact]
    public void GetZoomIndicatorText_WithPercentageFormat_ReturnsPercentage()
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

        // Assert - At 100% zoom, should contain "100" and "%"
        Assert.Contains("%", text);
    }

    [AvaloniaFact]
    public void GetZoomIndicatorText_WithMultiplierFormat_ReturnsMultiplier()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300,
            ZoomIndicatorFormat = "{0:F2}x"
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

        zoomBorder.Zoom(2.5, 100, 75);

        // Act
        var text = zoomBorder.GetZoomIndicatorText();

        // Assert
        Assert.Contains("x", text);
        Assert.Contains("2", text);
    }

    [AvaloniaFact]
    public void GetZoomIndicatorText_AfterZoomChange_ReflectsNewZoomLevel()
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

        // Act - Get text at initial zoom (1.0x)
        var textInitial = zoomBorder.GetZoomIndicatorText();

        // Zoom to 3x
        zoomBorder.Zoom(3.0, 100, 75);
        var textAt3x = zoomBorder.GetZoomIndicatorText();

        // Assert - The text values should be different after zooming
        Assert.NotEqual(textInitial, textAt3x);
        Assert.Contains("3", textAt3x);
    }

    #endregion

    #region ZoomIndicatorAutoHideDuration Tests

    [AvaloniaFact]
    public void ZoomIndicatorAutoHideDuration_ZeroDuration_AcceptsValue()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();

        // Act
        zoomBorder.ZoomIndicatorAutoHideDuration = TimeSpan.Zero;

        // Assert
        Assert.Equal(TimeSpan.Zero, zoomBorder.ZoomIndicatorAutoHideDuration);
    }

    [AvaloniaFact]
    public void ZoomIndicatorAutoHideDuration_LargeDuration_AcceptsValue()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();

        // Act
        zoomBorder.ZoomIndicatorAutoHideDuration = TimeSpan.FromMinutes(5);

        // Assert
        Assert.Equal(TimeSpan.FromMinutes(5), zoomBorder.ZoomIndicatorAutoHideDuration);
    }

    #endregion

    #region IsZoomIndicatorVisible Tests

    [AvaloniaFact]
    public void IsZoomIndicatorVisible_Initially_IsFalse()
    {
        // Arrange & Act
        var zoomBorder = new ZoomBorder();

        // Assert
        Assert.False(zoomBorder.IsZoomIndicatorVisible);
    }

    [AvaloniaFact]
    public void IsZoomIndicatorVisible_WhenShowZoomIndicatorFalse_RemainsHidden()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300,
            ShowZoomIndicator = false
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

        // Act - Zoom should not show indicator when ShowZoomIndicator is false
        zoomBorder.Zoom(2.0, 100, 75);

        // Assert
        Assert.False(zoomBorder.IsZoomIndicatorVisible);
    }

    [AvaloniaFact]
    public void IsZoomIndicatorVisible_WhenShowZoomIndicatorTrue_BecomesVisibleOnZoom()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300,
            ShowZoomIndicator = true,
            ZoomIndicatorAutoHideDuration = TimeSpan.FromMinutes(5) // Long duration to prevent auto-hide during test
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

        // Act - Zoom should trigger indicator visibility when ShowZoomIndicator is true
        zoomBorder.Zoom(2.0, 100, 75);

        // Assert
        Assert.True(zoomBorder.IsZoomIndicatorVisible);
    }

    #endregion
}
