using System;
using System.Collections.Generic;
using Conscripts.Helpers;
using Conscripts.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using NoMewing.SegoeFluentIcons;
using Windows.Storage;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Conscripts.Views
{
    public sealed partial class PropertyView : UserControl
    {
        public MainViewModel ViewModel => (MainViewModel)DataContext;

        private readonly ShortcutItemViewModel _editingShortcut;

        private readonly Action _closeView;

        private string _pickedIconGlyph = "\uE756";

        private List<IconInfo>? _iconList = null;

        public PropertyView(ShortcutItemViewModel editingShortcut, Action closeView)
        {
            _editingShortcut = editingShortcut;
            _closeView = closeView;
            _pickedIconGlyph = editingShortcut.Icon;

            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            ResetView();
        }

        private void ShortcutCategoryTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (ViewModel.ShortcutCategories.Count > 0)
            {
                ShortcutCategoryTextBox.IsSuggestionListOpen = true;
            }
        }

        private void ShortcutRunasCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ShortcutNoWindowCheckBox.IsChecked = false;
            ShortcutNoWindowCheckBox.IsEnabled = false;
        }

        private void ShortcutRunasCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ShortcutNoWindowCheckBox.IsEnabled = true;
        }

        private void ShortcutIconsGridView_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is GridView gridView)
            {
                _iconList ??= [.. Icons.GetAllIcons()];
                gridView.ItemsSource ??= _iconList;
            }
        }

        private void ShortcutIconsGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is IconInfo icon)
            {
                ShortcutIconButton.Content = icon.Glyph;
                _pickedIconGlyph = icon.Glyph.ToString();
                ShortcutIconsFlyout?.Hide();
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            ResetView();
        }

        private async void EditButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (sender is not Button button || !button.IsEnabled)
            {
                return;
            }

            button.IsEnabled = false;

            try
            {
                string icon = string.IsNullOrWhiteSpace(_pickedIconGlyph) ? _editingShortcut.Icon : _pickedIconGlyph;
                string name = string.IsNullOrWhiteSpace(ShortcutNameTextBox.Text) ? _editingShortcut.Name : ShortcutNameTextBox.Text;
                string category = string.IsNullOrWhiteSpace(ShortcutCategoryTextBox.Text) ? string.Empty : ShortcutCategoryTextBox.Text;
                int color = ShortcutColorComboBox.SelectedIndex + 1;
                bool runAsAdministrator = ShortcutRunasCheckBox.IsChecked == true;
                bool runWithoutWindow = ShortcutNoWindowCheckBox.IsChecked == true;
                bool showInJumpList = ShortcutJumpListCheckBox.IsChecked == true;

                bool edited = await ViewModel.EditShortcutAsync(_editingShortcut, icon, name, category, color, runAsAdministrator, runWithoutWindow, showInJumpList);

                if (edited)
                {
                    _closeView?.Invoke();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex);
            }
            finally
            {
                button.IsEnabled = true;
            }
        }

        private async void ViewButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
                var file = await StorageFilesService.GetFileFromDataFolderAsync(_editingShortcut.FileName);
                if (file is null)
                {
                    return;
                }

                StorageFolder folder = await file.GetParentAsync();
                if (folder is null)
                {
                    return;
                }

                var option = new FolderLauncherOptions();
                option.ItemsToSelect.Add(file);
                await Launcher.LaunchFolderAsync(folder, option);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex);
            }
        }

        private async void ResetView()
        {
            try
            {
                _pickedIconGlyph = _editingShortcut.Icon;
                ShortcutIconButton.Content = _editingShortcut.Icon;
                ShortcutNameTextBox.Text = _editingShortcut.Name;
                ShortcutNameTextBox.PlaceholderText = _editingShortcut.Name;
                ShortcutCategoryTextBox.Text = _editingShortcut.Category;
                ShortcutColorComboBox.SelectedIndex = Enum.IsDefined(_editingShortcut.Color) ? (int)_editingShortcut.Color - 1 : 4;
                ShortcutRunasCheckBox.IsChecked = _editingShortcut.RunAsAdministrator;
                ShortcutNoWindowCheckBox.IsChecked = _editingShortcut.RunWithoutWindow && !_editingShortcut.RunAsAdministrator;
                ShortcutJumpListCheckBox.IsChecked = _editingShortcut.ShowInJumpList;
                ShortcutNoWindowCheckBox.IsEnabled = !_editingShortcut.RunAsAdministrator;

                var file = await StorageFilesService.GetFileFromDataFolderAsync(_editingShortcut.FileName);
                bool fileExists = file is not null;
                ViewButton.IsEnabled = fileExists;
                ViewTextBlock.Text = fileExists ? "PropertyViewFileButtonText".GetLocalized() : "PropertyFileNotFoundButtonText".GetLocalized();

                ContentScrollViewer.ChangeView(0, 0, null, true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex);
            }
        }
    }
}
