using System.Collections.ObjectModel;

namespace Conscripts.Models
{
    public class ShortcutsGroupModel
    {
        public string Category { get; set; } = string.Empty;

        public ObservableCollection<ShortcutModel> Shortcuts { get; set; } = new();
    }
}
