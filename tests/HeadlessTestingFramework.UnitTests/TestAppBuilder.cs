using Avalonia;
using Avalonia.Headless;

[assembly: AvaloniaTestApplication(typeof(HeadlessTestingFramework.UnitTests.TestAppBuilder))]

namespace HeadlessTestingFramework.UnitTests;

public class TestAppBuilder
{
    public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<App>()
        .UseHeadless(new AvaloniaHeadlessPlatformOptions());
}
