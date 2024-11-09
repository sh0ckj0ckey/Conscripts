using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Conscripts.Models
{
    public class ShortcutsGroupModel
    {
        public string Category { get; set; } = string.Empty;

        public ObservableCollection<ShortcutModel> Shortcuts { get; set; } = new();
    }
}
