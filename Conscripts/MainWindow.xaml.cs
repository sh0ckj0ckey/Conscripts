using System;
using System.Runtime.InteropServices;
using Conscripts.Helpers;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Windows.Win32;
using Windows.Win32.Foundation;
using WinRT;

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

        private readonly WindowPlacementService _windowPlacementService = new();

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
                System.Diagnostics.Trace.WriteLine($"Failed to UpdateAppTheme: {ex}");
            }
        }

        /// <summary>
        /// Updates the app's backdrop material based on the user's settings.
        /// </summary>
        private void UpdateAppBackdrop()
        {
            try
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
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"Failed to UpdateAppBackdrop: {ex}");
            }
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
            try
            {
                if (this.Content is not FrameworkElement windowContent)
                {
                    System.Diagnostics.Trace.WriteLine("Window content is not a FrameworkElement.");
                    return;
                }

                if (windowContent.XamlRoot is null)
                {
                    System.Diagnostics.Trace.WriteLine("Window content's XamlRoot is null.");
                    return;
                }

                var presenter = this.AppWindow.Presenter.As<OverlappedPresenter>();
                if (presenter is null)
                {
                    System.Diagnostics.Trace.WriteLine("Window's AppWindow.Presenter is not an OverlappedPresenter.");
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
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"Failed to SetWindowMinSize: {ex}");
            }
        }

        /// <summary>
        /// Restores the window's placement (position, size, and maximized state) from the last saved settings.
        /// </summary>
        private void RestoreWindowPlacement()
        {
            try
            {
                if (!_windowPlacementService.TryLoad(out var placement))
                {
                    int defaultWidth = 960;
                    int defaultHeight = 680;
                    this.AppWindow.Resize(new Windows.Graphics.SizeInt32(defaultWidth, defaultHeight));
                    return;
                }

                int x = (int)Math.Round(placement.X);
                int y = (int)Math.Round(placement.Y);
                int width = (int)Math.Round(placement.Width);
                int height = (int)Math.Round(placement.Height);

                this.AppWindow.MoveAndResize(new Windows.Graphics.RectInt32(x, y, width, height));

                var presenter = this.AppWindow.Presenter.As<OverlappedPresenter>();
                if (presenter is null)
                {
                    System.Diagnostics.Trace.WriteLine("Window's AppWindow.Presenter is not an OverlappedPresenter.");
                    return;
                }

                if (placement.IsMaximized)
                {
                    presenter.Maximize();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"Failed to RestoreWindowPlacement: {ex}");
            }
        }

        /// <summary>
        /// Gets the current window placement from Win32, including the restored bounds and maximized state.
        /// </summary>
        /// <returns>
        /// A <see cref="WindowPlacement"/> representing the window's restored position, size, and whether it is maximized.
        /// </returns>
        private WindowPlacement GetCurrentWindowPlacement()
        {
            IntPtr hwndValue = WinRT.Interop.WindowNative.GetWindowHandle(this);
            HWND hwnd = new(hwndValue);

            var placement = new Windows.Win32.UI.WindowsAndMessaging.WINDOWPLACEMENT()
            {
                length = (uint)Marshal.SizeOf<Windows.Win32.UI.WindowsAndMessaging.WINDOWPLACEMENT>()
            };

            if (!PInvoke.GetWindowPlacement(hwnd, ref placement))
            {
                var position = this.AppWindow.Position;
                var size = this.AppWindow.Size;
                return new WindowPlacement(position.X, position.Y, size.Width, size.Height, false);
            }

            RECT rect = placement.rcNormalPosition;
            return new WindowPlacement(
                rect.left,
                rect.top,
                rect.right - rect.left,
                rect.bottom - rect.top,
                placement.showCmd == Windows.Win32.UI.WindowsAndMessaging.SHOW_WINDOW_CMD.SW_SHOWMAXIMIZED);
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            this.UpdateAppTheme();

            MainFrame.Navigate(typeof(Views.MainPage));

            this.SetWindowMinSize(680, 460);

            if (sender is FrameworkElement rootGrid && rootGrid.XamlRoot is not null)
            {
                rootGrid.XamlRoot.Changed -= RootGridXamlRoot_Changed;
                rootGrid.XamlRoot.Changed += RootGridXamlRoot_Changed;
            }
        }

        private void RootGridXamlRoot_Changed(XamlRoot sender, XamlRootChangedEventArgs args)
        {
            this.SetWindowMinSize(680, 460);
        }

        private void Window_Closed(object sender, WindowEventArgs args)
        {
            var placement = this.GetCurrentWindowPlacement();
            _windowPlacementService.Save(placement);

            this.TitleBarViewModel.Stop();

            Application.Current.Exit();
        }
    }
}
