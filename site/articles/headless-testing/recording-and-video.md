---
title: "Recording and Video"
---

# Recording and Video

Recording support turns headless tests into diagnostics artifacts instead of only pass/fail signals.

## Core Types

- `Avalonia.HeadlessTestingFramework.Recording.HeadlessScreenRecorder`
- `Avalonia.HeadlessTestingFramework.Recording.RecordedTouchSimulator`
- `Avalonia.HeadlessTestingFramework.Recording.RecordingSession`
- `Avalonia.HeadlessTestingFramework.Recording.VideoConverter`
- `Avalonia.HeadlessTestingFramework.Recording.RenderFrameCapture`

## Typical Flow

1. Configure `Avalonia.HeadlessTestingFramework.Recording.RecordingOptions`.
2. Start recording against a `TopLevel`.
3. Drive the test with a simulator or `RecordedTouchSimulator`.
4. Stop recording and inspect output files.
5. Optionally convert the PNG sequence to GIF or video with `VideoConverter`.

## Important Notes

- PNG sequence output is the most direct and lossless debug artifact.
- `VideoConverter` depends on FFmpeg for video conversion workflows.
- Element and driver screenshot helpers are useful when you only need a single frame and not a time-based recording.

## Related

- [Input Simulators](input-simulators.md)
- [Diagnostics and Testing](../advanced/diagnostics-and-testing.md)
