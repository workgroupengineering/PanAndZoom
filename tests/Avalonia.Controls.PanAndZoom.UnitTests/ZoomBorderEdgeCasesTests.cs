// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Media;
using Xunit;

namespace Avalonia.Controls.PanAndZoom.UnitTests;

/// <summary>
/// Tests for edge cases and additional scenarios in ZoomBorder.
/// </summary>
public class ZoomBorderEdgeCasesTests
{
    // ===== Keyboard Navigation Edge Cases =====

    [AvaloniaFact]
    public void KeyDown_D0_WithoutControl_DoesNotReset()
    {
        // Arrange
        var canvas = new Canvas { Width = 200, Height = 200 };
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300,
            EnableKeyboardNavigation = true,
            Child = canvas
        };
        var window = new Window { Content = zoomBorder };
        window.Show();

        // First zoom in
        zoomBorder.ZoomTo(2.0, 100, 100, skipTransitions: true);
        var zoomedValue = zoomBorder.ZoomX;

        // Act - Press D0 without Control modifier
        var keyEventArgs = new KeyEventArgs
        {
            RoutedEvent = InputElement.KeyDownEvent,
            Key = Key.D0,
            KeyModifiers = KeyModifiers.None
        };
        zoomBorder.RaiseEvent(keyEventArgs);

        // Assert - Zoom should remain unchanged
        Assert.Equal(zoomedValue, zoomBorder.ZoomX);
    }

    [AvaloniaFact]
    public void KeyDown_UnhandledKey_DoesNotAffectZoom()
    {
        // Arrange
        var canvas = new Canvas { Width = 200, Height = 200 };
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300,
            EnableKeyboardNavigation = true,
            Child = canvas
        };
        var window = new Window { Content = zoomBorder };
        window.Show();

        var initialZoomX = zoomBorder.ZoomX;
        var initialOffsetX = zoomBorder.OffsetX;

        // Act - Press an unhandled key (e.g., A)
        var keyEventArgs = new KeyEventArgs
        {
            RoutedEvent = InputElement.KeyDownEvent,
            Key = Key.A,
            KeyModifiers = KeyModifiers.None
        };
        zoomBorder.RaiseEvent(keyEventArgs);

        // Assert - State should remain unchanged
        Assert.Equal(initialZoomX, zoomBorder.ZoomX);
        Assert.Equal(initialOffsetX, zoomBorder.OffsetX);
    }

    // ===== Child Element Change Tests =====

    [AvaloniaFact]
    public void ChangingChild_DetachesOldAndAttachesNew()
    {
        // Arrange
        var firstCanvas = new Canvas { Width = 200, Height = 200 };
        var secondCanvas = new Canvas { Width = 300, Height = 300 };
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300,
            Child = firstCanvas
        };
        var window = new Window { Content = zoomBorder };
        window.Show();

        // Zoom the first child
        zoomBorder.ZoomTo(2.0, 100, 100, skipTransitions: true);

        // Act - Change the child
        zoomBorder.Child = secondCanvas;

        // Assert - ZoomBorder should still work with new child
        Assert.NotNull(zoomBorder.Child);
        Assert.Equal(secondCanvas, zoomBorder.Child);
    }

    [AvaloniaFact]
    public void RemovingChild_ClearsElement()
    {
        // Arrange
        var canvas = new Canvas { Width = 200, Height = 200 };
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300,
            Child = canvas
        };
        var window = new Window { Content = zoomBorder };
        window.Show();

        // Act - Remove the child
        zoomBorder.Child = null;

        // Assert
        Assert.Null(zoomBorder.Child);
    }

    // ===== Pointer Capture Loss Tests =====

    [AvaloniaFact]
    public void PointerCaptureLost_EndsPanning()
    {
        // Arrange
        var canvas = new Canvas { Width = 200, Height = 200 };
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300,
            EnablePan = true,
            PanButton = ButtonName.Left,
            Child = canvas
        };
        var window = new Window { Content = zoomBorder };
        window.Show();

        // Start panning
        var pointerPressedEventArgs = new PointerPressedEventArgs(
            zoomBorder,
            new Pointer(1, PointerType.Mouse, true),
            zoomBorder,
            new Point(100, 75),
            0,
            new PointerPointProperties(RawInputModifiers.None, PointerUpdateKind.LeftButtonPressed),
            KeyModifiers.None)
        {
            RoutedEvent = InputElement.PointerPressedEvent
        };
        zoomBorder.RaiseEvent(pointerPressedEventArgs);

        Assert.Contains(":isPanning", zoomBorder.Classes);

        // Act - Simulate pointer capture lost
        var captureLostArgs = new PointerCaptureLostEventArgs(
            zoomBorder,
            new Pointer(1, PointerType.Mouse, true))
        {
            RoutedEvent = InputElement.PointerCaptureLostEvent
        };
        zoomBorder.RaiseEvent(captureLostArgs);

        // Assert - Panning should end
        Assert.DoesNotContain(":isPanning", zoomBorder.Classes);
    }

    // ===== EnablePan and EnableZoom Tests =====

    [AvaloniaFact]
    public void EnablePan_DefaultValue_IsTrue()
    {
        // Arrange & Act
        var zoomBorder = new ZoomBorder();

        // Assert
        Assert.True(zoomBorder.EnablePan);
    }

    [AvaloniaFact]
    public void EnableZoom_DefaultValue_IsTrue()
    {
        // Arrange & Act
        var zoomBorder = new ZoomBorder();

        // Assert
        Assert.True(zoomBorder.EnableZoom);
    }

    [AvaloniaFact]
    public void ZoomIn_WhenEnableZoomIsFalse_StillWorks()
    {
        // Arrange
        var canvas = new Canvas { Width = 200, Height = 200 };
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300,
            EnableZoom = false, // User interaction zoom disabled
            Child = canvas
        };
        var window = new Window { Content = zoomBorder };
        window.Show();

        var initialZoom = zoomBorder.ZoomX;

        // Act - Programmatic zoom should still work
        zoomBorder.ZoomIn();

        // Assert
        Assert.True(zoomBorder.ZoomX > initialZoom);
    }

    [AvaloniaFact]
    public void Pan_WhenEnablePanIsFalse_StillWorks()
    {
        // Arrange
        var canvas = new Canvas { Width = 200, Height = 200 };
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300,
            EnablePan = false, // User interaction pan disabled
            Child = canvas
        };
        var window = new Window { Content = zoomBorder };
        window.Show();

        var initialOffsetX = zoomBorder.OffsetX;

        // Act - Programmatic pan should still work
        zoomBorder.Pan(50, 50);

        // Assert
        Assert.NotEqual(initialOffsetX, zoomBorder.OffsetX);
    }
}
