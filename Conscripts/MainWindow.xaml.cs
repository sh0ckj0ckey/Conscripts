using System;
using Microsoft.UI;
using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Conscripts
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private readonly Windows.UI.ViewManagement.UISettings _uiSettings = new();

        public ViewModels.TitleBarViewModel TitleBarViewModel { get; } = new();

        public MainWindow()
        {
            InitializeComponent();

            this.ExtendsContentIntoTitleBar = true;
            this.SetTitleBar(TitleBar);
            this.AppWindow.SetIcon("Assets/Conscripts.ico");
            this.AppWindow.TitleBar.PreferredHeightOption = Microsoft.UI.Windowing.TitleBarHeightOption.Tall;

            this.UpdateAppBackdrop();
            this.UpdateAppTheme();

            _uiSettings.ColorValuesChanged += (s, args) =>
            {
                if (App.Settings.AppearanceIndex == 0)
                {
                    Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread().TryEnqueue(() =>
                    {
                        this.UpdateAppTheme();
                    });
                }
            };

            App.Settings.AppearanceSettingChanged += (_, _) =>
            {
                Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread().TryEnqueue(() =>
                {
                    this.UpdateAppTheme();
                });
            };

            App.Settings.BackdropSettingChanged += (_, _) =>
            {
                Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread().TryEnqueue(() =>
                {
                    this.UpdateAppBackdrop();
                });
            };

            this.TitleBarViewModel.Start();
        }

        private void UpdateAppTheme()
        {
            try
            {
                // Get the current system theme by calculating the brightness of the foreground color.
                bool isLightTheme = true;
                if (App.Settings.AppearanceIndex == 0)
                {
                    var color = _uiSettings?.GetColorValue(Windows.UI.ViewManagement.UIColorType.Foreground) ?? Colors.Black;
                    var g = color.R * 0.299 + color.G * 0.587 + color.B * 0.114;
                    isLightTheme = g < 100;
                }
                else
                {
                    isLightTheme = App.Settings.AppearanceIndex == 2;
                }

                var titleBar = this.AppWindow.TitleBar;

                // Set active window colors
                // Note: No effect when app is running on Windows 10 since color customization is not supported.
                titleBar.ForegroundColor = isLightTheme ? Colors.Black : Colors.White;
                titleBar.BackgroundColor = Colors.Transparent;
                titleBar.ButtonForegroundColor = isLightTheme ? Colors.Black : Colors.White;
                titleBar.ButtonBackgroundColor = Colors.Transparent;
                titleBar.ButtonHoverForegroundColor = isLightTheme ? Colors.Black : Colors.White;
                titleBar.ButtonHoverBackgroundColor = isLightTheme ? Windows.UI.Color.FromArgb(10, 0, 0, 0) : Windows.UI.Color.FromArgb(16, 255, 255, 255);
                titleBar.ButtonPressedForegroundColor = isLightTheme ? Colors.Black : Colors.White;
                titleBar.ButtonPressedBackgroundColor = isLightTheme ? Windows.UI.Color.FromArgb(08, 0, 0, 0) : Windows.UI.Color.FromArgb(10, 255, 255, 255);

                // Set inactive window colors
                // Note: No effect when app is running on Windows 10 since color customization is not supported.
                titleBar.InactiveForegroundColor = Colors.Gray;
                titleBar.InactiveBackgroundColor = Colors.Transparent;
                titleBar.ButtonInactiveForegroundColor = Colors.Gray;
                titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

                // Set the theme of content
                if (this.Content is FrameworkElement rootElement)
                {
                    if (App.Settings.AppearanceIndex == 1)
                    {
                        rootElement.RequestedTheme = ElementTheme.Dark;
                    }
                    else if (App.Settings.AppearanceIndex == 2)
                    {
                        rootElement.RequestedTheme = ElementTheme.Light;
                    }
                    else
                    {
                        rootElement.RequestedTheme = ElementTheme.Default;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex);
            }
        }

        private void UpdateAppBackdrop()
        {
            this.SystemBackdrop = App.Settings.BackdropIndex == 2 ?
                new Microsoft.UI.Xaml.Media.DesktopAcrylicBackdrop() :
                new Microsoft.UI.Xaml.Media.MicaBackdrop()
                {
                    Kind = App.Settings.BackdropIndex == 1 ?
                        Microsoft.UI.Composition.SystemBackdrops.MicaKind.BaseAlt :
                        Microsoft.UI.Composition.SystemBackdrops.MicaKind.Base
                };
        }

        private void MainFrame_Loaded(object sender, RoutedEventArgs e)
        {
            this.UpdateAppTheme();
            MainFrame.Navigate(typeof(Views.MainPage));
        }

        private void Window_Closed(object sender, WindowEventArgs args)
        {
            TitleBarViewModel.Stop();
            Application.Current.Exit();
        }
    }
}
