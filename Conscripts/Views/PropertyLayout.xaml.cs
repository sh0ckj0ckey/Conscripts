using System;
using System.IO;
using System.Linq;
using Conscripts.Helpers;
using Conscripts.Models;
using Conscripts.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Conscripts.Views
{
    public sealed partial class PropertyLayout : UserControl
    {
        private MainViewModel _viewModel = null;

        private Action _closePropertyAction = null;

        /// <summary>
        /// 当前正在查看和编辑的脚本项
        /// </summary>
        private ShortcutModel _shortcut = null;

        public PropertyLayout(MainViewModel viewModel, Action closePropertyAction)
        {
            _viewModel = viewModel;
            _closePropertyAction += closePropertyAction;

            this.InitializeComponent();
        }

        /// <summary>
        /// 点击更换图标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeIconButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.LoadSegoeFluentIcons();
        }

        /// <summary>
        /// 选择图标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShortcutIconGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShortcutIcon.Glyph = ShortcutIconGridView.SelectedItem is Character character ? character.Char : "\uE756";
        }

        /// <summary>
        /// 设置当前正在查看和编辑的脚本项
        /// </summary>
        /// <param name="shortcut"></param>
        public void SetLayout(ShortcutModel shortcut)
        {
            _shortcut = shortcut;
            ResetLayout();
        }

        /// <summary>
        /// 重置UI
        /// </summary>
        public void ResetLayout()
        {
            try
            {
                ShortcutIcon.Glyph = _shortcut?.ShortcutIcon ?? "\uE756";
                ShortcutNameTextBox.Text = _shortcut?.ShortcutName ?? "";
                ShortcutNameTextBox.PlaceholderText = _shortcut?.ShortcutName ?? "默认使用脚本文件名";
                ShortcutCategoryTextBox.Text = _shortcut?.Category ?? "";
                ShortcutCategoryTextBox.PlaceholderText = _shortcut?.Category ?? "";

                if (_shortcut is not null)
                {
                    int colorIndex = (int)_shortcut.ShortcutColor - 1;
                    if (colorIndex < 0 || colorIndex > 8)
                    {
                        colorIndex = 4;
                    }

                    ShortcutColorComboBox.SelectedIndex = colorIndex;
                }
                else
                {
                    ShortcutColorComboBox.SelectedIndex = 4;
                }

                ShortcutNoWindowCheckBox.IsEnabled = true;
                ShortcutNoWindowCheckBox.IsChecked = _shortcut?.NoWindow == true;
                ShortcutRunasCheckBox.IsChecked = _shortcut?.ShortcutRunas == true;
                if (_shortcut?.ShortcutRunas == true)
                {
                    ShortcutNoWindowCheckBox.IsChecked = false;
                    ShortcutNoWindowCheckBox.IsEnabled = false;
                }

                bool fileExists = !string.IsNullOrWhiteSpace(_shortcut?.ScriptFilePath) && File.Exists(_shortcut?.ScriptFilePath);
                ViewFileButton.IsEnabled = fileExists;
                ViewFileTextBlock.Text = fileExists ? "查看文件" : "文件不存在";

                //if (fileExists)
                //{
                //    ShortcutFileNameTextBlock.Text = Path.GetFileName(_shortcut.ScriptFilePath);
                //    ShortcutFileNameTextBlock.Visibility = Visibility.Visible;
                //}
                //else
                //{
                //    ShortcutFileNameTextBlock.Text = "";
                //    ShortcutFileNameTextBlock.Visibility = Visibility.Collapsed;
                //}

                ChangeIconButton.IsChecked = false;

                ShortcutIconGridView.SelectedIndex = -1;
                if (ShortcutIconGridView.Items.Count > 0)
                {
                    ShortcutIconGridView.ScrollIntoView(ShortcutIconGridView.Items.First());
                }

                PropertyScrollViewer.ChangeView(0, 0, null, true);
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
        }

        /// <summary>
        /// 勾选管理员权限时，不可勾选无窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShortcutRunasCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ShortcutNoWindowCheckBox.IsChecked = false;
            ShortcutNoWindowCheckBox.IsEnabled = false;
        }

        /// <summary>
        /// 取消勾选管理员权限时，允许勾选无窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShortcutRunasCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ShortcutNoWindowCheckBox.IsEnabled = true;
        }

        /// <summary>
        /// 点击去查看脚本文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ViewFileButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var filePath = _shortcut?.ScriptFilePath;
                if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                {
                    return;
                }

                var directoryName = Path.GetDirectoryName(filePath);
                var fileName = Path.GetFileName(filePath);

                var option = new FolderLauncherOptions();
                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(directoryName);
                foreach (var file in await folder.GetFilesAsync())
                {
                    if (file.Name == fileName)
                    {
                        option.ItemsToSelect.Add(file);
                        break;
                    }
                }

                await Launcher.LaunchFolderAsync(folder, option);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 点击重置UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            ResetLayout();
        }

        /// <summary>
        /// 点击确认修改属性
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConfirmEditButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string name = string.IsNullOrWhiteSpace(ShortcutNameTextBox.Text) ? _shortcut.ShortcutName : ShortcutNameTextBox.Text;
                string category = string.IsNullOrWhiteSpace(ShortcutCategoryTextBox.Text) ? _shortcut.Category : ShortcutCategoryTextBox.Text;
                int colorIndex = ShortcutColorComboBox.SelectedIndex + 1;
                bool runas = ShortcutRunasCheckBox.IsChecked == true;
                bool noWindow = ShortcutNoWindowCheckBox.IsChecked == true;
                string icon = ShortcutIcon.Glyph;

                _viewModel.EditShortcut(_shortcut, name, category, colorIndex, runas, noWindow, icon);

                _closePropertyAction?.Invoke();
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
        }
    }
}
