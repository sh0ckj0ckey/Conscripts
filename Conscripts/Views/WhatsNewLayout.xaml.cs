using System;
using Conscripts.ViewModels;
using Microsoft.UI.Xaml.Controls;

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
