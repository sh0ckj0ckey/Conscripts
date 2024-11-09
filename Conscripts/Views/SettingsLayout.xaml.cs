using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Conscripts.Core;
using Conscripts.Core.Utils;
using Conscripts.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Conscripts.Views
{
    public sealed partial class SettingsLayout : UserControl
    {
        private MainViewModel _viewModel = null;
        private string _appVersion = string.Empty;

        public SettingsLayout()
        {
            this.InitializeComponent();

            _viewModel = MainViewModel.Instance;
            _appVersion = $"v{AppVersionUtil.GetAppVersion()}";
        }

        /// <summary>
        /// 打分评价
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnClickGoToStoreRate(object sender, RoutedEventArgs e)
        {
            try
            {
                await Launcher.LaunchUriAsync(new Uri($"ms-windows-store:REVIEW?PFN={Package.Current.Id.FamilyName}"));
            }
            catch { }
        }

        /// <summary>
        /// 查看数据目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnClickDbPath(object sender, RoutedEventArgs e)
        {
            try
            {
                var folder = await StorageFilesService.GetDataFolder();
                await Launcher.LaunchFolderAsync(folder);
            }
            catch { }
        }

        /// <summary>
        /// 访问 GitHub
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnClickGoGitHub(object sender, RoutedEventArgs e)
        {
            try
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/sh0ckj0ckey/Conscripts"));
            }
            catch { }
        }
    }
}
