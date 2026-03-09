---
title: "Appium-like API"
---

# Appium-like API

The Appium-style layer is meant for teams that prefer WebDriver-flavored tests instead of calling simulators directly.

## Core Types

- `Avalonia.HeadlessTestingFramework.Appium.AvaloniaDriver`
- `Avalonia.HeadlessTestingFramework.Appium.AvaloniaElement`
- `Avalonia.HeadlessTestingFramework.Appium.By`
- `Avalonia.HeadlessTestingFramework.Appium.TouchAction`
- `Avalonia.HeadlessTestingFramework.Appium.WaitHelper`

## Example

```csharp
using var driver = new AvaloniaDriver(window);

driver.FindElement(By.Name("Username")).SendKeys("admin");
driver.FindElement(By.Name("Password")).SendKeys("password");
driver.FindElement(By.Name("LoginButton")).Click();

var welcome = driver.Wait.Until(d => d.FindElement(By.Name("WelcomeText")));
Assert.Equal("Welcome, admin!", welcome.Text);
```

## What It Covers

- element lookup by name, id, type, XPath, and composite locators
- click, tap, double tap, long press, drag, pinch, scroll, and key input
- explicit wait helpers and expected-condition patterns
- screenshots from drivers and individual elements
- session logging and action sequences

## When To Prefer This Layer

- your team already thinks in Selenium/Appium patterns
- you want expressive element-centric tests rather than raw event simulation
- you need richer waiting and locator composition than imperative tree traversal alone provides
