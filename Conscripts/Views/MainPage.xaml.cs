using System;
using System.Linq;
using Conscripts.Helpers;
using Conscripts.Models;
using Conscripts.ViewModels;
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
        public MainViewModel ViewModel => new();

        public MainPage()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.DataContext is not ShortcutItemViewModel shortcut)
            {
                return;
            }

            if (shortcut.Type == ShortcutType.None)
            {
                if (shortcut.Category == "add")
                {
                    OpenAddingView();
                }
                else if (shortcut.Category == "whatsnew")
                {
                    OpenWhatsNewView();
                }
                else if (shortcut.Category == "settings")
                {
                    OpenSettingsView();
                }
            }
            else
            {
                Windows.Storage.StorageFile? file = null;

                if (!string.IsNullOrWhiteSpace(shortcut.FileName))
                {
                    file = await StorageFilesService.GetFileFromDataFolderAsync(shortcut.FileName);
                }

                if (file is null)
                {
                    _ = await new ContentDialog
                    {
                        Title = "DialogTitleCannotLaunch".GetLocalized(),
                        Content = $"{"DialogContentCannotLaunch".GetLocalized()} {shortcut.FileName}",
                        CloseButtonText = "DialogButtonGotIt".GetLocalized(),
                        XamlRoot = this.XamlRoot,
                        Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                        RequestedTheme = this.ActualTheme,
                        DefaultButton = ContentDialogButton.Close
                    }.ShowAsync();
                }
                else
                {
                    await this.ViewModel.LaunchShortcutAsync(shortcut);
                }
            }
        }

        private void Button_ContextRequested(UIElement sender, Microsoft.UI.Xaml.Input.ContextRequestedEventArgs args)
        {
            if (sender is not Button btn || btn.DataContext is not ShortcutItemViewModel shortcut)
            {
                return;
            }

            args.Handled = true;

            if (shortcut.Type == ShortcutType.None || shortcut.IsRunning)
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

        private async void FrontMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not MenuFlyoutItem menuItem || menuItem.DataContext is not ShortcutItemViewModel shortcut)
            {
                return;
            }

            await this.ViewModel.MoveShortcutToFrontAsync(shortcut);
        }

        private async void LeftMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not MenuFlyoutItem menuItem || menuItem.DataContext is not ShortcutItemViewModel shortcut)
            {
                return;
            }

            await this.ViewModel.MoveShortcutLeftAsync(shortcut);
        }

        private async void RightMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not MenuFlyoutItem menuItem || menuItem.DataContext is not ShortcutItemViewModel shortcut)
            {
                return;
            }

            await this.ViewModel.MoveShortcutRightAsync(shortcut);
        }

        private void InfoMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not MenuFlyoutItem menuItem || menuItem.DataContext is not ShortcutItemViewModel shortcut)
            {
                return;
            }


            OpenPropertyView(shortcut);
        }

        private async void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not MenuFlyoutItem menuItem || menuItem.DataContext is not ShortcutItemViewModel shortcut)
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
                await this.ViewModel.DeleteShortcutAsync(shortcut);
            }
        }

        private void CloseSettings_Click(object sender, RoutedEventArgs e)
        {
            CloseSettingsView();
        }

        private void CloseWhatsNew_Click(object sender, RoutedEventArgs e)
        {
            CloseWhatsNewView();
        }

        private void CloseAdding_Click(object sender, RoutedEventArgs e)
        {
            CloseAddingView();
        }

        private void CloseProperty_Click(object sender, RoutedEventArgs e)
        {
            ClosePropertyView();
        }

        private void OpenSettingsView()
        {
            SettingsGrid.Visibility = Visibility.Visible;
            SettingsContentBorder.Child = new SettingsView();
            SettingsGrid.Focus(FocusState.Keyboard);
        }

        private void OpenWhatsNewView()
        {
            WhatsNewGrid.Visibility = Visibility.Visible;
            WhatsNewContentBorder.Child = new ReleaseNotesView();
            WhatsNewGrid.Focus(FocusState.Keyboard);
        }

        private void OpenAddingView()
        {
            AddingGrid.Visibility = Visibility.Visible;
            AddingContentBorder.Child = new AddingView(CloseAddingView);
            AddingGrid.Focus(FocusState.Keyboard);
        }

        private void OpenPropertyView(ShortcutItemViewModel shortcut)
        {
            PropertyGrid.Visibility = Visibility.Visible;
            //PropertyContentBorder.Child ??= _propertyLayout = new PropertyLayout(_viewModel, ClosePropertyLayout);
            //_propertyLayout.SetLayout(shortcut);
            PropertyGrid.Focus(FocusState.Keyboard);
        }

        private void CloseSettingsView()
        {
            SettingsGrid.Visibility = Visibility.Collapsed;
            SettingsContentBorder.Child = null;
        }

        private void CloseWhatsNewView()
        {
            WhatsNewGrid.Visibility = Visibility.Collapsed;
            WhatsNewContentBorder.Child = null;
        }

        private void CloseAddingView()
        {
            AddingGrid.Visibility = Visibility.Collapsed;
            AddingContentBorder.Child = null;
        }

        private void ClosePropertyView()
        {
            PropertyGrid.Visibility = Visibility.Collapsed;
            PropertyContentBorder.Child = null;
        }
    }
}
