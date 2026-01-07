using Avalonia;
using Avalonia.Headless;

[assembly: AvaloniaTestApplication(typeof(TouchTestingFramework.UnitTests.TestAppBuilder))]

namespace TouchTestingFramework.UnitTests;

public class TestAppBuilder
{
    public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<App>()
        .UseHeadless(new AvaloniaHeadlessPlatformOptions());
}
