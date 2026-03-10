---
title: "Namespace Avalonia.HeadlessTestingFramework"
---

# Namespace Avalonia.HeadlessTestingFramework

This namespace contains the core testing utilities that are not specific to Appium-style automation or recording output.

## Input And Gesture Types

| Type | Primary article |
|---|---|
| `Avalonia.HeadlessTestingFramework.TouchInputSimulator` | [headless-testing/input-simulators.md](../headless-testing/input-simulators.md) |
| `Avalonia.HeadlessTestingFramework.KeyboardInputSimulator` | [headless-testing/input-simulators.md](../headless-testing/input-simulators.md) |
| `Avalonia.HeadlessTestingFramework.MouseInputSimulator` | [headless-testing/input-simulators.md](../headless-testing/input-simulators.md) |
| `Avalonia.HeadlessTestingFramework.GestureSimulator` | [headless-testing/input-simulators.md](../headless-testing/input-simulators.md) |
| `Avalonia.HeadlessTestingFramework.MultiTouchTestHelperFactory` | [headless-testing/input-simulators.md](../headless-testing/input-simulators.md) |
| `Avalonia.HeadlessTestingFramework.GestureRecognizerTestHelper` | [getting-started/quickstart-headless-testing.md](../getting-started/quickstart-headless-testing.md) |
| `Avalonia.HeadlessTestingFramework.SwipeDirection` | [headless-testing/input-simulators.md](../headless-testing/input-simulators.md) |

## Query And Assertion Types

| Type | Primary article |
|---|---|
| `Avalonia.HeadlessTestingFramework.ControlFinder` | [headless-testing/tree-helpers.md](../headless-testing/tree-helpers.md) |
| `Avalonia.HeadlessTestingFramework.ControlFinderExtensions` | [headless-testing/tree-helpers.md](../headless-testing/tree-helpers.md) |
| `Avalonia.HeadlessTestingFramework.VisualTreeTestHelper` | [headless-testing/tree-helpers.md](../headless-testing/tree-helpers.md) |
| `Avalonia.HeadlessTestingFramework.LogicalTreeTestHelper` | [headless-testing/tree-helpers.md](../headless-testing/tree-helpers.md) |
| `Avalonia.HeadlessTestingFramework.TreeXPath` and `Avalonia.HeadlessTestingFramework.TreeXPathExtensions` | [headless-testing/tree-helpers.md](../headless-testing/tree-helpers.md) |
| `Avalonia.HeadlessTestingFramework.TreeValidator` and related result and exception types | [headless-testing/tree-helpers.md](../headless-testing/tree-helpers.md) |
| `Avalonia.HeadlessTestingFramework.TreeComparer` and related snapshot and difference types | [headless-testing/tree-helpers.md](../headless-testing/tree-helpers.md) |
| `Avalonia.HeadlessTestingFramework.TemplateComparer` and related template comparison types | [headless-testing/tree-helpers.md](../headless-testing/tree-helpers.md) |

## Guidance

- Start with `ControlFinder` for readable tests.
- Move to `TreeXPath` when selector-style queries are clearer.
- Use `TreeValidator`, `TreeComparer`, or `TemplateComparer` when you need reusable regression assertions instead of one-off expectations.
