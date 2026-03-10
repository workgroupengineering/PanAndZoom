---
title: "Input Simulators"
---

# Input Simulators

HeadlessTestingFramework offers both low-level and high-level input entry points.

## Touch

`Avalonia.HeadlessTestingFramework.TouchInputSimulator` is the lowest-level API and exposes direct touch lifecycle methods:

- `TouchDown(...)`
- `TouchMove(...)`
- `TouchUp(...)`
- `Tap(...)`
- `DoubleTap(...)`
- `Swipe(...)`
- `SimulatePinchZoom(...)`
- `SimulateTwoFingerPan(...)`

Use it when precise control over positions and timing matters.

## Keyboard

`Avalonia.HeadlessTestingFramework.KeyboardInputSimulator` covers individual keys, text typing, modifiers, and convenience shortcuts such as:

- `CtrlKey(...)`
- `Copy(...)`
- `Paste(...)`
- `SelectAll(...)`
- `ArrowUp()` through `ArrowRight()`

## Mouse

`Avalonia.HeadlessTestingFramework.MouseInputSimulator` is the direct path for click, drag, and wheel scenarios where pointer semantics matter more than touch semantics.

## Gesture Level APIs

`Avalonia.HeadlessTestingFramework.GestureSimulator` raises higher-level gesture events, while `Avalonia.HeadlessTestingFramework.MultiTouchTestHelperFactory` helps trigger recognizer-based pinch, pan, and rotation flows.

## Recommended Usage

| Scenario | Recommended API |
|---|---|
| Basic tap or drag | `TouchInputSimulator` |
| Keyboard navigation or shortcuts | `KeyboardInputSimulator` |
| Mouse-specific behavior | `MouseInputSimulator` |
| High-level gesture events | `GestureSimulator` |
| Actual recognizer-driven multi-touch | `MultiTouchTestHelperFactory` |
