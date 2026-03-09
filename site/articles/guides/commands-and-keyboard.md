---
title: "Commands and Keyboard"
---

# Commands and Keyboard

`ZoomBorder` is designed to work both with direct input and with command surfaces such as toolbars, menus, and keyboard shortcuts.

## Built-In Commands

The main command properties are:

- `ZoomInCommand`
- `ZoomOutCommand`
- `ResetCommand`
- `FitCommand`
- `FillCommand`
- `UniformCommand`
- `UniformToFillCommand`
- `NavigateBackCommand`
- `NavigateForwardCommand`
- `ToggleStretchCommand`

Bind them directly:

```xml
<StackPanel>
  <Button Content="Zoom In" Command="{Binding #ZoomBorder.ZoomInCommand}" />
  <Button Content="Zoom Out" Command="{Binding #ZoomBorder.ZoomOutCommand}" />
  <Button Content="Reset" Command="{Binding #ZoomBorder.ResetCommand}" />
  <Button Content="Back" Command="{Binding #ZoomBorder.NavigateBackCommand}" />
</StackPanel>
```

## Default Keyboard Shortcuts

When `EnableKeyboardNavigation` is enabled:

- arrow keys pan
- `+` and `=` zoom in
- `-` zoom out
- `Ctrl+0` resets the view
- `Home` fits content to the viewport
- `Ctrl+Left` and `Ctrl+Right` navigate history

## Practical Guidance

- Keep `Focusable="True"` on the control if you rely on keyboard input.
- Prefer command bindings for toolbar buttons instead of reimplementing the same method calls in code-behind.
- If you need custom keyboard behavior, handle key events externally and call methods such as `ZoomIn(...)`, `ResetMatrix(...)`, or `CenterOn(...)`.
