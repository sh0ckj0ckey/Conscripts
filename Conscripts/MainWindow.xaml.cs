using System;
using Microsoft.UI;
using Microsoft.UI.Windowing;
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

        private readonly Helpers.WindowPlacementService _windowPlacementService = new();

        private Helpers.WindowPlacement _lastNormalWindowPlacement;

        public ViewModels.TitleBarViewModel TitleBarViewModel { get; } = new();

        public MainWindow()
        {
            InitializeComponent();

            this.ExtendsContentIntoTitleBar = true;
            this.SetTitleBar(TitleBar);
            this.AppWindow.SetIcon("Assets/Conscripts.ico");
            this.AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;

            this.RestoreWindowPlacement();

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

        /// <summary>
        /// Updates the app's theme based on the user's settings and system theme.
        /// </summary>
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

        /// <summary>
        /// Updates the app's backdrop material based on the user's settings.
        /// </summary>
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

        /// <summary>
        /// Sets the minimum allowable size for the window.
        /// </summary>
        /// <param name="width">Minimum width in effective pixels. Must be non-negative.</param>
        /// <param name="height">Minimum height in effective pixels. Must be non-negative.</param>
        /// <seealso href="https://github.com/microsoft/WinUI-Gallery/blob/main/WinUIGallery/Helpers/WindowHelper.cs">
        /// WindowHelper.cs in WinUI Gallery
        /// </seealso>
        private void SetWindowMinSize(double width, double height)
        {
            if (this.Content is not FrameworkElement windowContent)
            {
                System.Diagnostics.Debug.WriteLine("Window content is not a FrameworkElement.");
                return;
            }

            if (windowContent.XamlRoot is null)
            {
                System.Diagnostics.Debug.WriteLine("Window content's XamlRoot is null.");
                return;
            }

            if (this.AppWindow.Presenter is not OverlappedPresenter presenter)
            {
                System.Diagnostics.Debug.WriteLine("Window's AppWindow.Presenter is not an OverlappedPresenter.");
                return;
            }

            var scale = windowContent.XamlRoot.RasterizationScale;
            if (scale <= 0)
            {
                scale = 1.0;
            }

            var minWidth = width * scale;
            var minHeight = height * scale;
            presenter.PreferredMinimumWidth = (int)minWidth;
            presenter.PreferredMinimumHeight = (int)minHeight;
        }

        /// <summary>
        /// Restores the window's placement (position, size, and maximized state) from the last saved settings.
        /// </summary>
        private void RestoreWindowPlacement()
        {
            if (this.AppWindow.Presenter is not OverlappedPresenter presenter)
            {
                System.Diagnostics.Debug.WriteLine("Window's AppWindow.Presenter is not an OverlappedPresenter.");
                return;
            }

            if (!_windowPlacementService.TryLoad(out var placement))
            {
                return;
            }

            _lastNormalWindowPlacement = placement with { IsMaximized = false };

            int x = (int)Math.Round(placement.X);
            int y = (int)Math.Round(placement.Y);
            int width = (int)Math.Round(placement.Width);
            int height = (int)Math.Round(placement.Height);

            this.AppWindow.MoveAndResize(new Windows.Graphics.RectInt32(x, y, width, height));

            if (placement.IsMaximized)
            {
                presenter.Maximize();
            }
        }

        /// <summary>
        /// Records the current window's placement (position, size, and maximized state) to be saved for future sessions.
        /// </summary>
        private void RecordLastNormalWindowPlacement()
        {
            if (this.AppWindow.Presenter is not OverlappedPresenter presenter)
            {
                return;
            }

            if (presenter.State != OverlappedPresenterState.Restored)
            {
                return;
            }

            var position = this.AppWindow.Position;
            var size = this.AppWindow.Size;
            _lastNormalWindowPlacement = new Helpers.WindowPlacement(position.X, position.Y, size.Width, size.Height, false);
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            this.UpdateAppTheme();

            MainFrame.Navigate(typeof(Views.MainPage));

            this.SetWindowMinSize(680, 460);

            this.RecordLastNormalWindowPlacement();

            this.AppWindow.Changed -= AppWindow_Changed;
            this.AppWindow.Changed += AppWindow_Changed;

            if (sender is FrameworkElement rootGrid && rootGrid.XamlRoot is not null)
            {
                rootGrid.XamlRoot.Changed -= RootGridXamlRoot_Changed;
                rootGrid.XamlRoot.Changed += RootGridXamlRoot_Changed;
            }
        }

        private void AppWindow_Changed(AppWindow sender, AppWindowChangedEventArgs args)
        {
            if (!args.DidPositionChange && !args.DidSizeChange)
            {
                return;
            }

            this.RecordLastNormalWindowPlacement();
        }

        private void RootGridXamlRoot_Changed(XamlRoot sender, XamlRootChangedEventArgs args)
        {
            this.SetWindowMinSize(680, 460);
        }

        private void Window_Closed(object sender, WindowEventArgs args)
        {
            this.AppWindow.Changed -= AppWindow_Changed;

            bool isMaximized = this.AppWindow.Presenter is OverlappedPresenter presenter && presenter.State == OverlappedPresenterState.Maximized;
            _windowPlacementService.Save(_lastNormalWindowPlacement with { IsMaximized = isMaximized });

            TitleBarViewModel.Stop();
            Application.Current.Exit();
        }
    }
}
