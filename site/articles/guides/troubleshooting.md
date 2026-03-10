---
title: "Troubleshooting"
---

# Troubleshooting

## Keyboard shortcuts do not work

- Ensure `Focusable="True"` on `ZoomBorder`.
- Ensure the control actually has focus when keys are pressed.
- Verify `EnableKeyboardNavigation` is enabled.

## Pointer dragging does not pan

- Check `EnablePan`.
- Verify `PanButton` matches the button you are pressing.
- If the child handles pointer input aggressively, confirm events still bubble to the `ZoomBorder`.

## Wheel input behaves unexpectedly inside a `ScrollViewer`

- Review `WheelBehavior`, `WheelWithCtrl`, and `WheelWithShift`.
- If the control lives inside a `ScrollViewer`, also review [ScrollViewer and Logical Scroll](../advanced/scrollviewer-and-logical-scroll.md).

## Gesture tests do not fire pinch or scroll handlers

- Register gesture handlers before `window.Show()`.
- Prefer `MultiTouchTestHelperFactory` or `GestureSimulator` when you need recognizer-aware tests.

## Grid or zoom indicator properties are set, but nothing is rendered

- These properties configure state only.
- Rendering of custom grids and zoom indicators must be supplied by the host application.

## Saved or serialized state is not restoring the exact expected result

- Check whether bounds or resize behavior is adjusting the imported matrix.
- Confirm the target content size matches the original state context.
