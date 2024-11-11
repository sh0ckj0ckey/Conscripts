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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Conscripts.Views
{
    public sealed partial class WhatsNewLayout : UserControl
    {
        private readonly MainViewModel _viewModel = null;

        public WhatsNewLayout(MainViewModel viewModel)
        {
            this.InitializeComponent();

            _viewModel = viewModel;
        }

        /// <summary>
        /// ÷ÿ÷√UI
        /// </summary>
        public void ResetLayout()
        {
            try
            {
                WhatsNewScrollViewer.ChangeView(0, 0, null, true);
            }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
        }
    }
}
