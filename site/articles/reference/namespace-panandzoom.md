---
title: "Namespace Avalonia.Controls.PanAndZoom"
---

# Namespace Avalonia.Controls.PanAndZoom

This namespace contains the shipping control, its state objects, event args, and supporting enums.

## Core Types

| Type | Kind | Primary article |
|---|---|---|
| `Avalonia.Controls.PanAndZoom.ZoomBorder` | Control | [getting-started/quickstart-zoom-border.md](../getting-started/quickstart-zoom-border.md) |
| `Avalonia.Controls.PanAndZoom.MatrixHelper` | Static class | [concepts/transform-and-coordinate-spaces.md](../concepts/transform-and-coordinate-spaces.md) |
| `Avalonia.Controls.PanAndZoom.ZoomBorderState` | Class | [concepts/view-state-and-persistence.md](../concepts/view-state-and-persistence.md) |
| `Avalonia.Controls.PanAndZoom.ViewState` | Struct | [concepts/view-state-and-persistence.md](../concepts/view-state-and-persistence.md) |
| `Avalonia.Controls.PanAndZoom.SavedView` | Struct | [concepts/view-state-and-persistence.md](../concepts/view-state-and-persistence.md) |

## Enums

| Type | Purpose |
|---|---|
| `Avalonia.Controls.PanAndZoom.ButtonName` | Selects the mouse button used for panning |
| `Avalonia.Controls.PanAndZoom.StretchMode` | Chooses fit behavior for content |
| `Avalonia.Controls.PanAndZoom.ContentBoundsMode` | Constrains how content can move inside the viewport |
| `Avalonia.Controls.PanAndZoom.ResizeBehaviorMode` | Controls resize reactions |
| `Avalonia.Controls.PanAndZoom.WheelBehaviorMode` | Maps wheel input to zoom or panning |
| `Avalonia.Controls.PanAndZoom.DoubleClickZoomMode` | Controls double-click zoom behavior |
| `Avalonia.Controls.PanAndZoom.ZoomIndicatorPosition` | Positions custom zoom indicators |

## Event Args And Delegates

| Type | Typical use |
|---|---|
| `Avalonia.Controls.PanAndZoom.ZoomChangedEventArgs` and `Avalonia.Controls.PanAndZoom.ZoomChangedEventHandler` | Listen for viewport zoom and offset changes |
| `Avalonia.Controls.PanAndZoom.ZoomEventArgs` and `Avalonia.Controls.PanAndZoom.ZoomEventHandler` | Observe zoom-specific operations |
| `Avalonia.Controls.PanAndZoom.PanEventArgs` and `Avalonia.Controls.PanAndZoom.PanEventHandler` | Observe pan operations |
| `Avalonia.Controls.PanAndZoom.GestureEventArgs` and `Avalonia.Controls.PanAndZoom.GestureEventHandler` | Observe gesture-driven actions |
| `Avalonia.Controls.PanAndZoom.MatrixChangedEventArgs` and `Avalonia.Controls.PanAndZoom.MatrixChangedEventHandler` | React to transform-matrix updates |
| `Avalonia.Controls.PanAndZoom.StretchModeChangedEventArgs` and `Avalonia.Controls.PanAndZoom.StretchModeChangedEventHandler` | Track stretch-mode transitions |
