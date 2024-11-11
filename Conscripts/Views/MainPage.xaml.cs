using System;
using System.IO;
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

        private WhatsNewLayout _whatsNewLayout = null;

        private SettingsLayout _settingsLayout = null;

        private PropertyLayout _propertyLayout = null;

        public MainPage()
        {
            _viewModel = MainViewModel.Instance;

            this.InitializeComponent();

            _viewModel.DispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();
        }

        /// <summary>
        /// 点击打开启动对应的脚本或者功能项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is ShortcutModel shortcut)
            {
                if (shortcut.ShortcutType == ShortcutTypeEnum.None)
                {
                    if (shortcut.Category == "add")
                    {
                        AddingGrid.Visibility = Visibility.Visible;
                        AddingBorder.Child ??= _addingLayout = new AddingLayout(_viewModel, CloseAddingLayout);
                    }
                    else if (shortcut.Category == "whatsnew")
                    {
                        WhatsNewGrid.Visibility = Visibility.Visible;
                        WhatsNewBorder.Child ??= _whatsNewLayout = new WhatsNewLayout(_viewModel);
                    }
                    else if (shortcut.Category == "settings")
                    {
                        SettingsGrid.Visibility = Visibility.Visible;
                        SettingsBorder.Child ??= _settingsLayout = new SettingsLayout(_viewModel);
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(shortcut.ScriptFilePath) ||
                        !File.Exists(shortcut.ScriptFilePath))
                    {
                        await new ContentDialog
                        {
                            Title = "无法运行这个脚本",
                            Content = $"文件不存在，可能文件已经被删除或者重命名。\r\n文件路径是 {shortcut.ScriptFilePath}。",
                            CloseButtonText = "我知道了",
                            XamlRoot = this.XamlRoot,
                            RequestedTheme = this.ActualTheme,
                        }.ShowAsync();
                    }
                    else
                    {
                        _viewModel.LaunchShortcut(shortcut);
                    }
                }
            }
        }

        /// <summary>
        /// 如果直接给Button添加了ContextFlyout右键菜单，则不会触发这个事件
        /// 因此要使用资源字典的方式来添加右键菜单，在这个事件里面处理弹出
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
                    if (shortcut.Running)
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
        }

        /// <summary>
        /// 查看脚本信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InfoMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuFlyoutItem menuItem && menuItem.DataContext is ShortcutModel shortcut)
            {
                PropertyGrid.Visibility = Visibility.Visible;
                PropertyBorder.Child ??= _propertyLayout = new PropertyLayout(_viewModel, ClosePropertyLayout);
                _propertyLayout.SetLayout(shortcut);
            }
        }

        /// <summary>
        /// 删除脚本项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuFlyoutItem menuItem && menuItem.DataContext is ShortcutModel shortcut)
            {
                ContentDialogResult result = await new ContentDialog
                {
                    Title = "删除脚本",
                    Content = $"确定要删除脚本 \"{shortcut?.ShortcutName}\" 吗？删除后将无法恢复。",
                    PrimaryButtonText = "删除",
                    CloseButtonText = "取消",
                    XamlRoot = this.XamlRoot,
                    RequestedTheme = this.ActualTheme,
                    DefaultButton = ContentDialogButton.Close
                }.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    _viewModel.DeleteShortcut(shortcut);
                }
            }
        }


        private void CloseSettings_Click(object sender, RoutedEventArgs e)
        {
            CloseSettingsLayout();
        }

        private void CloseWhatsNew_Click(object sender, RoutedEventArgs e)
        {
            CloseWhatsNewLayout();
        }

        private void CloseAdding_Click(object sender, RoutedEventArgs e)
        {
            CloseAddingLayout();
        }

        private void CloseProperty_Click(object sender, RoutedEventArgs e)
        {
            ClosePropertyLayout();
        }

        private void CloseSettingsLayout()
        {
            SettingsGrid.Visibility = Visibility.Collapsed;
            _settingsLayout?.ResetLayout();
        }

        private void CloseWhatsNewLayout()
        {
            WhatsNewGrid.Visibility = Visibility.Collapsed;
            _whatsNewLayout?.ResetLayout();
        }

        private void CloseAddingLayout()
        {
            AddingGrid.Visibility = Visibility.Collapsed;
            _addingLayout?.ResetLayout();
        }

        private void ClosePropertyLayout()
        {
            PropertyGrid.Visibility = Visibility.Collapsed;
            _propertyLayout?.ResetLayout();
        }
    }
}
