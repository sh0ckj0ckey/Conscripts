namespace Conscripts.Models
{
    public class ShortcutModel
    {
        public string ScriptFilePath { get; set; } = string.Empty;

        public string ShortcutName { get; set; } = string.Empty;

        public string ShortcutIcon { get; set; } = "\uE756";

        public ShortcutType ShortcutType { get; set; } = ShortcutType.None;

        public ShortcutColor ShortcutColor { get; set; } = ShortcutColor.Transparent;

        public string Category { get; set; } = string.Empty;

        public bool ShortcutRunas { get; set; } = false;

        public bool NoWindow { get; set; } = false;

        public bool ShowInJumpList { get; set; } = false;
    }
}
