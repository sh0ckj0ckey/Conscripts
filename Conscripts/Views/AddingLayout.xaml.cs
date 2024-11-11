using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Conscripts.Helpers;
using Conscripts.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
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
            _viewModel = viewModel;
            _closeAddingAction += closeAddingAction;

            this.InitializeComponent();

            this.Loaded += (_, _) =>
            {
                ResetLayout();
                _viewModel.LoadSegoeFluentIcons();
            };
        }

        /// <summary>
        /// 点击选择文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnClickChooseFile(object sender, RoutedEventArgs e)
        {
            try
            {
                var openPicker = new Windows.Storage.Pickers.FileOpenPicker();
                WinRT.Interop.InitializeWithWindow.Initialize(openPicker, App.MainWindow.GetWindowHandle());
                openPicker.ViewMode = PickerViewMode.Thumbnail;
                openPicker.FileTypeFilter.Add(".ps1");
                openPicker.FileTypeFilter.Add(".bat");

                _chosenFile = await openPicker.PickSingleFileAsync();

                UpdateLayoutByChosenFile();
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
        }

        /// <summary>
        /// 点击确认创建，复制文件，添加列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnClickCreate(object sender, RoutedEventArgs e)
        {
            try
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
                            string name = string.IsNullOrWhiteSpace(AddingShortcutNameTextBox.Text) ? _chosenFile.DisplayName : AddingShortcutNameTextBox.Text;
                            string category = string.IsNullOrWhiteSpace(AddingShortcutCategoryTextBox.Text) ? "未分类" : AddingShortcutCategoryTextBox.Text;
                            int colorIndex = AddingShortcutColorComboBox.SelectedIndex + 1;
                            bool runas = AddingShortcutRunasCheckBox.IsChecked == true;
                            bool noWindow = AddingShortcutNoWindowCheckBox.IsChecked == true;
                            string icon = AddingShortcutIconGridView.SelectedItem?.ToString();

                            _viewModel.AddShortcut(name, category, colorIndex, runas, noWindow, icon, ext, copiedFile.Path);

                            _closeAddingAction?.Invoke();
                        }
                    }
                }
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
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
                    CopyTipTextBlock.Text = $"将作为 {_desireFileName}.bat 复制到\r\n \"文档\\NoMewing\\Conscript\\\"";
                    AddingShortcutNameTextBox.PlaceholderText = _chosenFile.DisplayName;
                }
                else if (fileExt == ".ps1")
                {
                    NoFileSelectedStackPanel.Visibility = Visibility.Collapsed;
                    FileSelectedStackPanel.Visibility = Visibility.Visible;
                    Ps1FileIconImage.Visibility = Visibility.Visible;
                    BatFileIconImage.Visibility = Visibility.Collapsed;
                    CopyTipTextBlock.Text = $"将作为 {_desireFileName}.ps1 复制到\r\n \"文档\\NoMewing\\Conscript\\\"";
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
                AddingShortcutCategoryTextBox.Text = "";
                AddingShortcutColorComboBox.SelectedIndex = 4;
                AddingShortcutIconGridView.SelectedIndex = AddingShortcutIconGridView.Items.Count > 0 ? 0 : -1;
                AddingShortcutRunasCheckBox.IsChecked = false;
                AddingShortcutNoWindowCheckBox.IsEnabled = true;
                AddingShortcutNoWindowCheckBox.IsChecked = false;
                UpdateLayoutByChosenFile();

                AddingShortcutIconGridView.ScrollIntoView(AddingShortcutIconGridView.Items.First());
                AddingShortcutScrollViewer.ChangeView(0, 0, null, true);
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
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

        /// <summary>
        /// 支持拖拽文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Copy;

            //try
            //{
            //    e.AcceptedOperation = DataPackageOperation.None;

            //    if (e.DataView.Contains(StandardDataFormats.StorageItems))
            //    {
            //        var items = await e.DataView.GetStorageItemsAsync();

            //        if (items?.Count == 1 &&
            //            items[0] is StorageFile file &&
            //            (file.FileType.Equals(".bat", StringComparison.CurrentCultureIgnoreCase) ||
            //            file.FileType.Equals(".ps1", StringComparison.CurrentCultureIgnoreCase)))
            //        {
            //            e.AcceptedOperation = DataPackageOperation.Copy;
            //            e.DragUIOverride.Caption = "释放以选择此文件";
            //        }
            //        else
            //        {
            //            e.DragUIOverride.Caption = "只接受单个.bat或.ps1文件";
            //        }
            //    }
            //}
            //catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
        }

        /// <summary>
        /// 释放文件时选中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Grid_Drop(object sender, DragEventArgs e)
        {
            try
            {
                if (e.DataView.Contains(StandardDataFormats.StorageItems))
                {
                    var items = await e.DataView.GetStorageItemsAsync();
                    foreach (StorageFile file in items.Cast<StorageFile>())
                    {
                        if (file.FileType.Equals(".bat", StringComparison.CurrentCultureIgnoreCase) ||
                            file.FileType.Equals(".ps1", StringComparison.CurrentCultureIgnoreCase))
                        {
                            _chosenFile = file;
                            UpdateLayoutByChosenFile();
                            break;
                        }
                    }
                }
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
        }
    }
}
