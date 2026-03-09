---
title: "Rotation, Grid, and Visibility"
---

# Rotation, Grid, and Visibility

These features are optional, but they matter in editors, diagramming tools, and debugging overlays.

## Rotation

Rotation state is exposed by:

- `Rotate(...)`
- `RotateAt(...)`
- `ResetRotation(...)`
- `SnapRotation()`
- `Rotation`
- `MinRotation`, `MaxRotation`
- `EnableRotationSnapping`, `RotationSnapAngle`

The current implementation provides rotation state management and constraints. If your UX depends on deeper rotation customization, treat this as an advanced integration point rather than a complete 2D design surface out of the box.

## Grid And Snap

Grid-related properties configure a logical grid model:

- `ShowGrid`
- `GridSize`
- `GridBrush`
- `GridThickness`
- `GridOpacity`
- `MajorGridInterval`
- `MajorGridBrush`
- `MajorGridThickness`
- `EnableSnapToGrid`

The helper methods `SnapToGrid(double)`, `SnapToGrid(Point)`, and `SnapToGrid(Rect)` are implemented directly. Grid drawing itself is left to the host application.

## Visibility Queries

Use viewport queries to cull overlays or lazily render expensive content:

- `IsRectangleVisible(Rect rect)`
- `IsPointVisible(Point point)`
- `GetVisiblePortion(Rect rect)`

## Accessibility And Indicators

`UpdateAccessibilityDescriptions()` refreshes screen-reader-facing descriptions. `GetZoomIndicatorText()` returns formatted indicator text for any custom overlay or status element.
