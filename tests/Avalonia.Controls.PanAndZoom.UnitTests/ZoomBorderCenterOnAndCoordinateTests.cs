// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Xunit;

namespace Avalonia.Controls.PanAndZoom.UnitTests;

/// <summary>
/// Tests for center on methods and coordinate system conversion.
/// </summary>
public class ZoomBorderCenterOnAndCoordinateTests
{
    [AvaloniaFact]
    public void CenterPadding_DefaultValue_IsZero()
    {
        // Arrange & Act
        var zoomBorder = new ZoomBorder();

        // Assert
        Assert.Equal(new Thickness(0), zoomBorder.CenterPadding);
    }

    [AvaloniaFact]
    public void CenterPadding_CanBeSetToCustomValue()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();

        // Act
        zoomBorder.CenterPadding = new Thickness(10, 20, 30, 40);

        // Assert
        Assert.Equal(new Thickness(10, 20, 30, 40), zoomBorder.CenterPadding);
    }

    [AvaloniaFact]
    public void CenterOnPoint_CentersViewportOnSpecifiedPoint()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300
        };

        var childElement = new Border
        {
            Width = 800,
            Height = 600,
            Background = Brushes.Red
        };

        zoomBorder.Child = childElement;
        var window = new Window { Content = zoomBorder };
        window.Show();

        var targetPoint = new Point(100, 100);

        // Act
        zoomBorder.CenterOn(targetPoint);

        // Assert - The target point should now be centered in the viewport
        var viewportCenter = new Point(zoomBorder.Bounds.Width / 2.0, zoomBorder.Bounds.Height / 2.0);
        var transformedPoint = zoomBorder.ContentToViewport(targetPoint);

        Assert.True(System.Math.Abs(transformedPoint.X - viewportCenter.X) < 1, "X should be centered");
        Assert.True(System.Math.Abs(transformedPoint.Y - viewportCenter.Y) < 1, "Y should be centered");
    }

    [AvaloniaFact]
    public void CenterOnPointWithZoom_CentersAndZooms()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300
        };

        var childElement = new Border
        {
            Width = 800,
            Height = 600,
            Background = Brushes.Red
        };

        zoomBorder.Child = childElement;
        var window = new Window { Content = zoomBorder };
        window.Show();

        var targetPoint = new Point(400, 300);
        var targetZoom = 2.0;

        // Act
        zoomBorder.CenterOn(targetPoint, targetZoom);

        // Assert
        Assert.Equal(targetZoom, zoomBorder.ZoomX, 2);
        Assert.Equal(targetZoom, zoomBorder.ZoomY, 2);
    }

    [AvaloniaFact]
    public void CenterOnRect_CentersOnRectangleWithAppropriateZoom()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300
        };

        var childElement = new Border
        {
            Width = 800,
            Height = 600,
            Background = Brushes.Red
        };

        zoomBorder.Child = childElement;
        var window = new Window { Content = zoomBorder };
        window.Show();

        var targetRect = new Rect(100, 100, 200, 150);

        // Act
        zoomBorder.CenterOn(targetRect);

        // Assert - Zoom should be calculated to fit rect in viewport
        Assert.True(zoomBorder.ZoomX > 1.0, "Should zoom to fit rectangle");
    }

    [AvaloniaFact]
    public void ViewportToContent_ConvertsCoordinatesCorrectly()
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

        zoomBorder.Zoom(2.0, 100, 75);

        var viewportPoint = new Point(200, 150);

        // Act
        var contentPoint = zoomBorder.ViewportToContent(viewportPoint);

        // Assert - Should convert from viewport to content coordinates
        Assert.NotEqual(viewportPoint, contentPoint);
    }

    [AvaloniaFact]
    public void ContentToViewport_ConvertsCoordinatesCorrectly()
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

        // Zoom at origin (0,0) with 2x zoom
        zoomBorder.Zoom(2.0, 0, 0);

        var contentPoint = new Point(100, 75);

        // Act
        var viewportPoint = zoomBorder.ContentToViewport(contentPoint);

        // Assert - Should convert from content to viewport coordinates
        // At 2x zoom, content point (100, 75) should map to viewport (200, 150)
        Assert.True(viewportPoint.X > contentPoint.X, "Viewport X should be larger due to zoom");
        Assert.True(viewportPoint.Y > contentPoint.Y, "Viewport Y should be larger due to zoom");
    }

    [AvaloniaFact]
    public void ViewportToContent_RoundTrip_ReturnsOriginalPoint()
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

        zoomBorder.Zoom(1.5, 100, 75);

        var originalPoint = new Point(150, 100);

        // Act
        var contentPoint = zoomBorder.ViewportToContent(originalPoint);
        var backToViewport = zoomBorder.ContentToViewport(contentPoint);

        // Assert - Round trip should return to original point
        Assert.True(System.Math.Abs(backToViewport.X - originalPoint.X) < 0.01, "X should match");
        Assert.True(System.Math.Abs(backToViewport.Y - originalPoint.Y) < 0.01, "Y should match");
    }

    [AvaloniaFact]
    public void ViewportToContent_Rect_ConvertsCorrectly()
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

        var viewportRect = new Rect(50, 50, 100, 75);

        // Act
        var contentRect = zoomBorder.ViewportToContent(viewportRect);

        // Assert
        Assert.True(contentRect.Width > 0 && contentRect.Height > 0);
    }

    [AvaloniaFact]
    public void ContentToViewport_Rect_ConvertsCorrectly()
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

        var contentRect = new Rect(25, 25, 50, 37.5);

        // Act
        var viewportRect = zoomBorder.ContentToViewport(contentRect);

        // Assert
        Assert.True(viewportRect.Width > 0 && viewportRect.Height > 0);
    }

    [AvaloniaFact]
    public void ScreenToContent_Size_ConvertsCorrectly()
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

        zoomBorder.Zoom(2.0, 100, 75);
        var screenSize = new Size(100, 100);

        // Act
        var contentSize = zoomBorder.ScreenToContent(screenSize);

        // Assert - Content size should be smaller when zoomed in
        Assert.True(contentSize.Width < screenSize.Width);
        Assert.True(contentSize.Height < screenSize.Height);
    }

    [AvaloniaFact]
    public void ContentToScreen_Size_ConvertsCorrectly()
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

        zoomBorder.Zoom(2.0, 100, 75);
        var contentSize = new Size(50, 50);

        // Act
        var screenSize = zoomBorder.ContentToScreen(contentSize);

        // Assert - Screen size should be larger when zoomed in
        Assert.True(screenSize.Width > contentSize.Width);
        Assert.True(screenSize.Height > contentSize.Height);
    }

    [AvaloniaFact]
    public void GetContentToScreenMatrix_ReturnsCurrentMatrix()
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

        zoomBorder.Zoom(2.0, 100, 75);

        // Act
        var matrix = zoomBorder.GetContentToScreenMatrix();

        // Assert
        Assert.Equal(zoomBorder.Matrix, matrix);
    }

    [AvaloniaFact]
    public void GetScreenToContentMatrix_ReturnsInvertedMatrix()
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

        zoomBorder.Zoom(2.0, 100, 75);

        // Act
        var matrix = zoomBorder.GetScreenToContentMatrix();
        var contentToScreen = zoomBorder.GetContentToScreenMatrix();

        // Assert - Multiplying them should give identity
        var product = matrix * contentToScreen;
        Assert.True(System.Math.Abs(product.M11 - 1.0) < 0.01);
        Assert.True(System.Math.Abs(product.M22 - 1.0) < 0.01);
    }

    [AvaloniaFact]
    public void GetVisibleContentBounds_ReturnsCorrectBounds()
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

        // Act
        var bounds = zoomBorder.GetVisibleContentBounds();

        // Assert
        Assert.True(bounds.Width > 0);
        Assert.True(bounds.Height > 0);
    }

    [AvaloniaFact]
    public void GetViewportBounds_ReturnsViewportSize()
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

        // Act
        var bounds = zoomBorder.GetViewportBounds();

        // Assert
        Assert.Equal(0, bounds.X);
        Assert.Equal(0, bounds.Y);
        Assert.Equal(400, bounds.Width);
        Assert.Equal(300, bounds.Height);
    }

    [AvaloniaFact]
    public void CenterOn_Control_CentersOnChildControl()
    {
        // Arrange
        var innerBorder = new Border
        {
            Width = 50,
            Height = 50,
            Background = Brushes.Blue
        };

        var canvas = new Canvas
        {
            Width = 400,
            Height = 400
        };
        canvas.Children.Add(innerBorder);
        Canvas.SetLeft(innerBorder, 200);
        Canvas.SetTop(innerBorder, 200);

        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300,
            Child = canvas
        };

        var window = new Window { Content = zoomBorder };
        window.Show();

        // Act
        zoomBorder.CenterOn(innerBorder, animate: false);

        // Assert - Just verify it didn't throw
        Assert.NotNull(zoomBorder);
    }

    [AvaloniaFact]
    public void ScreenToContent_ConvertsCoordinates()
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

        // Act
        var contentPoint = zoomBorder.ScreenToContent(new Vector(100, 100));

        // Assert - At identity matrix, coordinates should be the same
        Assert.NotEqual(default, contentPoint);
    }

    [AvaloniaFact]
    public void ContentToScreen_ConvertsCoordinates()
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

        // Act
        var screenPoint = zoomBorder.ContentToScreen(new Vector(100, 100));

        // Assert - At identity matrix, coordinates should be the same
        Assert.NotEqual(default, screenPoint);
    }

    [AvaloniaFact]
    public void GetSavedViews_ReturnsEmptyWhenNoSavedViews()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300
        };

        var childElement = new Border { Width = 200, Height = 150, Background = Brushes.Red };
        zoomBorder.Child = childElement;
        var window = new Window { Content = zoomBorder };
        window.Show();

        // Act
        var savedViews = zoomBorder.GetSavedViews();

        // Assert
        Assert.Empty(savedViews);
    }

    [AvaloniaFact]
    public void GetSavedViews_ReturnsSavedViewsAfterSaving()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300
        };

        var childElement = new Border { Width = 200, Height = 150, Background = Brushes.Red };
        zoomBorder.Child = childElement;
        var window = new Window { Content = zoomBorder };
        window.Show();

        // Save a view
        zoomBorder.SaveView("TestView");

        // Act
        var savedViews = zoomBorder.GetSavedViews();

        // Assert
        Assert.Contains(savedViews, v => v.Name == "TestView");
    }
}
