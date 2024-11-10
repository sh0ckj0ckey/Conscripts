using Conscripts.Models;
using Conscripts.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Conscripts.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MainViewModel _viewModel = null;

        private AddingLayout _addingLayout = null;

        private SettingsLayout _settingsLayout = null;

        public MainPage()
        {
            _viewModel = MainViewModel.Instance;

            this.InitializeComponent();

            _viewModel.DispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is ShortcutModel shortcut)
            {
                if (shortcut.ShortcutType == ShortcutTypeEnum.None)
                {
                    if (shortcut.Category == "add")
                    {
                        AddingBorder.Child ??= _addingLayout = new AddingLayout(_viewModel, CloseAddingLayout);
                        AddingGrid.Visibility = Visibility.Visible;
                    }
                    else if (shortcut.Category == "whatsnew")
                    {

                    }
                    else if (shortcut.Category == "settings")
                    {
                        SettingsBorder.Child ??= _settingsLayout = new SettingsLayout(_viewModel);
                        SettingsGrid.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        /// <summary>
        /// 如果直接给Button添加了ContextFlyout右键菜单，则不会触发这个事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void Button_ContextRequested(UIElement sender, ContextRequestedEventArgs args)
        {
            if (sender is Button btn && btn.DataContext is ShortcutModel shortcut)
            {
                if (shortcut.ShortcutType == ShortcutTypeEnum.None)
                {
                    args.Handled = true;
                }
                else
                {
                    MenuFlyout flyout = (MenuFlyout)btn.Resources["ShortcutMenuFlyout"];
                    flyout.ShowAt(btn);
                }
            }
        }

        private void InfoMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CloseSettings_Click(object sender, RoutedEventArgs e)
        {
            CloseSettingsLayout();
        }

        private void CloseAdding_Click(object sender, RoutedEventArgs e)
        {
            CloseAddingLayout();
        }

        private void CloseSettingsLayout()
        {
            SettingsGrid.Visibility = Visibility.Collapsed;
        }

        private void CloseAddingLayout()
        {
            AddingGrid.Visibility = Visibility.Collapsed;
            _addingLayout?.ResetLayout();
        }
    }
}
