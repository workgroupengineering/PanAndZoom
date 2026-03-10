---
title: "Samples"
---

# Samples

The sample application lives in `samples/AvaloniaDemo.Base` and `samples/AvaloniaDemo.Desktop`. It exposes the main control capabilities as tabs, which makes it the fastest way to validate behavior before writing application code.

## PanAndZoom Sample Areas

- Commands: built-in `ICommand` bindings and common toolbar actions
- Coordinate Conversion: `ViewportToContent`, `ContentToViewport`, and matrix-derived helpers
- Bounds and Callbacks: visible bounds, viewport bounds, and content-bound callbacks
- Keyboard and Reset: keyboard shortcuts, reset behavior, and focus requirements
- Zoom-to-Rectangle: fitting or exactly matching content rectangles
- Inertia and Gestures: gesture recognizers, pinch, and scroll interactions
- View History: back/forward navigation across saved viewport states
- Dynamic Zoom Limits: auto-calculated min and max zoom behavior
- Grid and Snap: grid parameters and `SnapToGrid(...)` helpers
- Rotation and Accessibility: rotation APIs, descriptions, and accessibility text
- Saved Views and State Serialization: named views plus `ExportState()` and `ImportState(...)`
- Resize Behavior and Gesture Fine Control: custom resize policies and gesture toggles

## Why The Sample Matters

- It shows the intended composition pattern for `ZoomBorder` in real XAML.
- It demonstrates feature combinations that are easy to miss by reading only API docs.
- It provides concrete scenarios that already map to unit tests in the repository.

## Related

- [Quickstart: ZoomBorder](getting-started/quickstart-zoom-border.md)
- [Guides](guides/readme.md)
- [Advanced](advanced/readme.md)
