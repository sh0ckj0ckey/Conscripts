using Microsoft.UI.Xaml;
using WinUIEx;

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

        public static WindowEx MainWindow { get; } = new MainWindow();

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            // Microsoft.Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "zh-CN";
            // Microsoft.Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "en-US";

            this.InitializeComponent();

            _dispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();

            UnhandledException += (s, e) =>
            {
                System.Diagnostics.Trace.WriteLine(e);
                e.Handled = true;
            };
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            MainWindow.Activate();
        }

        public void ShowMainWindow()
        {
            _dispatcherQueue.TryEnqueue(() =>
            {
                MainWindow.Restore();
                MainWindow.BringToFront();
            });
        }
    }
}
