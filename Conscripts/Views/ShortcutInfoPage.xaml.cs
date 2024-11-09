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
using Conscripts.Models;
using System.Diagnostics;
using Conscripts.Core;
using Windows.System;
using Windows.Storage;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Conscripts.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ShortcutInfoPage : Page
    {
        private MainViewModel MainViewModel = null;

        private ShortcutModel _shortcutModel = null;

        public ShortcutInfoPage()
        {
            this.InitializeComponent();

            MainViewModel = MainViewModel.Instance;
        }

        private void OnClickBack(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.Frame.CanGoBack)
                {
                    this.Frame.GoBack();
                }
            }
            catch { }
        }

        private void OnClickRun(object sender, RoutedEventArgs e)
        {
            try
            {
                //MainViewModel.Instance.LaunchShortcut(MainViewModel.Instance.CurrentShortcut);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void OnClickDelete(object sender, RoutedEventArgs e)
        {
            try
            {
                //MainViewModel.Instance.DelShortcut(MainViewModel.Instance.CurrentShortcut);

                if (this.Frame.CanGoBack)
                {
                    this.Frame.GoBack();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private async void OnClickGoVisit(object sender, RoutedEventArgs e)
        {
            try
            {
                //var filePath = MainViewModel.Instance.CurrentShortcut.ScriptFilePath;
                //var directoryName = Path.GetDirectoryName(filePath);
                //var fileName = Path.GetFileName(filePath);

                //var option = new FolderLauncherOptions();
                //StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(directoryName);
                //foreach (var file in await folder.GetFilesAsync())
                //{
                //    if (file.Name == fileName)
                //    {
                //        option.ItemsToSelect.Add(file);
                //        break;
                //    }
                //}

                //await Launcher.LaunchFolderAsync(folder, option);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
