using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using Windows.ApplicationModel.Activation;

namespace Conscripts;

public static class Program
{
    private static App? _app;

    [STAThread]
    public static void Main(string[] _)
    {
        WinRT.ComWrappersSupport.InitializeComWrappers();

        if (DecideRedirectionAsync().Result == true)
        {
            return;
        }

        Application.Start((p) =>
        {
            var context = new DispatcherQueueSynchronizationContext(DispatcherQueue.GetForCurrentThread());
            SynchronizationContext.SetSynchronizationContext(context);
            _app = new App();
        });
    }

    private static async Task<bool> DecideRedirectionAsync()
    {
        AppActivationArguments args = AppInstance.GetCurrent().GetActivatedEventArgs();
        AppInstance keyInstance = AppInstance.FindOrRegisterForKey("NoMewing.Conscripts");

        if (keyInstance.IsCurrent)
        {
            keyInstance.Activated += Program_Activated;
            return false;
        }

        await keyInstance.RedirectActivationToAsync(args);
        return true;
    }

    private static void Program_Activated(object? sender, AppActivationArguments args)
    {
        ExtendedActivationKind kind = args.Kind;
        string? launchArguments = null;

        if (kind == ExtendedActivationKind.Launch && args.Data is ILaunchActivatedEventArgs launchArgs)
        {
            launchArguments = string.IsNullOrWhiteSpace(launchArgs.Arguments) ? null : launchArgs.Arguments.Trim();
        }

        _app?.HandleRedirectedActivation(kind, launchArguments);
    }
}
