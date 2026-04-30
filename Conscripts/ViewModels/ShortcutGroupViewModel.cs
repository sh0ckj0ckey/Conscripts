using System.Collections.ObjectModel;

namespace Conscripts.ViewModels
{
    public class ShortcutGroupViewModel(string category)
    {
        public string Category { get; } = category;

        public ObservableCollection<ShortcutItemViewModel> Shortcuts { get; } = [];
    }
}
