---
title: "Centering and Zoom to Rectangle"
---

# Centering and Zoom to Rectangle

Several APIs move the viewport toward interesting content.

## Center On Overloads

Use `CenterOn(...)` when you already know the focal point:

- `CenterOn(Point point, bool animate = true)`
- `CenterOn(Point point, double zoom, bool animate = true)`
- `CenterOn(Rect rect, bool animate = true)`
- `CenterOn(Control element, bool animate = true)`

This is ideal for:

- jumping to a selected node on a diagram
- focusing on a search result
- recentering after domain-specific navigation

## Zoom To Rectangle

Use `ZoomToRectangle(...)` when the selected content region should fill the viewport with optional padding:

```csharp
zoomBorder.ZoomToRectangle(new Rect(100, 100, 200, 150), padding: new Thickness(20));
```

Use `ZoomToRectangleExact(...)` when you need to fit a content rectangle into a specific viewport rectangle rather than the whole control.

## Padding And Bounds

`CenterPadding` influences how much visual space should remain around centered content. `BoundsMode` still applies after movement, so a requested center can be adjusted when bounds enforcement is active.

## Related APIs

- `GetVisibleContentBounds()`
- `ContentToViewport(...)`
- `ViewportToContent(...)`
- `GetVisiblePortion(Rect rect)`
