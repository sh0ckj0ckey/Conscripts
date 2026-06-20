using CommunityToolkit.Mvvm.Messaging;
using Conscripts.Messages;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using Windows.ApplicationModel.Activation;
using Windows.Win32;
using Windows.Win32.Foundation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Conscripts
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private readonly Microsoft.UI.Dispatching.DispatcherQueue _dispatcherQueue;

        private static string? _launchArguments = null;

        internal static MainWindow? MainWindow { get; private set; } = null;

        internal static Helpers.SettingsService Settings { get; } = new Helpers.SettingsService();

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            _dispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();

            this.InitializeComponent();

            WeakReferenceMessenger.Default.Register<ConsumeLaunchArgumentsRequestMessage>(this, static (_, message) =>
            {
                string? arguments = _launchArguments;
                _launchArguments = null;
                message.Reply(arguments);
            });

            UnhandledException += (s, e) =>
            {
                e.Handled = true;

                var notification = new AppNotificationBuilder()
                .AddText("An exception was thrown.")
                .AddText($"Type: {e.Exception.GetType()}")
                .AddText($"Message: {e.Message}\r\n" +
                         $"HResult: {e.Exception.HResult}")
                .BuildNotification();

                AppNotificationManager.Default.Show(notification);
            };
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            var activationArgs = AppInstance.GetCurrent().GetActivatedEventArgs();
            if (activationArgs is not null && activationArgs.Kind == ExtendedActivationKind.Launch && activationArgs.Data is ILaunchActivatedEventArgs launchArgs)
            {
                _launchArguments = launchArgs.Arguments?.Trim();
            }

            MainWindow ??= new MainWindow();
            MainWindow.Activate();
        }

        internal void HandleRedirectedActivation(ExtendedActivationKind kind, string? launchArguments)
        {
            _dispatcherQueue.TryEnqueue(() =>
            {
                MainWindow ??= new MainWindow();

                HWND hWnd = new(WinRT.Interop.WindowNative.GetWindowHandle(MainWindow));
                PInvoke.ShowWindow(hWnd, Windows.Win32.UI.WindowsAndMessaging.SHOW_WINDOW_CMD.SW_RESTORE);
                PInvoke.SetForegroundWindow(hWnd);

                MainWindow.Activate();

                if (kind == ExtendedActivationKind.Launch)
                {
                    if (!string.IsNullOrWhiteSpace(launchArguments))
                    {
                        WeakReferenceMessenger.Default.Send(new RedirectedActivationArgumentsMessage(launchArguments));
                    }
                }
            });
        }
    }
}