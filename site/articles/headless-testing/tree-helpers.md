---
title: "Tree Helpers"
---

# Tree Helpers

Tree inspection is one of the most useful parts of the testing package because it turns a visual hierarchy into something you can query and assert against directly.

## Query APIs

- `Avalonia.HeadlessTestingFramework.VisualTreeTestHelper`: visual-tree traversal helpers
- `Avalonia.HeadlessTestingFramework.LogicalTreeTestHelper`: logical-tree traversal helpers
- `Avalonia.HeadlessTestingFramework.ControlFinder`: fluent control-finding API
- `Avalonia.HeadlessTestingFramework.TreeXPath`: XPath-like queries for tree traversal

These are a good fit when test code needs to locate controls by type, name, class, property, or relative position.

## Assertion APIs

- `Avalonia.HeadlessTestingFramework.TreeValidator`: declarative validation rules and summary output
- `Avalonia.HeadlessTestingFramework.TreeComparer`: snapshot and diff workflows for visual or logical trees
- `Avalonia.HeadlessTestingFramework.TemplateComparer`: template-part and template-structure verification

Example validator usage:

```csharp
window.Validate()
    .RequireName("ZoomBorder")
    .RequireType<Canvas>()
    .AssertValid();
```

## When To Use Which

- Use `ControlFinder` for ergonomic queries in everyday tests.
- Use `TreeXPath` when selector-style queries are easier to read than chained calls.
- Use `TreeValidator` when you need a small ruleset with strong failure messages.
- Use `TreeComparer` or `TemplateComparer` when regression detection matters more than imperative assertions.
