using Avalonia;
using Avalonia.Markup.Xaml;

namespace TouchTestingFramework.UnitTests;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
