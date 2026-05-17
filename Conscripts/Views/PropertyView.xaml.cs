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

        private void AddingShortcutCategoryTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (ViewModel.ShortcutCategories.Count > 0)
            {
                AddingShortcutCategoryTextBox.IsSuggestionListOpen = true;
            }
        }

        private void AddingShortcutRunasCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            AddingShortcutNoWindowCheckBox.IsChecked = false;
            AddingShortcutNoWindowCheckBox.IsEnabled = false;
        }

        private void AddingShortcutRunasCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            AddingShortcutNoWindowCheckBox.IsEnabled = true;
        }

        private void AddingShortcutIconsGridView_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is GridView gridView)
            {
                _iconList ??= [.. Icons.GetAllIcons()];
                gridView.ItemsSource ??= _iconList;
            }
        }

        private void AddingShortcutIconsGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is IconInfo icon)
            {
                AddingShortcutIconButton.Content = icon.Glyph;
                _pickedIconGlyph = icon.Glyph.ToString();
                AddingShortcutIconsFlyout?.Hide();
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
                string name = string.IsNullOrWhiteSpace(AddingShortcutNameTextBox.Text) ? _editingShortcut.Name : AddingShortcutNameTextBox.Text;
                string category = string.IsNullOrWhiteSpace(AddingShortcutCategoryTextBox.Text) ? string.Empty : AddingShortcutCategoryTextBox.Text;
                int color = AddingShortcutColorComboBox.SelectedIndex + 1;
                bool runAsAdministrator = AddingShortcutRunasCheckBox.IsChecked == true;
                bool runWithoutWindow = AddingShortcutNoWindowCheckBox.IsChecked == true;
                bool showInJumpList = AddingShortcutJumpListCheckBox.IsChecked == true;

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
                AddingShortcutIconButton.Content = _editingShortcut.Icon;
                AddingShortcutNameTextBox.Text = _editingShortcut.Name;
                AddingShortcutNameTextBox.PlaceholderText = _editingShortcut.Name;
                AddingShortcutCategoryTextBox.Text = _editingShortcut.Category;
                AddingShortcutColorComboBox.SelectedIndex = Enum.IsDefined(_editingShortcut.Color) ? (int)_editingShortcut.Color - 1 : 4;
                AddingShortcutRunasCheckBox.IsChecked = _editingShortcut.RunAsAdministrator;
                AddingShortcutNoWindowCheckBox.IsChecked = _editingShortcut.RunWithoutWindow && !_editingShortcut.RunAsAdministrator;
                AddingShortcutJumpListCheckBox.IsChecked = _editingShortcut.ShowInJumpList;
                AddingShortcutNoWindowCheckBox.IsEnabled = !_editingShortcut.RunAsAdministrator;

                var file = await StorageFilesService.GetFileFromDataFolderAsync(_editingShortcut.FileName);
                bool fileExists = file is not null;
                ViewButton.IsEnabled = fileExists;
                ViewTextBlock.Text = fileExists ? "PropertyViewFileButtonText".GetLocalized() : "PropertyFileNotFoundButtonText".GetLocalized();

                PropertyScrollViewer.ChangeView(0, 0, null, true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex);
            }
        }
    }
}
