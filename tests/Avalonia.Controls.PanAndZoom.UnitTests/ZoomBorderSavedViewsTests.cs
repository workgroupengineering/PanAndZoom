// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Xunit;

namespace Avalonia.Controls.PanAndZoom.UnitTests;

/// <summary>
/// Tests for ZoomBorder saved views functionality.
/// </summary>
public class ZoomBorderSavedViewsTests
{
    [AvaloniaFact]
    public void SaveView_SavesCurrentViewState()
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

        zoomBorder.ZoomIn();

        // Act
        zoomBorder.SaveView("TestView", "Test description");

        // Assert
        var view = zoomBorder.GetSavedView("TestView");
        Assert.NotNull(view);
        Assert.Equal("TestView", view!.Value.Name);
        Assert.Equal("Test description", view.Value.Description);
    }

    [AvaloniaFact]
    public void RestoreView_RestoresSavedView()
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

        zoomBorder.ZoomIn();
        var savedZoom = zoomBorder.ZoomX;
        zoomBorder.SaveView("TestView");

        // Change the view
        zoomBorder.ResetMatrix();

        // Act
        var result = zoomBorder.RestoreView("TestView");

        // Assert
        Assert.True(result);
        Assert.Equal(savedZoom, zoomBorder.ZoomX, 2);
    }

    [AvaloniaFact]
    public void RestoreView_ReturnsFalseForNonExistentView()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();

        // Act
        var result = zoomBorder.RestoreView("NonExistent");

        // Assert
        Assert.False(result);
    }

    [AvaloniaFact]
    public void GetSavedViewNames_ReturnsAllSavedViewNames()
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

        zoomBorder.SaveView("View1");
        zoomBorder.SaveView("View2");
        zoomBorder.SaveView("View3");

        // Act
        var names = zoomBorder.GetSavedViewNames();

        // Assert
        Assert.Equal(3, names.Length);
        Assert.Contains("View1", names);
        Assert.Contains("View2", names);
        Assert.Contains("View3", names);
    }

    [AvaloniaFact]
    public void DeleteSavedView_RemovesView()
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

        zoomBorder.SaveView("TestView");

        // Act
        var result = zoomBorder.DeleteSavedView("TestView");

        // Assert
        Assert.True(result);
        Assert.Null(zoomBorder.GetSavedView("TestView"));
    }

    [AvaloniaFact]
    public void ClearSavedViews_RemovesAllViews()
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

        zoomBorder.SaveView("View1");
        zoomBorder.SaveView("View2");

        // Act
        zoomBorder.ClearSavedViews();

        // Assert
        Assert.Empty(zoomBorder.GetSavedViewNames());
    }
}
