---
title: "Introduction"
---

# Introduction

`PanAndZoom` is an Avalonia repository with two complementary libraries:

- `PanAndZoom`: the `ZoomBorder` control, matrix helpers, commands, bounds logic, and view-state APIs for pan-and-zoom experiences.
- `HeadlessTestingFramework`: input simulation, tree inspection, recording, and Appium-style helpers for validating Avalonia controls in automated tests.

Together they cover both sides of interactive UI work:

- build zoomable, pannable canvases, diagram surfaces, and image viewers
- persist view state, expose keyboard shortcuts, and constrain viewport behavior
- test touch, wheel, keyboard, and gesture scenarios without a full desktop session
- capture frames or turn headless recordings into video artifacts for diagnostics

## Start With

- [Getting Started Overview](getting-started/overview.md) if you need a package-level entry point
- [Quickstart: ZoomBorder](getting-started/quickstart-zoom-border.md) to wire the control into an Avalonia view
- [Quickstart: Headless Testing](getting-started/quickstart-headless-testing.md) to drive gestures in `AvaloniaFact` tests
- [API Documentation](../api/index.md) for member-level lookup

## Repository Layout

- `src/PanAndZoom`: shipping control package
- `src/HeadlessTestingFramework`: shipping testing package
- `samples/AvaloniaDemo.Base` and `samples/AvaloniaDemo.Desktop`: interactive sample app
- `tests/Avalonia.Controls.PanAndZoom.UnitTests`: broad control and testing coverage
- `tests/HeadlessTestingFramework.UnitTests`: focused framework-level tests
