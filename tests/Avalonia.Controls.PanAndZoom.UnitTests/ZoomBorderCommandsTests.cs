// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Xunit;

namespace Avalonia.Controls.PanAndZoom.UnitTests;

/// <summary>
/// Tests for ICommand implementations.
/// </summary>
public class ZoomBorderCommandsTests
{
    [AvaloniaFact]
    public void ZoomInCommand_Exists()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();

        // Act & Assert
        Assert.NotNull(zoomBorder.ZoomInCommand);
    }

    [AvaloniaFact]
    public void ZoomOutCommand_Exists()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();

        // Act & Assert
        Assert.NotNull(zoomBorder.ZoomOutCommand);
    }

    [AvaloniaFact]
    public void ResetCommand_Exists()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();

        // Act & Assert
        Assert.NotNull(zoomBorder.ResetCommand);
    }

    [AvaloniaFact]
    public void FitCommand_Exists()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();

        // Act & Assert
        Assert.NotNull(zoomBorder.FitCommand);
    }

    [AvaloniaFact]
    public void FillCommand_Exists()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();

        // Act & Assert
        Assert.NotNull(zoomBorder.FillCommand);
    }

    [AvaloniaFact]
    public void UniformCommand_Exists()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();

        // Act & Assert
        Assert.NotNull(zoomBorder.UniformCommand);
    }

    [AvaloniaFact]
    public void UniformToFillCommand_Exists()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();

        // Act & Assert
        Assert.NotNull(zoomBorder.UniformToFillCommand);
    }

    [AvaloniaFact]
    public void NavigateBackCommand_Exists()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();

        // Act & Assert
        Assert.NotNull(zoomBorder.NavigateBackCommand);
    }

    [AvaloniaFact]
    public void NavigateForwardCommand_Exists()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();

        // Act & Assert
        Assert.NotNull(zoomBorder.NavigateForwardCommand);
    }

    [AvaloniaFact]
    public void ToggleStretchCommand_Exists()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();

        // Act & Assert
        Assert.NotNull(zoomBorder.ToggleStretchCommand);
    }

    [AvaloniaFact]
    public void ZoomInCommand_Execute_ZoomsIn()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300,
            EnableZoom = true
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
        zoomBorder.ZoomInCommand.Execute(null);

        // Assert
        Assert.True(zoomBorder.ZoomX > initialZoom, "Zoom should increase after ZoomInCommand");
    }

    [AvaloniaFact]
    public void ZoomOutCommand_Execute_ZoomsOut()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300,
            EnableZoom = true
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
        var zoomedInLevel = zoomBorder.ZoomX;

        // Act
        zoomBorder.ZoomOutCommand.Execute(null);

        // Assert
        Assert.True(zoomBorder.ZoomX < zoomedInLevel, "Zoom should decrease after ZoomOutCommand");
    }

    [AvaloniaFact]
    public void ResetCommand_Execute_ResetsView()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300
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
        zoomBorder.ResetCommand.Execute(null);

        // Assert
        Assert.Equal(1.0, zoomBorder.ZoomX);
        Assert.Equal(1.0, zoomBorder.ZoomY);
        Assert.Equal(0.0, zoomBorder.OffsetX);
        Assert.Equal(0.0, zoomBorder.OffsetY);
    }

    [AvaloniaFact]
    public void FitCommand_Execute_FitsToViewport()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300,
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

        // Act
        zoomBorder.FitCommand.Execute(null);

        // Assert - Should apply AutoFit
        Assert.NotEqual(1.0, zoomBorder.ZoomX);
    }

    [AvaloniaFact]
    public void ToggleStretchCommand_Execute_TogglesStretchMode()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300,
            Stretch = StretchMode.None
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
        zoomBorder.ToggleStretchCommand.Execute(null);

        // Assert
        Assert.Equal(StretchMode.Fill, zoomBorder.Stretch);
    }

    [AvaloniaFact]
    public void NavigateBackCommand_CanExecute_FalseWhenNoHistory()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            EnableViewHistory = true
        };

        // Act & Assert
        Assert.False(zoomBorder.NavigateBackCommand.CanExecute(null), "Should not be able to navigate back without history");
    }

    [AvaloniaFact]
    public void NavigateBackCommand_CanExecute_TrueWhenHistoryExists()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300,
            EnableViewHistory = true,
            EnableZoom = true
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

        // Create history
        zoomBorder.ZoomIn();

        // Act & Assert
        Assert.True(zoomBorder.NavigateBackCommand.CanExecute(null), "Should be able to navigate back with history");
    }

    [AvaloniaFact]
    public void NavigateForwardCommand_CanExecute_FalseInitially()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            EnableViewHistory = true
        };

        // Act & Assert
        Assert.False(zoomBorder.NavigateForwardCommand.CanExecute(null), "Should not be able to navigate forward initially");
    }

    [AvaloniaFact]
    public void NavigateForwardCommand_CanExecute_TrueAfterNavigateBack()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300,
            EnableViewHistory = true,
            EnableZoom = true
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

        // Create history and navigate back
        zoomBorder.ZoomIn();
        zoomBorder.NavigateBack();

        // Act & Assert
        Assert.True(zoomBorder.NavigateForwardCommand.CanExecute(null), "Should be able to navigate forward after back");
    }

    [AvaloniaFact]
    public void NavigateBackCommand_Execute_NavigatesBack()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300,
            EnableViewHistory = true,
            EnableZoom = true
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
        zoomBorder.ZoomIn();

        // Act
        zoomBorder.NavigateBackCommand.Execute(null);

        // Assert
        Assert.Equal(initialZoom, zoomBorder.ZoomX, 2);
    }

    [AvaloniaFact]
    public void NavigateForwardCommand_Execute_NavigatesForward()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300,
            EnableViewHistory = true,
            EnableZoom = true
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

        zoomBorder.ZoomIn();
        var zoomedInLevel = zoomBorder.ZoomX;
        zoomBorder.NavigateBack();

        // Act
        zoomBorder.NavigateForwardCommand.Execute(null);

        // Assert
        Assert.Equal(zoomedInLevel, zoomBorder.ZoomX, 2);
    }

    [AvaloniaFact]
    public void AllCommands_CanBeUsedMultipleTimes()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300,
            EnableZoom = true,
            Stretch = StretchMode.None
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

        // Act - Execute commands multiple times
        zoomBorder.ZoomInCommand.Execute(null);
        zoomBorder.ZoomInCommand.Execute(null);
        zoomBorder.ZoomOutCommand.Execute(null);
        zoomBorder.ResetCommand.Execute(null);
        zoomBorder.ToggleStretchCommand.Execute(null);
        zoomBorder.ToggleStretchCommand.Execute(null);

        // Assert - No exceptions should be thrown
        Assert.NotNull(zoomBorder);
    }
}
