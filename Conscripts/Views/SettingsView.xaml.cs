using System;
using Conscripts.Helpers;
using Conscripts.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Conscripts.Views
{
    public sealed partial class SettingsView : UserControl
    {
        public SettingsService Settings => App.Settings;

        public string AppVersion { get; private set; }

        public SettingsView()
        {
            this.AppVersion = GetAppVersion();
            InitializeComponent();
        }

        private string GetAppVersion()
        {
            try
            {
                Windows.ApplicationModel.Package package = Windows.ApplicationModel.Package.Current;
                Windows.ApplicationModel.PackageId packageId = package.Id;
                Windows.ApplicationModel.PackageVersion version = packageId.Version;
                return string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex);
            }

            return "";
        }

        private async void GoToMicrosoftStore(object sender, RoutedEventArgs e)
        {
            try
            {
                await Launcher.LaunchUriAsync(new Uri($"ms-windows-store:REVIEW?PFN={Windows.ApplicationModel.Package.Current.Id.FamilyName}"));
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
        }

        private async void GoToDataFolder(object sender, RoutedEventArgs e)
        {
            try
            {
                var folder = await StorageFilesService.GetDataFolderAsync();
                await Launcher.LaunchFolderAsync(folder);
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
        }

        private async void GoToGitHub(object sender, RoutedEventArgs e)
        {
            try
            {
                await Launcher.LaunchUriAsync(new Uri("https://github.com/sh0ckj0ckey/Conscripts"));
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
        }
    }
}
