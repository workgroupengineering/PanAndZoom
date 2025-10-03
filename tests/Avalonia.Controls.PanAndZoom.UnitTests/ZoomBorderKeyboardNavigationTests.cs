// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Media;
using Xunit;

namespace Avalonia.Controls.PanAndZoom.UnitTests;

/// <summary>
/// Tests for keyboard navigation functionality.
/// </summary>
public class ZoomBorderKeyboardNavigationTests
{
    [AvaloniaFact]
    public void EnableKeyboardNavigation_DefaultValue_IsTrue()
    {
        // Arrange & Act
        var zoomBorder = new ZoomBorder();

        // Assert
        Assert.True(zoomBorder.EnableKeyboardNavigation);
    }

    [AvaloniaFact]
    public void EnableKeyboardNavigation_CanBeSetToFalse()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();

        // Act
        zoomBorder.EnableKeyboardNavigation = false;

        // Assert
        Assert.False(zoomBorder.EnableKeyboardNavigation);
    }

    [AvaloniaFact]
    public void KeyboardPanStep_DefaultValue_Is50()
    {
        // Arrange & Act
        var zoomBorder = new ZoomBorder();

        // Assert
        Assert.Equal(50.0, zoomBorder.KeyboardPanStep);
    }

    [AvaloniaFact]
    public void KeyboardPanStep_CanBeSetToCustomValue()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();

        // Act
        zoomBorder.KeyboardPanStep = 100.0;

        // Assert
        Assert.Equal(100.0, zoomBorder.KeyboardPanStep);
    }

    [AvaloniaFact]
    public void KeyboardZoomStep_DefaultValue_Is1Point1()
    {
        // Arrange & Act
        var zoomBorder = new ZoomBorder();

        // Assert
        Assert.Equal(1.1, zoomBorder.KeyboardZoomStep);
    }

    [AvaloniaFact]
    public void KeyboardZoomStep_CanBeSetToCustomValue()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();

        // Act
        zoomBorder.KeyboardZoomStep = 1.5;

        // Assert
        Assert.Equal(1.5, zoomBorder.KeyboardZoomStep);
    }

    [AvaloniaFact]
    public void KeyboardNavigation_AllPropertiesCanBeSetTogether()
    {
        // Arrange & Act
        var zoomBorder = new ZoomBorder
        {
            EnableKeyboardNavigation = false,
            KeyboardPanStep = 75.0,
            KeyboardZoomStep = 1.25
        };

        // Assert
        Assert.False(zoomBorder.EnableKeyboardNavigation);
        Assert.Equal(75.0, zoomBorder.KeyboardPanStep);
        Assert.Equal(1.25, zoomBorder.KeyboardZoomStep);
    }

    [AvaloniaFact]
    public void KeyboardNavigation_LeftArrow_PansLeft()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300,
            EnableKeyboardNavigation = true,
            EnablePan = true,
            KeyboardPanStep = 50.0
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

        // Act
        var keyEventArgs = new KeyEventArgs
        {
            Key = Key.Left,
            RoutedEvent = InputElement.KeyDownEvent
        };
        zoomBorder.RaiseEvent(keyEventArgs);

        // Assert
        Assert.True(zoomBorder.OffsetX < initialOffsetX, "Offset should decrease when panning left");
    }

    [AvaloniaFact]
    public void KeyboardNavigation_RightArrow_PansRight()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300,
            EnableKeyboardNavigation = true,
            EnablePan = true,
            KeyboardPanStep = 50.0
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

        // Act
        var keyEventArgs = new KeyEventArgs
        {
            Key = Key.Right,
            RoutedEvent = InputElement.KeyDownEvent
        };
        zoomBorder.RaiseEvent(keyEventArgs);

        // Assert
        Assert.True(zoomBorder.OffsetX > initialOffsetX, "Offset should increase when panning right");
    }

    [AvaloniaFact]
    public void KeyboardNavigation_UpArrow_PansUp()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300,
            EnableKeyboardNavigation = true,
            EnablePan = true,
            KeyboardPanStep = 50.0
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

        var initialOffsetY = zoomBorder.OffsetY;

        // Act
        var keyEventArgs = new KeyEventArgs
        {
            Key = Key.Up,
            RoutedEvent = InputElement.KeyDownEvent
        };
        zoomBorder.RaiseEvent(keyEventArgs);

        // Assert
        Assert.True(zoomBorder.OffsetY < initialOffsetY, "Offset should decrease when panning up");
    }

    [AvaloniaFact]
    public void KeyboardNavigation_DownArrow_PansDown()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300,
            EnableKeyboardNavigation = true,
            EnablePan = true,
            KeyboardPanStep = 50.0
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

        var initialOffsetY = zoomBorder.OffsetY;

        // Act
        var keyEventArgs = new KeyEventArgs
        {
            Key = Key.Down,
            RoutedEvent = InputElement.KeyDownEvent
        };
        zoomBorder.RaiseEvent(keyEventArgs);

        // Assert
        Assert.True(zoomBorder.OffsetY > initialOffsetY, "Offset should increase when panning down");
    }

    [AvaloniaFact]
    public void KeyboardNavigation_PlusKey_ZoomsIn()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300,
            EnableKeyboardNavigation = true,
            EnableZoom = true,
            KeyboardZoomStep = 1.5
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

        var initialZoom = zoomBorder.ZoomX;

        // Act
        var keyEventArgs = new KeyEventArgs
        {
            Key = Key.OemPlus,
            RoutedEvent = InputElement.KeyDownEvent
        };
        zoomBorder.RaiseEvent(keyEventArgs);

        // Assert
        Assert.True(zoomBorder.ZoomX > initialZoom, "Zoom should increase with Plus key");
    }

    [AvaloniaFact]
    public void KeyboardNavigation_MinusKey_ZoomsOut()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300,
            EnableKeyboardNavigation = true,
            EnableZoom = true,
            KeyboardZoomStep = 1.5
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

        // Zoom in first
        zoomBorder.ZoomIn();
        var initialZoom = zoomBorder.ZoomX;

        // Act
        var keyEventArgs = new KeyEventArgs
        {
            Key = Key.OemMinus,
            RoutedEvent = InputElement.KeyDownEvent
        };
        zoomBorder.RaiseEvent(keyEventArgs);

        // Assert
        Assert.True(zoomBorder.ZoomX < initialZoom, "Zoom should decrease with Minus key");
    }

    [AvaloniaFact]
    public void KeyboardNavigation_HomeKey_FitsToViewport()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300,
            EnableKeyboardNavigation = true,
            Stretch = StretchMode.Uniform
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

        // Zoom in first
        zoomBorder.ZoomIn();
        var zoomBeforeFit = zoomBorder.ZoomX;

        // Act
        var keyEventArgs = new KeyEventArgs
        {
            Key = Key.Home,
            RoutedEvent = InputElement.KeyDownEvent
        };
        zoomBorder.RaiseEvent(keyEventArgs);

        // Assert - AutoFit should apply
        Assert.True(zoomBorder.ZoomX != zoomBeforeFit, "Home key should trigger AutoFit");
    }

    [AvaloniaFact]
    public void KeyboardNavigation_Ctrl0_ResetsView()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300,
            EnableKeyboardNavigation = true
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

        // Zoom and pan
        zoomBorder.ZoomIn();
        zoomBorder.Pan(50, 50);

        // Act
        var keyEventArgs = new KeyEventArgs
        {
            Key = Key.D0,
            KeyModifiers = KeyModifiers.Control,
            RoutedEvent = InputElement.KeyDownEvent
        };
        zoomBorder.RaiseEvent(keyEventArgs);

        // Assert - Should reset to identity matrix
        Assert.Equal(1.0, zoomBorder.ZoomX);
        Assert.Equal(1.0, zoomBorder.ZoomY);
        Assert.Equal(0.0, zoomBorder.OffsetX);
        Assert.Equal(0.0, zoomBorder.OffsetY);
    }

    [AvaloniaFact]
    public void KeyboardNavigation_DisabledWhenEnableKeyboardNavigationIsFalse()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300,
            EnableKeyboardNavigation = false,
            EnablePan = true,
            KeyboardPanStep = 50.0
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

        // Act
        var keyEventArgs = new KeyEventArgs
        {
            Key = Key.Left,
            RoutedEvent = InputElement.KeyDownEvent
        };
        zoomBorder.RaiseEvent(keyEventArgs);

        // Assert - Offset should not change when keyboard navigation is disabled
        Assert.Equal(initialOffsetX, zoomBorder.OffsetX);
    }
}
