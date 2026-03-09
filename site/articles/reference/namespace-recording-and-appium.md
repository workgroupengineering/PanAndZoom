---
title: "Namespaces Recording and Appium"
---

# Namespaces Recording and Appium

The remaining public surface is split between recording infrastructure and Appium-style automation.

## `Avalonia.HeadlessTestingFramework.Recording`

| Type | Primary article |
|---|---|
| `Avalonia.HeadlessTestingFramework.Recording.HeadlessScreenRecorder` | [headless-testing/recording-and-video.md](../headless-testing/recording-and-video.md) |
| `Avalonia.HeadlessTestingFramework.Recording.RecordedTouchSimulator` | [headless-testing/recording-and-video.md](../headless-testing/recording-and-video.md) |
| `Avalonia.HeadlessTestingFramework.Recording.RecordingSession` | [headless-testing/recording-and-video.md](../headless-testing/recording-and-video.md) |
| `Avalonia.HeadlessTestingFramework.Recording.RecordingOptions` | [headless-testing/recording-and-video.md](../headless-testing/recording-and-video.md) |
| `Avalonia.HeadlessTestingFramework.Recording.RenderFrameCapture` | [headless-testing/recording-and-video.md](../headless-testing/recording-and-video.md) |
| `Avalonia.HeadlessTestingFramework.Recording.VideoConverter` | [headless-testing/recording-and-video.md](../headless-testing/recording-and-video.md) |
| frame, encoder, conversion, and event types | [headless-testing/recording-and-video.md](../headless-testing/recording-and-video.md) |

## `Avalonia.HeadlessTestingFramework.Appium`

| Type | Primary article |
|---|---|
| `Avalonia.HeadlessTestingFramework.Appium.AvaloniaDriver` | [headless-testing/appium-like-api.md](../headless-testing/appium-like-api.md) |
| `Avalonia.HeadlessTestingFramework.Appium.AvaloniaElement` | [headless-testing/appium-like-api.md](../headless-testing/appium-like-api.md) |
| `Avalonia.HeadlessTestingFramework.Appium.By` | [headless-testing/appium-like-api.md](../headless-testing/appium-like-api.md) |
| `Avalonia.HeadlessTestingFramework.Appium.TouchAction` and `Avalonia.HeadlessTestingFramework.Appium.MultiTouchAction` | [headless-testing/appium-like-api.md](../headless-testing/appium-like-api.md) |
| `Avalonia.HeadlessTestingFramework.Appium.WaitHelper` and `Avalonia.HeadlessTestingFramework.Appium.WaitBuilder` | [headless-testing/appium-like-api.md](../headless-testing/appium-like-api.md) |
| `Avalonia.HeadlessTestingFramework.Appium.ExpectedConditions` | [headless-testing/appium-like-api.md](../headless-testing/appium-like-api.md) |
| driver session, actions, locator, and exception types | [headless-testing/appium-like-api.md](../headless-testing/appium-like-api.md) |

## Practical Split

- Use `Recording` types when your priority is diagnostics artifacts.
- Use `Appium` types when your priority is expressive end-to-end interaction and waiting.
