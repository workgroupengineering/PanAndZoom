---
title: "Custom Bounds, Resize, and Rotation"
---

# Custom Bounds, Resize, and Rotation

If the built-in behavior is close but not exact, `ZoomBorder` exposes virtual hooks for custom policy.

## Extensibility Points

- `GetContentBounds()`: return the effective content bounds used for custom constraints
- `ValidateTransform(Matrix newMatrix)`: veto proposed transforms
- `OnResized(Size oldSize, Size newSize)`: apply custom resize policy

Example:

```csharp
public class CustomZoomBorder : ZoomBorder
{
    protected override Rect GetContentBounds() => base.GetContentBounds();

    protected override bool ValidateTransform(Matrix newMatrix)
    {
        return base.ValidateTransform(newMatrix);
    }

    protected override void OnResized(Size oldSize, Size newSize)
    {
        base.OnResized(oldSize, newSize);
    }
}
```

## When To Override

- content bounds depend on domain data rather than only the child's measured bounds
- certain zoom or pan regions must be blocked
- resize behavior needs to preserve a custom anchor or workflow-specific invariant

## Rotation Considerations

Rotation state is configurable, but advanced rotation-heavy surfaces should validate how rotation interacts with bounds, serialization, and any custom overlay math before relying on it as a full scene-graph feature.
