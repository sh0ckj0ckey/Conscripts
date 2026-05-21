using Windows.Storage;

namespace Conscripts.Helpers
{
    public readonly record struct WindowPlacement(double X, double Y, double Width, double Height, bool IsMaximized);

    public class WindowPlacementService
    {
        private const string KeyX = "MainWindow_X";
        private const string KeyY = "MainWindow_Y";
        private const string KeyWidth = "MainWindow_Width";
        private const string KeyHeight = "MainWindow_Height";
        private const string KeyIsMaximized = "MainWindow_IsMaximized";

        private readonly ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;

        public bool TryLoad(out WindowPlacement placement)
        {
            if (_localSettings.Values[KeyX] is double x &&
                _localSettings.Values[KeyY] is double y &&
                _localSettings.Values[KeyWidth] is double width &&
                _localSettings.Values[KeyHeight] is double height &&
                _localSettings.Values[KeyIsMaximized] is bool isMaximized &&
                width > 0 &&
                height > 0)
            {
                placement = new WindowPlacement(x, y, width, height, isMaximized);
                return true;
            }

            placement = default;
            return false;
        }

        public void Save(WindowPlacement placement)
        {
            _localSettings.Values[KeyX] = placement.X;
            _localSettings.Values[KeyY] = placement.Y;
            _localSettings.Values[KeyWidth] = placement.Width;
            _localSettings.Values[KeyHeight] = placement.Height;
            _localSettings.Values[KeyIsMaximized] = placement.IsMaximized;
        }
    }
}
