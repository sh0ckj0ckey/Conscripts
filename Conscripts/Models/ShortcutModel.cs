using CommunityToolkit.Mvvm.ComponentModel;

namespace Conscripts.Models
{
    public class ShortcutModel : ObservableObject
    {
        private string _shortcutName = string.Empty;

        private string _shortcutIcon = string.Empty;

        private ShortcutTypeEnum _shortcutType = ShortcutTypeEnum.None;

        private ShortcutColorEnum _shortcutColor = ShortcutColorEnum.Transparent;

        private bool _shortcutRunas = false;

        private bool _noWindow = false;

        private string _category = "";

        /// <summary>
        /// 脚本文件路径
        /// </summary>
        public string ScriptFilePath { get; set; } = string.Empty;

        /// <summary>
        /// 脚本名称
        /// </summary>
        public string ShortcutName
        {
            get => _shortcutName;
            set => SetProperty(ref _shortcutName, value);
        }

        /// <summary>
        /// 脚本图标
        /// </summary>
        public string ShortcutIcon
        {
            get => _shortcutIcon;
            set => SetProperty(ref _shortcutIcon, value);
        }

        /// <summary>
        /// 脚本类型
        /// </summary>
        public ShortcutTypeEnum ShortcutType
        {
            get => _shortcutType;
            set => SetProperty(ref _shortcutType, value);
        }

        /// <summary>
        /// 脚本颜色
        /// </summary>
        public ShortcutColorEnum ShortcutColor
        {
            get => _shortcutColor;
            set => SetProperty(ref _shortcutColor, value);
        }

        /// <summary>
        /// 是否需要管理员权限
        /// </summary>
        public bool ShortcutRunas
        {
            get => _shortcutRunas;
            set => SetProperty(ref _shortcutRunas, value);
        }

        /// <summary>
        /// 运行时是否隐藏命令行窗口
        /// </summary>
        public bool NoWindow
        {
            get => _noWindow;
            set => SetProperty(ref _noWindow, value);
        }

        /// <summary>
        /// 所属分类
        /// </summary>
        public string Category
        {
            get => _category;
            set => SetProperty(ref _category, value);
        }
    }
}
