---
title: "Transform and Coordinate Spaces"
---

# Transform and Coordinate Spaces

`ZoomBorder` maintains a transform matrix that maps content coordinates into viewport coordinates. Most higher-level APIs are thin wrappers over that model.

## The Three Views Of Space

- content space: the original coordinate system of the child control
- viewport space: the visible region inside the `ZoomBorder`
- screen-like vectors and sizes: transformed measurements derived from the active matrix

## Core Conversion APIs

Use these methods instead of manually duplicating matrix math:

- `Avalonia.Controls.PanAndZoom.ZoomBorder.ViewportToContent(Avalonia.Point)`
- `Avalonia.Controls.PanAndZoom.ZoomBorder.ContentToViewport(Avalonia.Point)`
- `Avalonia.Controls.PanAndZoom.ZoomBorder.ViewportToContent(Avalonia.Rect)`
- `Avalonia.Controls.PanAndZoom.ZoomBorder.ContentToViewport(Avalonia.Rect)`
- `Avalonia.Controls.PanAndZoom.ZoomBorder.GetContentToScreenMatrix`
- `Avalonia.Controls.PanAndZoom.ZoomBorder.GetScreenToContentMatrix`

These become important when:

- zooming to a region selected in viewport space
- showing overlays aligned with content coordinates
- translating pointer positions into domain coordinates on a diagram or image

## Matrix Helpers

`Avalonia.Controls.PanAndZoom.MatrixHelper` contains reusable helpers such as:

- `Translate(...)`
- `ScaleAt(...)`
- `Rotation(...)`
- `TransformPoint(...)`

It is useful when you build custom overlays or test matrix calculations separately from the control.

## Visible Area Queries

Two methods are especially important for viewport-aware logic:

- `Avalonia.Controls.PanAndZoom.ZoomBorder.GetVisibleContentBounds`
- `Avalonia.Controls.PanAndZoom.ZoomBorder.GetViewportBounds`

They allow item culling, overlay rendering, and "jump to region" UX without duplicating coordinate conversion code.
