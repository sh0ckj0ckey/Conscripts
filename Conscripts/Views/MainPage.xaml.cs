using System;
using System.Linq;
using Conscripts.Helpers;
using Conscripts.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Conscripts.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public ViewModels.MainViewModel ViewModel { get; }

        public MainPage()
        {
            this.ViewModel = new ViewModels.MainViewModel(Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread());

            InitializeComponent();

            _ = this.ViewModel.LoadShortcutsAsync();
        }

        private void Button_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            //if (sender is Button btn && btn.DataContext is ShortcutModel shortcut)
            //{
            //    if (shortcut.ShortcutType == ShortcutTypeEnum.None)
            //    {
            //        if (shortcut.Category == "add")
            //        {
            //            AddingGrid.Visibility = Visibility.Visible;
            //            AddingBorder.Child ??= _addingLayout = new AddingLayout(_viewModel, CloseAddingLayout);
            //        }
            //        else if (shortcut.Category == "whatsnew")
            //        {
            //            WhatsNewGrid.Visibility = Visibility.Visible;
            //            WhatsNewBorder.Child ??= _whatsNewLayout = new WhatsNewLayout(_viewModel);
            //        }
            //        else if (shortcut.Category == "settings")
            //        {
            //            SettingsGrid.Visibility = Visibility.Visible;
            //            SettingsBorder.Child ??= _settingsLayout = new SettingsLayout(_viewModel);
            //        }
            //    }
            //    else
            //    {
            //        if (string.IsNullOrWhiteSpace(shortcut.ScriptFilePath) ||
            //            !File.Exists(shortcut.ScriptFilePath))
            //        {
            //            await new ContentDialog
            //            {
            //                Title = "DialogTitleCannotLaunch".GetLocalized(),
            //                Content = $"{"DialogContentCannotLaunch".GetLocalized()} {shortcut.ScriptFilePath}",
            //                CloseButtonText = "DialogButtonGotIt".GetLocalized(),
            //                XamlRoot = this.XamlRoot,
            //                RequestedTheme = this.ActualTheme,
            //            }.ShowAsync();
            //        }
            //        else
            //        {
            //            _viewModel.LaunchShortcut(shortcut);
            //        }
            //    }
            //}
        }

        private void Button_ContextRequested(Microsoft.UI.Xaml.UIElement sender, Microsoft.UI.Xaml.Input.ContextRequestedEventArgs args)
        {
            if (sender is not Button btn || btn.DataContext is not ViewModels.ShortcutItemViewModel shortcut)
            {
                return;
            }

            args.Handled = true;

            if (shortcut.Type == Models.ShortcutType.None || shortcut.IsRunning)
            {
                return;
            }

            var group = this.ViewModel.ShortcutGroups.FirstOrDefault(group => group.Shortcuts.Contains(shortcut));
            if (group is null)
            {
                return;
            }

            if (btn.Resources["ShortcutMenuFlyout"] is not MenuFlyout flyout)
            {
                return;
            }

            int index = group.Shortcuts.IndexOf(shortcut);
            bool isFirst = index <= 0;
            bool isLast = index >= group.Shortcuts.Count - 1;

            foreach (var item in flyout.Items)
            {
                item?.Visibility = (item?.Tag?.ToString()) switch
                {
                    "front" or "left" => isFirst ? Visibility.Collapsed : Visibility.Visible,
                    "right" => isLast ? Visibility.Collapsed : Visibility.Visible,
                    _ => Visibility.Visible,
                };
            }

            flyout.ShowAt(btn);
        }

        private void FrontMenuItem_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (sender is not MenuFlyoutItem menuItem || menuItem.DataContext is not ViewModels.ShortcutItemViewModel shortcut)
            {
                return;
            }

            this.ViewModel.MoveShortcutToFront(shortcut);
        }

        private void LeftMenuItem_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (sender is not MenuFlyoutItem menuItem || menuItem.DataContext is not ViewModels.ShortcutItemViewModel shortcut)
            {
                return;
            }

            this.ViewModel.MoveShortcutLeft(shortcut);
        }

        private void RightMenuItem_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (sender is not MenuFlyoutItem menuItem || menuItem.DataContext is not ViewModels.ShortcutItemViewModel shortcut)
            {
                return;
            }

            this.ViewModel.MoveShortcutRight(shortcut);
        }

        private void InfoMenuItem_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            //if (sender is MenuFlyoutItem menuItem && menuItem.DataContext is ShortcutModel shortcut)
            //{
            //    PropertyGrid.Visibility = Visibility.Visible;
            //    PropertyBorder.Child ??= _propertyLayout = new PropertyLayout(_viewModel, ClosePropertyLayout);
            //    _propertyLayout.SetLayout(shortcut);
            //}
        }

        private async void DeleteMenuItem_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (sender is not MenuFlyoutItem menuItem || menuItem.DataContext is not ViewModels.ShortcutItemViewModel shortcut)
            {
                return;
            }

            ContentDialogResult result = await new ContentDialog
            {
                Title = "DialogTitleDeleteScript".GetLocalized(),
                Content = $"{"DialogContentDeleteConfirm1".GetLocalized()} \"{shortcut.Name}\" {"DialogContentDeleteConfirm2".GetLocalized()}",
                PrimaryButtonText = "DialogButtonDelete".GetLocalized(),
                CloseButtonText = "DialogButtonCancel".GetLocalized(),
                XamlRoot = this.XamlRoot,
                Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                RequestedTheme = this.ActualTheme,
                DefaultButton = ContentDialogButton.Close
            }.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                this.ViewModel.DeleteShortcut(shortcut);
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
            //_settingsLayout?.ResetLayout();
        }

        private void CloseWhatsNewLayout()
        {
            WhatsNewGrid.Visibility = Visibility.Collapsed;
            //_whatsNewLayout?.ResetLayout();
        }

        private void CloseAddingLayout()
        {
            AddingGrid.Visibility = Visibility.Collapsed;
            //_addingLayout?.ResetLayout();
        }

        private void ClosePropertyLayout()
        {
            PropertyGrid.Visibility = Visibility.Collapsed;
            //_propertyLayout?.ResetLayout();
        }
    }
}
