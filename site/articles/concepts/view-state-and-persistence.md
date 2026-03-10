---
title: "View State and Persistence"
---

# View State and Persistence

PanAndZoom offers three levels of state management.

## Immediate State

Current state is available through properties such as:

- `ZoomX`, `ZoomY`
- `OffsetX`, `OffsetY`
- `Rotation`
- `Stretch`

These are the values you bind into status bars or diagnostics panels.

## View History

When `EnableViewHistory` is enabled, navigation APIs record viewport changes:

- `NavigateBack(...)`
- `NavigateForward(...)`
- `ClearViewHistory()`
- `CanNavigateBack`
- `CanNavigateForward`

This is intended for undo-like viewport navigation rather than full application undo stacks.

## Named Views And Serialized State

Two features support longer-lived persistence:

- named views: `SaveView(...)`, `RestoreView(...)`, `GetSavedViews()`
- serialized state: `ExportState()` and `ImportState(...)`

`Avalonia.Controls.PanAndZoom.ZoomBorderState` is the broadest persistence payload and is the right choice when you need to store or transmit a complete viewport configuration.

## When To Use Each

- Use current properties for UI display and lightweight telemetry.
- Use view history for back/forward navigation inside a session.
- Use saved views for user-defined bookmarks.
- Use serialized state for persistence between runs or for collaboration workflows.
