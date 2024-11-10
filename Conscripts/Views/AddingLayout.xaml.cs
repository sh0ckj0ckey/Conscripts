using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Conscripts.ViewModels;
using Windows.Storage;
using Conscripts.Helpers;
using Windows.Storage.Pickers;
using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Conscripts.Views
{
    public sealed partial class AddingLayout : UserControl
    {
        private MainViewModel _viewModel = null;

        private string _desireFileName = string.Empty;

        private StorageFile _chosenFile = null;

        private Action _closeAddingAction = null;

        public AddingLayout(MainViewModel viewModel, Action closeAddingAction)
        {
            this.InitializeComponent();

            _viewModel = viewModel;

            viewModel.LoadSegoeFluentIcons();

            this.Loaded += (_, _) => ResetLayout();

            _closeAddingAction += closeAddingAction;
        }

        /// <summary>
        /// 点击选择文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnClickChooseFile(object sender, RoutedEventArgs e)
        {
            var openPicker = new Windows.Storage.Pickers.FileOpenPicker();
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, App.MainWindow.GetWindowHandle());
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.FileTypeFilter.Add(".ps1");
            openPicker.FileTypeFilter.Add(".bat");

            _chosenFile = await openPicker.PickSingleFileAsync();

            UpdateLayoutByChosenFile();
        }

        /// <summary>
        /// 点击确认创建，复制文件，添加列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnClickCreate(object sender, RoutedEventArgs e)
        {
            if (_chosenFile is not null)
            {
                var ext = Path.GetExtension(_chosenFile.Name);
                if (ext == ".ps1" || ext == ".bat")
                {
                    var dataFolder = await StorageFilesService.GetDataFolder();
                    var copiedFile = await _chosenFile.CopyAsync(dataFolder, $"{_desireFileName}{ext}", NameCollisionOption.ReplaceExisting);
                    if (copiedFile is not null)
                    {
                        int colorIndex = AddingShortcutColorComboBox.SelectedIndex + 1;
                        int iconIndex = AddingShortcutIconGridView.SelectedIndex;
                        string name = string.IsNullOrWhiteSpace(AddingShortcutNameTextBox.Text) ? _chosenFile.DisplayName : AddingShortcutNameTextBox.Text;
                        bool runas = AddingShortcutRunasCheckBox.IsChecked == true;
                        bool noWindow = AddingShortcutNoWindowCheckBox.IsChecked == true;

                        _viewModel.AddShortcut(colorIndex, iconIndex, name, ext, copiedFile.Path, runas);

                        _closeAddingAction?.Invoke();
                    }
                }
            }
        }

        /// <summary>
        /// 点击重置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickReset(object sender, RoutedEventArgs e)
        {
            ResetLayout();
        }

        /// <summary>
        /// 根据当前选择的文件，更新UI
        /// </summary>
        private void UpdateLayoutByChosenFile()
        {
            AddingShortcutNameTextBox.PlaceholderText = "默认使用脚本文件名";

            if (string.IsNullOrWhiteSpace(_chosenFile?.Name))
            {
                NoFileSelectedStackPanel.Visibility = Visibility.Visible;
                FileSelectedStackPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                var fileExt = Path.GetExtension(_chosenFile.Name);
                if (fileExt == ".bat")
                {
                    NoFileSelectedStackPanel.Visibility = Visibility.Collapsed;
                    FileSelectedStackPanel.Visibility = Visibility.Visible;
                    Ps1FileIconImage.Visibility = Visibility.Collapsed;
                    BatFileIconImage.Visibility = Visibility.Visible;
                    CopyTipTextBlock.Text = $"将作为 {_desireFileName}.bat 复制到 \"文档\\NoMewing\\Conscript\\\"";
                    AddingShortcutNameTextBox.PlaceholderText = _chosenFile.DisplayName;
                }
                else if (fileExt == ".ps1")
                {
                    NoFileSelectedStackPanel.Visibility = Visibility.Collapsed;
                    FileSelectedStackPanel.Visibility = Visibility.Visible;
                    Ps1FileIconImage.Visibility = Visibility.Visible;
                    BatFileIconImage.Visibility = Visibility.Collapsed;
                    CopyTipTextBlock.Text = $"将作为 {_desireFileName}.ps1 复制到 \"文档\\NoMewing\\Conscript\\\"";
                    AddingShortcutNameTextBox.PlaceholderText = _chosenFile.DisplayName;
                }
                else
                {
                    NoFileSelectedStackPanel.Visibility = Visibility.Visible;
                    FileSelectedStackPanel.Visibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// 重置UI
        /// </summary>
        public void ResetLayout()
        {
            try
            {
                _desireFileName = DateTime.Now.Ticks.ToString();
                _chosenFile = null;

                AddingShortcutNameTextBox.Text = "";
                AddingShortcutNameTextBox.PlaceholderText = "默认使用脚本文件名";
                AddingShortcutColorComboBox.SelectedIndex = 4;
                AddingShortcutIconGridView.SelectedIndex = 0;
                AddingShortcutRunasCheckBox.IsChecked = false;
                AddingShortcutNoWindowCheckBox.IsEnabled = true;
                AddingShortcutNoWindowCheckBox.IsChecked = false;
                UpdateLayoutByChosenFile();

                AddingShortcutIconGridView.ScrollIntoView(AddingShortcutIconGridView.Items.First());
            }
            catch { }
        }

        /// <summary>
        /// 勾选管理员权限时，不可勾选无窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddingShortcutRunasCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            AddingShortcutNoWindowCheckBox.IsChecked = false;
            AddingShortcutNoWindowCheckBox.IsEnabled = false;
        }

        /// <summary>
        /// 取消勾选管理员权限时，允许勾选无窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddingShortcutRunasCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            AddingShortcutNoWindowCheckBox.IsEnabled = true;
        }

    }
}
