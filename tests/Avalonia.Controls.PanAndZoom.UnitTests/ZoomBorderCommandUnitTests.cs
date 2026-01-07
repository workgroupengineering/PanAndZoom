// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using System.Reflection;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Xunit;

namespace Avalonia.Controls.PanAndZoom.UnitTests;

/// <summary>
/// Unit tests for ZoomBorderCommand internal class via reflection and ZoomBorder commands.
/// </summary>
public class ZoomBorderCommandUnitTests
{
    // Helper to get internal ZoomBorderCommand type
    private static Type GetZoomBorderCommandType()
    {
        var assembly = typeof(ZoomBorder).Assembly;
        return assembly.GetType("Avalonia.Controls.PanAndZoom.ZoomBorderCommand")!;
    }

    // Helper to create ZoomBorderCommand instance via reflection
    private static ICommand CreateZoomBorderCommand(Action execute, Func<bool>? canExecute = null)
    {
        var type = GetZoomBorderCommandType();
        var constructor = type.GetConstructor(new[] { typeof(Action), typeof(Func<bool>) });
        return (ICommand)constructor!.Invoke(new object?[] { execute, canExecute });
    }

    [AvaloniaFact]
    public void ZoomBorderCommand_Constructor_ThrowsOnNullExecute()
    {
        // Arrange & Act & Assert
        var type = GetZoomBorderCommandType();
        var constructor = type.GetConstructor(new[] { typeof(Action), typeof(Func<bool>) });
        
        var ex = Assert.Throws<TargetInvocationException>(() => 
            constructor!.Invoke(new object?[] { null, null }));
        Assert.IsType<ArgumentNullException>(ex.InnerException);
    }

    [AvaloniaFact]
    public void ZoomBorderCommand_CanExecute_WithCanExecuteFunc_ReturnsTrue()
    {
        // Arrange
        var command = CreateZoomBorderCommand(() => { }, () => true);

        // Act
        var result = command.CanExecute(null);

        // Assert
        Assert.True(result);
    }

    [AvaloniaFact]
    public void ZoomBorderCommand_CanExecute_WithCanExecuteFuncReturnsFalse_ReturnsFalse()
    {
        // Arrange
        var command = CreateZoomBorderCommand(() => { }, () => false);

        // Act
        var result = command.CanExecute(null);

        // Assert
        Assert.False(result);
    }

    [AvaloniaFact]
    public void ZoomBorderCommand_CanExecute_WithNullCanExecuteFunc_ReturnsTrue()
    {
        // Arrange
        var command = CreateZoomBorderCommand(() => { }, null);

        // Act
        var result = command.CanExecute(null);

        // Assert
        Assert.True(result);
    }

    [AvaloniaFact]
    public void ZoomBorderCommand_Execute_InvokesAction()
    {
        // Arrange
        var executed = false;
        var command = CreateZoomBorderCommand(() => executed = true, null);

        // Act
        command.Execute(null);

        // Assert
        Assert.True(executed);
    }

    [AvaloniaFact]
    public void ZoomBorderCommand_RaiseCanExecuteChanged_RaisesEvent()
    {
        // Arrange
        var command = CreateZoomBorderCommand(() => { }, () => true);
        var eventRaised = false;
        command.CanExecuteChanged += (s, e) => eventRaised = true;

        // Act - Call RaiseCanExecuteChanged via reflection
        var type = GetZoomBorderCommandType();
        var method = type.GetMethod("RaiseCanExecuteChanged");
        method!.Invoke(command, null);

        // Assert
        Assert.True(eventRaised);
    }

    [AvaloniaFact]
    public void ZoomBorderCommand_RaiseCanExecuteChanged_WithNoHandlers_DoesNotThrow()
    {
        // Arrange
        var command = CreateZoomBorderCommand(() => { }, () => true);

        // Act & Assert - Should not throw even with no handlers subscribed
        var type = GetZoomBorderCommandType();
        var method = type.GetMethod("RaiseCanExecuteChanged");
        var exception = Record.Exception(() => method!.Invoke(command, null));
        Assert.Null(exception);
    }

    [AvaloniaFact]
    public void ZoomBorderCommand_CanExecute_DynamicCanExecute_ReflectsChanges()
    {
        // Arrange
        var canExecuteValue = true;
        var command = CreateZoomBorderCommand(() => { }, () => canExecuteValue);

        // Act & Assert - Initial state
        Assert.True(command.CanExecute(null));

        // Change the value
        canExecuteValue = false;
        Assert.False(command.CanExecute(null));

        // Change back
        canExecuteValue = true;
        Assert.True(command.CanExecute(null));
    }

    // ===== ZoomBorder Command Tests via Public API =====

    [AvaloniaFact]
    public void ZoomInCommand_CanExecute_WithChild_ReturnsTrue()
    {
        // Arrange
        var canvas = new Canvas { Width = 200, Height = 200 };
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300,
            EnableZoom = true,
            Child = canvas
        };
        var window = new Window { Content = zoomBorder };
        window.Show();

        // Act
        var result = zoomBorder.ZoomInCommand.CanExecute(null);

        // Assert
        Assert.True(result);
    }

    [AvaloniaFact]
    public void ZoomInCommand_CanExecute_WithoutChild_ReturnsFalse()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300,
            EnableZoom = true
            // No child
        };
        var window = new Window { Content = zoomBorder };
        window.Show();

        // Act
        var result = zoomBorder.ZoomInCommand.CanExecute(null);

        // Assert
        Assert.False(result);
    }

    [AvaloniaFact]
    public void ZoomInCommand_CanExecute_WhenZoomDisabled_ReturnsFalse()
    {
        // Arrange
        var canvas = new Canvas { Width = 200, Height = 200 };
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300,
            EnableZoom = false,
            Child = canvas
        };
        var window = new Window { Content = zoomBorder };
        window.Show();

        // Act
        var result = zoomBorder.ZoomInCommand.CanExecute(null);

        // Assert
        Assert.False(result);
    }

    [AvaloniaFact]
    public void ZoomOutCommand_CanExecute_WithChild_ReturnsTrue()
    {
        // Arrange
        var canvas = new Canvas { Width = 200, Height = 200 };
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300,
            EnableZoom = true,
            Child = canvas
        };
        var window = new Window { Content = zoomBorder };
        window.Show();

        // Act
        var result = zoomBorder.ZoomOutCommand.CanExecute(null);

        // Assert
        Assert.True(result);
    }

    [AvaloniaFact]
    public void ResetCommand_CanExecute_WithChild_ReturnsTrue()
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

        // Act
        var result = zoomBorder.ResetCommand.CanExecute(null);

        // Assert
        Assert.True(result);
    }

    [AvaloniaFact]
    public void FitCommand_CanExecute_WithChild_ReturnsTrue()
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

        // Act
        var result = zoomBorder.FitCommand.CanExecute(null);

        // Assert
        Assert.True(result);
    }

    [AvaloniaFact]
    public void FillCommand_CanExecute_WithChild_ReturnsTrue()
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

        // Act
        var result = zoomBorder.FillCommand.CanExecute(null);

        // Assert
        Assert.True(result);
    }

    [AvaloniaFact]
    public void ToggleStretchCommand_CanExecute_WithChild_ReturnsTrue()
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

        // Act
        var result = zoomBorder.ToggleStretchCommand.CanExecute(null);

        // Assert
        Assert.True(result);
    }
}
