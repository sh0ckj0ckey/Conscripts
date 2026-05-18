using System;
using System.Collections.Generic;
using Conscripts.Helpers;
using Conscripts.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using NoMewing.SegoeFluentIcons;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Conscripts.Views
{
    public sealed partial class AddingView : UserControl
    {
        public MainViewModel ViewModel => (MainViewModel)DataContext;

        private readonly Action _closeView;

        private readonly string _desiredFileName;

        private string _pickedFilePath = string.Empty;

        private string _pickedIconGlyph = "\uE756";

        private List<IconInfo>? _iconList = null;

        public AddingView(Action closeView)
        {
            _closeView = closeView;
            _desiredFileName = DateTime.Now.Ticks.ToString();

            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ResetView();
        }

        private async void PickFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button)
            {
                return;
            }

            try
            {
                button.IsEnabled = false;

                var picker = new Microsoft.Windows.Storage.Pickers.FileOpenPicker(button.XamlRoot.ContentIslandEnvironment.AppWindowId);
                picker.FileTypeFilter.Add(".ps1");
                picker.FileTypeFilter.Add(".bat");
                var pickFileResult = await picker.PickSingleFileAsync();
                if (pickFileResult is not null)
                {
                    _pickedFilePath = pickFileResult.Path;
                    UpdateViewFromFile();
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

        private void RepickFileButton_Click(object sender, RoutedEventArgs e)
        {
            _pickedFilePath = string.Empty;
            UpdateViewFromFile();
        }

        private void ShortcutGrid_DragOver(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(Windows.ApplicationModel.DataTransfer.StandardDataFormats.StorageItems))
            {
                e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Copy;
            }
            else
            {
                e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.None;
            }
        }

        private async void ShortcutGrid_Drop(object sender, DragEventArgs e)
        {
            Windows.Storage.StorageFile? dropedFile = null;

            if (e.DataView.Contains(Windows.ApplicationModel.DataTransfer.StandardDataFormats.StorageItems))
            {
                var items = await e.DataView.GetStorageItemsAsync();
                foreach (var item in items)
                {
                    if (item is Windows.Storage.StorageFile file)
                    {
                        if (file.FileType.Equals(".bat", StringComparison.OrdinalIgnoreCase) ||
                            file.FileType.Equals(".ps1", StringComparison.OrdinalIgnoreCase))
                        {
                            dropedFile = file;
                            break;
                        }
                    }
                }
            }

            if (dropedFile is null)
            {
                await new ContentDialog
                {
                    Title = "DialogTitleDropedFileInvalid".GetLocalized(),
                    Content = "DialogContentDropedFileInvalid".GetLocalized(),
                    CloseButtonText = "DialogButtonGotIt".GetLocalized(),
                    XamlRoot = this.XamlRoot,
                    RequestedTheme = this.ActualTheme,
                }.ShowAsync();
                return;
            }

            _pickedFilePath = dropedFile.Path;
            UpdateViewFromFile();
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

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button || !button.IsEnabled)
            {
                return;
            }

            button.IsEnabled = false;

            try
            {
                if (string.IsNullOrWhiteSpace(_pickedFilePath))
                {
                    return;
                }

                if (!System.IO.File.Exists(_pickedFilePath))
                {
                    return;
                }

                string icon = _pickedIconGlyph;
                string name = string.IsNullOrWhiteSpace(ShortcutNameTextBox.Text) ? System.IO.Path.GetFileNameWithoutExtension(_pickedFilePath) : ShortcutNameTextBox.Text;
                string category = string.IsNullOrWhiteSpace(ShortcutCategoryTextBox.Text) ? string.Empty : ShortcutCategoryTextBox.Text;
                int color = ShortcutColorComboBox.SelectedIndex + 1;
                bool runAsAdministrator = ShortcutRunasCheckBox.IsChecked == true;
                bool runWithoutWindow = ShortcutNoWindowCheckBox.IsChecked == true;
                bool showInJumpList = ShortcutJumpListCheckBox.IsChecked == true;

                bool added = await ViewModel.AddShortcutAsync(_pickedFilePath, _desiredFileName, icon, name, category, color, runAsAdministrator, runWithoutWindow, showInJumpList);

                if (added)
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

        private void ResetView()
        {
            try
            {
                _pickedFilePath = string.Empty;
                _pickedIconGlyph = "\uE756";
                ShortcutIconButton.Content = '\uE756';
                ShortcutNameTextBox.Text = "";
                ShortcutNameTextBox.PlaceholderText = "AddingNamePlaceholderText".GetLocalized();
                ShortcutCategoryTextBox.Text = "";
                ShortcutColorComboBox.SelectedIndex = 4;
                ShortcutNoWindowCheckBox.IsEnabled = true;
                ShortcutRunasCheckBox.IsChecked = false;
                ShortcutNoWindowCheckBox.IsChecked = false;
                ShortcutJumpListCheckBox.IsChecked = false;

                UpdateViewFromFile();

                ContentScrollViewer.ChangeView(0, 0, null, true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex);
            }
        }

        private void UpdateViewFromFile()
        {
            ShortcutNameTextBox.PlaceholderText = "AddingNamePlaceholderText".GetLocalized();

            if (string.IsNullOrWhiteSpace(_pickedFilePath))
            {
                NoFileSelectedStackPanel.Visibility = Visibility.Visible;
                FileSelectedStackPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                var fileExt = System.IO.Path.GetExtension(_pickedFilePath);
                if (string.Equals(fileExt, ".bat", StringComparison.OrdinalIgnoreCase))
                {
                    NoFileSelectedStackPanel.Visibility = Visibility.Collapsed;
                    FileSelectedStackPanel.Visibility = Visibility.Visible;

                    Ps1FileIconImage.Visibility = Visibility.Collapsed;
                    BatFileIconImage.Visibility = Visibility.Visible;
                    CopyTipTextBlock.Text = $"{"AddingCopyNoticeText1".GetLocalized()} \"{_desiredFileName}.bat\" {"AddingCopyNoticeText2".GetLocalized()}";
                    ShortcutNameTextBox.PlaceholderText = System.IO.Path.GetFileNameWithoutExtension(_pickedFilePath);
                }
                else if (string.Equals(fileExt, ".ps1", StringComparison.OrdinalIgnoreCase))
                {
                    NoFileSelectedStackPanel.Visibility = Visibility.Collapsed;
                    FileSelectedStackPanel.Visibility = Visibility.Visible;

                    Ps1FileIconImage.Visibility = Visibility.Visible;
                    BatFileIconImage.Visibility = Visibility.Collapsed;
                    CopyTipTextBlock.Text = $"{"AddingCopyNoticeText1".GetLocalized()} \"{_desiredFileName}.ps1\" {"AddingCopyNoticeText2".GetLocalized()}";
                    ShortcutNameTextBox.PlaceholderText = System.IO.Path.GetFileNameWithoutExtension(_pickedFilePath);
                }
                else
                {
                    NoFileSelectedStackPanel.Visibility = Visibility.Visible;
                    FileSelectedStackPanel.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}
