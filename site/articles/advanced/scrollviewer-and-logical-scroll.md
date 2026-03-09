---
title: "ScrollViewer and Logical Scroll"
---

# ScrollViewer and Logical Scroll

`ZoomBorder` implements `ILogicalScrollable` so it can participate in Avalonia scrolling infrastructure instead of fighting it.

## Why This Matters

Embedding zoomable content inside a `ScrollViewer` is common, but naive implementations can create feedback loops between viewport transforms and host scroll offsets.

`ZoomBorder` addresses that through:

- the `ILogicalScrollable` implementation in `ZoomBorder.ILogicalScrollable.cs`
- `CalculateScrollable(...)` for computing extent, viewport, and offset from source bounds and the active matrix

## Practical Guidance

- Put a `ScrollViewer` around the control only when you want scrollbars or host-level scrolling semantics.
- Review wheel configuration carefully; wheel input may need to pan instead of zoom in some host layouts.
- Use the existing unit tests around logical scrolling as the baseline behavior contract when modifying this area.

## Related APIs

- `Avalonia.Controls.PanAndZoom.ZoomBorder.CalculateScrollable(Avalonia.Rect,Avalonia.Size,Avalonia.Media.Transformation.Matrix,Avalonia.Size@,Avalonia.Size@,Avalonia.Vector@)`
- `BringIntoView(...)` behavior through the scrollable contract
- [Bounds, Wheel, and Resize](../guides/bounds-wheel-and-resize.md)
