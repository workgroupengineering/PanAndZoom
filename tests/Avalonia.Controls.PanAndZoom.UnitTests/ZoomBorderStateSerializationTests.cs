// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Xunit;

namespace Avalonia.Controls.PanAndZoom.UnitTests;

/// <summary>
/// Tests for ZoomBorder state serialization functionality.
/// </summary>
public class ZoomBorderStateSerializationTests
{
    [AvaloniaFact]
    public void ExportState_ReturnsZoomBorderState()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            Width = 400,
            Height = 300,
            ZoomSpeed = 1.5,
            EnablePan = true,
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

        // Act
        var state = zoomBorder.ExportState();

        // Assert
        Assert.NotNull(state);
        Assert.Equal(zoomBorder.ZoomSpeed, state.ZoomSpeed);
        Assert.Equal(zoomBorder.EnablePan, state.EnablePan);
        Assert.Equal(zoomBorder.EnableZoom, state.EnableZoom);
    }

    [AvaloniaFact]
    public void ExportState_CapturesMatrix()
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
        var state = zoomBorder.ExportState();

        // Assert
        Assert.NotNull(state);
        Assert.NotEqual(Matrix.Identity, state.Matrix);
    }

    [AvaloniaFact]
    public void ExportState_CapturesRotation()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            Rotation = 45.0
        };

        // Act
        var state = zoomBorder.ExportState();

        // Assert
        Assert.Equal(45.0, state.Rotation);
    }

    [AvaloniaFact]
    public void ExportState_CapturesStretchMode()
    {
        // Arrange
        var zoomBorder = new ZoomBorder
        {
            Stretch = StretchMode.Fill
        };

        // Act
        var state = zoomBorder.ExportState();

        // Assert
        Assert.Equal(StretchMode.Fill, state.Stretch);
    }

    [AvaloniaFact]
    public void ExportState_CapturesTimestamp()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();
        var beforeExport = DateTime.UtcNow;

        // Act
        var state = zoomBorder.ExportState();
        var afterExport = DateTime.UtcNow;

        // Assert
        Assert.True(state.Timestamp >= beforeExport);
        Assert.True(state.Timestamp <= afterExport);
    }

    [AvaloniaFact]
    public void ImportState_RestoresZoomSpeed()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();
        var state = new ZoomBorderState
        {
            ZoomSpeed = 2.0,
            Matrix = Matrix.Identity,
            Stretch = StretchMode.None,
            EnablePan = true,
            EnableZoom = true
        };

        // Act
        zoomBorder.ImportState(state);

        // Assert
        Assert.Equal(2.0, zoomBorder.ZoomSpeed);
    }

    [AvaloniaFact]
    public void ImportState_RestoresEnablePan()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();
        var state = new ZoomBorderState
        {
            EnablePan = false,
            Matrix = Matrix.Identity,
            Stretch = StretchMode.None
        };

        // Act
        zoomBorder.ImportState(state);

        // Assert
        Assert.False(zoomBorder.EnablePan);
    }

    [AvaloniaFact]
    public void ImportState_RestoresEnableZoom()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();
        var state = new ZoomBorderState
        {
            EnableZoom = false,
            Matrix = Matrix.Identity,
            Stretch = StretchMode.None
        };

        // Act
        zoomBorder.ImportState(state);

        // Assert
        Assert.False(zoomBorder.EnableZoom);
    }

    [AvaloniaFact]
    public void ImportState_RestoresRotation()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();
        var state = new ZoomBorderState
        {
            Rotation = 90.0,
            Matrix = Matrix.Identity,
            Stretch = StretchMode.None
        };

        // Act
        zoomBorder.ImportState(state);

        // Assert
        Assert.Equal(90.0, zoomBorder.Rotation);
    }

    [AvaloniaFact]
    public void ImportState_RestoresStretchMode()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();
        var state = new ZoomBorderState
        {
            Stretch = StretchMode.Uniform,
            Matrix = Matrix.Identity
        };

        // Act
        zoomBorder.ImportState(state);

        // Assert
        Assert.Equal(StretchMode.Uniform, zoomBorder.Stretch);
    }

    [AvaloniaFact]
    public void ImportState_RestoresZoomLimits()
    {
        // Arrange
        var zoomBorder = new ZoomBorder();
        var state = new ZoomBorderState
        {
            MinZoomX = 0.5,
            MaxZoomX = 10.0,
            MinZoomY = 0.5,
            MaxZoomY = 10.0,
            Matrix = Matrix.Identity,
            Stretch = StretchMode.None
        };

        // Act
        zoomBorder.ImportState(state);

        // Assert
        Assert.Equal(0.5, zoomBorder.MinZoomX);
        Assert.Equal(10.0, zoomBorder.MaxZoomX);
        Assert.Equal(0.5, zoomBorder.MinZoomY);
        Assert.Equal(10.0, zoomBorder.MaxZoomY);
    }

    [AvaloniaFact]
    public void ExportAndImportState_RoundTrip_PreservesState()
    {
        // Arrange
        var zoomBorder1 = new ZoomBorder
        {
            Width = 400,
            Height = 300,
            ZoomSpeed = 1.5,
            EnablePan = false,
            EnableZoom = true,
            Rotation = 45.0,
            Stretch = StretchMode.Fill
        };

        var childElement = new Border
        {
            Width = 200,
            Height = 150,
            Background = Brushes.Red
        };

        zoomBorder1.Child = childElement;
        var window = new Window { Content = zoomBorder1 };
        window.Show();

        zoomBorder1.Zoom(2.0, 100, 75);

        var zoomBorder2 = new ZoomBorder();

        // Act
        var state = zoomBorder1.ExportState();
        zoomBorder2.ImportState(state);

        // Assert
        Assert.Equal(zoomBorder1.ZoomSpeed, zoomBorder2.ZoomSpeed);
        Assert.Equal(zoomBorder1.EnablePan, zoomBorder2.EnablePan);
        Assert.Equal(zoomBorder1.EnableZoom, zoomBorder2.EnableZoom);
        Assert.Equal(zoomBorder1.Rotation, zoomBorder2.Rotation);
        Assert.Equal(zoomBorder1.Stretch, zoomBorder2.Stretch);
    }
}
