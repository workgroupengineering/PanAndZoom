// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Xunit;

namespace Avalonia.Controls.PanAndZoom.UnitTests;

/// <summary>
/// Tests for view history and undo/redo functionality.
/// </summary>
public class ZoomBorderViewHistoryTests
{
    [AvaloniaFact]
    public void EnableViewHistory_DefaultValue_IsTrue()
    {
        // Arrange & Act
        var zoomBorder = new ZoomBorder();

        // Assert
        Assert.True(zoomBorder.EnableViewHistory);
    }

    [AvaloniaFact]
    public void EnableViewHistory_CanBeSetToFalse()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();

        // Act
        zoomBorder.EnableViewHistory = false;

        // Assert
        Assert.False(zoomBorder.EnableViewHistory);
    }

    [AvaloniaFact]
    public void ViewHistorySize_DefaultValue_Is50()
    {
        // Arrange & Act
        var zoomBorder = new ZoomBorder();

        // Assert
        Assert.Equal(50, zoomBorder.ViewHistorySize);
    }

    [AvaloniaFact]
    public void ViewHistorySize_CanBeSetToCustomValue()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();

        // Act
        zoomBorder.ViewHistorySize = 100;

        // Assert
        Assert.Equal(100, zoomBorder.ViewHistorySize);
    }

    [AvaloniaFact]
    public void CanNavigateBack_InitiallyFalse()
    {
        // Arrange & Act
        var zoomBorder = new ZoomBorder();

        // Assert
        Assert.False(zoomBorder.CanNavigateBack);
    }

    [AvaloniaFact]
    public void CanNavigateForward_InitiallyFalse()
    {
        // Arrange & Act
        var zoomBorder = new ZoomBorder();

        // Assert
        Assert.False(zoomBorder.CanNavigateForward);
    }

    [AvaloniaFact]
    public void ViewHistory_AllPropertiesCanBeSetTogether()
    {
        // Arrange & Act
        var zoomBorder = new ZoomBorder
        {
            EnableViewHistory = false,
            ViewHistorySize = 25
        };

        // Assert
        Assert.False(zoomBorder.EnableViewHistory);
        Assert.Equal(25, zoomBorder.ViewHistorySize);
    }

    [AvaloniaFact]
    public void NavigateBack_AfterZoom_RestoresPreviousState()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300,
            EnableViewHistory = true,
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

        var initialZoom = zoomBorder.ZoomX;

        // Zoom in
        zoomBorder.ZoomIn();
        var zoomedInLevel = zoomBorder.ZoomX;

        Assert.True(zoomBorder.CanNavigateBack, "Should be able to navigate back after zoom");

        // Act
        zoomBorder.NavigateBack();

        // Assert
        Assert.Equal(initialZoom, zoomBorder.ZoomX, 2);
        Assert.True(zoomBorder.ZoomX < zoomedInLevel, "Should restore previous zoom level");
        Assert.True(zoomBorder.CanNavigateForward, "Should be able to navigate forward after back");
    }

    [AvaloniaFact]
    public void NavigateForward_AfterBackNavigation_RestoresLaterState()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300,
            EnableViewHistory = true,
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

        // Zoom in
        zoomBorder.ZoomIn();
        var zoomedInLevel = zoomBorder.ZoomX;

        // Navigate back
        zoomBorder.NavigateBack();

        // Act
        zoomBorder.NavigateForward();

        // Assert
        Assert.Equal(zoomedInLevel, zoomBorder.ZoomX, 2);
        Assert.False(zoomBorder.CanNavigateForward, "Should not be able to navigate forward at latest state");
    }

    [AvaloniaFact]
    public void ViewHistory_MultipleNavigations_MaintainsCorrectHistory()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300,
            EnableViewHistory = true,
            EnableZoom = true,
            EnablePan = true,
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

        // Perform multiple operations
        var state1Zoom = zoomBorder.ZoomX;
        zoomBorder.ZoomIn();
        var state2Zoom = zoomBorder.ZoomX;
        zoomBorder.ZoomIn();
        var state3Zoom = zoomBorder.ZoomX;

        // Act & Assert - Navigate back twice
        zoomBorder.NavigateBack();
        Assert.Equal(state2Zoom, zoomBorder.ZoomX, 2);

        zoomBorder.NavigateBack();
        Assert.Equal(state1Zoom, zoomBorder.ZoomX, 2);

        // At the initial state (state1Zoom), cannot navigate back further
        Assert.False(zoomBorder.CanNavigateBack, "Should not be able to navigate back past initial state");
        Assert.True(zoomBorder.CanNavigateForward, "Should be able to navigate forward");

        // Navigate forward twice
        zoomBorder.NavigateForward();
        Assert.Equal(state2Zoom, zoomBorder.ZoomX, 2);

        zoomBorder.NavigateForward();
        Assert.Equal(state3Zoom, zoomBorder.ZoomX, 2);

        Assert.True(zoomBorder.CanNavigateBack, "Should be able to navigate back");
        Assert.False(zoomBorder.CanNavigateForward, "Should not be able to navigate forward beyond latest state");
    }

    [AvaloniaFact]
    public void ClearViewHistory_RemovesAllHistory()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300,
            EnableViewHistory = true,
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

        // Add some history
        zoomBorder.ZoomIn();
        zoomBorder.ZoomIn();
        Assert.True(zoomBorder.CanNavigateBack, "Should have history before clear");

        // Act
        zoomBorder.ClearViewHistory();

        // Assert
        Assert.False(zoomBorder.CanNavigateBack, "Should not be able to navigate back after clear");
        Assert.False(zoomBorder.CanNavigateForward, "Should not be able to navigate forward after clear");
    }

    [AvaloniaFact]
    public void ViewHistory_NewOperationAfterBack_ClearsForwardHistory()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300,
            EnableViewHistory = true,
            EnableZoom = true,
            EnablePan = true,
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

        // Create history
        zoomBorder.ZoomIn();
        zoomBorder.ZoomIn();

        // Navigate back
        zoomBorder.NavigateBack();
        Assert.True(zoomBorder.CanNavigateForward, "Should be able to navigate forward after back");

        // Act - Perform new operation
        zoomBorder.Pan(10, 10);

        // Assert - Forward history should be cleared
        Assert.False(zoomBorder.CanNavigateForward, "Forward history should be cleared after new operation");
        Assert.True(zoomBorder.CanNavigateBack, "Should still be able to navigate back");
    }

    [AvaloniaFact]
    public void ViewHistory_DisabledWhenEnableViewHistoryIsFalse()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300,
            EnableViewHistory = false,
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

        // Act - Perform operations
        zoomBorder.ZoomIn();
        zoomBorder.ZoomIn();

        // Assert - No history should be recorded
        Assert.False(zoomBorder.CanNavigateBack, "Should not record history when disabled");
    }

    [AvaloniaFact]
    public void ViewHistoryChanged_EventRaisedOnNavigateBack()
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

        var eventRaised = false;
        zoomBorder.ViewHistoryChanged += (sender, args) => eventRaised = true;

        // Act
        zoomBorder.NavigateBack();

        // Assert
        Assert.True(eventRaised, "ViewHistoryChanged event should be raised on NavigateBack");
    }

    [AvaloniaFact]
    public void ViewHistoryChanged_EventRaisedOnNavigateForward()
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
        zoomBorder.NavigateBack();

        var eventRaised = false;
        zoomBorder.ViewHistoryChanged += (sender, args) => eventRaised = true;

        // Act
        zoomBorder.NavigateForward();

        // Assert
        Assert.True(eventRaised, "ViewHistoryChanged event should be raised on NavigateForward");
    }

    [AvaloniaFact]
    public void ViewHistoryChanged_EventRaisedOnClearHistory()
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

        var eventRaised = false;
        zoomBorder.ViewHistoryChanged += (sender, args) => eventRaised = true;

        // Act
        zoomBorder.ClearViewHistory();

        // Assert
        Assert.True(eventRaised, "ViewHistoryChanged event should be raised on ClearViewHistory");
    }
}
