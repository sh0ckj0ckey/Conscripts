using CommunityToolkit.Mvvm.ComponentModel;
using Conscripts.Models;

namespace Conscripts.ViewModels
{
    public partial class ShortcutItemViewModel : ObservableObject
    {
        private readonly ShortcutModel _shortcutModel;

        private bool _isRunning = false;

        public bool IsRunning
        {
            get => _isRunning;
            set => SetProperty(ref _isRunning, value);
        }

        public string Path
        {
            get => _shortcutModel.ScriptFilePath;
            set
            {
                if (_shortcutModel.ScriptFilePath != value)
                {
                    _shortcutModel.ScriptFilePath = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Name
        {
            get => _shortcutModel.ShortcutName;
            set
            {
                if (_shortcutModel.ShortcutName != value)
                {
                    _shortcutModel.ShortcutName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Icon
        {
            get => _shortcutModel.ShortcutIcon;
            set
            {
                if (_shortcutModel.ShortcutIcon != value)
                {
                    _shortcutModel.ShortcutIcon = value;
                    OnPropertyChanged();
                }
            }
        }

        public ShortcutType Type
        {
            get => _shortcutModel.ShortcutType;
            set
            {
                if (_shortcutModel.ShortcutType != value)
                {
                    _shortcutModel.ShortcutType = value;
                    OnPropertyChanged();
                }
            }
        }

        public ShortcutColor Color
        {
            get => _shortcutModel.ShortcutColor;
            set
            {
                if (_shortcutModel.ShortcutColor != value)
                {
                    _shortcutModel.ShortcutColor = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Category
        {
            get => _shortcutModel.Category;
            set
            {
                if (_shortcutModel.Category != value)
                {
                    _shortcutModel.Category = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool RunAsAdministrator
        {
            get => _shortcutModel.ShortcutRunas;
            set
            {
                if (_shortcutModel.ShortcutRunas != value)
                {
                    _shortcutModel.ShortcutRunas = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool RunWithoutWindow
        {
            get => _shortcutModel.NoWindow;
            set
            {
                if (_shortcutModel.NoWindow != value)
                {
                    _shortcutModel.NoWindow = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool ShowInJumpList
        {
            get => _shortcutModel.ShowInJumpList;
            set
            {
                if (_shortcutModel.ShowInJumpList != value)
                {
                    _shortcutModel.ShowInJumpList = value;
                    OnPropertyChanged();
                }
            }
        }

        public ShortcutItemViewModel(ShortcutModel shortcutModel)
        {
            _shortcutModel = shortcutModel;
        }
    }
}
