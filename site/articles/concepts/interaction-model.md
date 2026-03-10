---
title: "Interaction Model"
---

# Interaction Model

`ZoomBorder` combines several input paths, each with its own enable flags and behavior settings.

## Pointer And Wheel

- dragging pans the content using the configured `PanButton`
- wheel input is controlled by `WheelBehavior`, `WheelWithCtrl`, `WheelWithShift`, `WheelZoomSensitivity`, and `WheelPanSensitivity`
- `EnablePan` and `EnableZoom` are the first-level switches

## Gestures

When `EnableGestures` is `true`, the control attaches pinch and scroll recognizers. Additional flags refine what a gesture is allowed to do:

- `EnableGestureZoom`
- `EnableGestureTranslation`
- `EnableGestureRotation`
- `EnableSimultaneousPanZoom`

The gesture path is useful on touch devices and in headless tests that use gesture recognizers rather than just pointer emulation.

## Keyboard

With `EnableKeyboardNavigation`, the control listens for built-in navigation and zoom shortcuts. That makes the control usable without custom command wiring, but the command properties are still available for toolbar and MVVM scenarios.

## Commands

`ZoomBorder` exposes `ICommand` properties such as `ZoomInCommand`, `ResetCommand`, `FitCommand`, and history navigation commands. These commands reflect control state and are the preferred way to surface interaction in view models.

## Animation

Most high-level methods accept an `animate` or `skipTransitions` argument. `EnableAnimations` and `AnimationDuration` determine whether transitions should be applied by default for command-driven view changes.
