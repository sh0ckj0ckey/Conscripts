using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;

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
        bool isRedirect = false;
        AppActivationArguments args = AppInstance.GetCurrent().GetActivatedEventArgs();
        AppInstance keyInstance = AppInstance.FindOrRegisterForKey("NoMewing.Conscripts");

        if (keyInstance.IsCurrent)
        {
            keyInstance.Activated += Program_Activated;
        }
        else
        {
            isRedirect = true;
            await keyInstance.RedirectActivationToAsync(args);
        }

        return isRedirect;
    }

    private static void Program_Activated(object? sender, AppActivationArguments args)
    {
        _app?.ShowMainWindow();
    }
}
