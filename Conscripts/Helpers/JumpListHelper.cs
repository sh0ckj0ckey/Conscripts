using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Conscripts.Models;
using Windows.UI.StartScreen;

namespace Conscripts.Helpers
{
    /// <summary>
    /// Manages the app's taskbar JumpList, providing quick access to
    /// recently visited items and user-favorited items.
    /// </summary>
    /// <seealso cref="https://github.com/microsoft/WinUI-Gallery/blob/main/WinUIGallery/Helpers/JumpListHelper.cs"/>
    internal static class JumpListHelper
    {
        /// <summary>
        /// Rebuilds the JumpList with recently visited items and the current set of favorite items.
        /// Safe to call at any time; silently returns if JumpList is not supported.
        /// </summary>
        public static async Task UpdateJumpListAsync(List<ShortcutModel> shortcuts)
        {
            if (!JumpList.IsSupported())
            {
                return;
            }

            try
            {
                JumpList jumpList = await JumpList.LoadCurrentAsync();
                jumpList.Items.Clear();
                jumpList.SystemGroupKind = JumpListSystemGroupKind.None;

                foreach (var shortcut in shortcuts)
                {
                    if (shortcut.ShowInJumpList)
                    {
                        AddItemTask(jumpList, shortcut);
                    }
                }

                await jumpList.SaveAsync();
            }
            catch
            {
                // JumpList updates are best-effort; don't crash the app.
            }
        }

        private static void AddItemTask(JumpList jumpList, ShortcutModel shortcut)
        {
            if (shortcut is null)
            {
                return;
            }

            JumpListItem task = JumpListItem.CreateWithArguments(shortcut.ScriptFilePath, shortcut.ShortcutName);
            task.GroupName = shortcut.Category;
            task.Description = "Run " + shortcut.ShortcutName;
            //task.Logo = new Uri();
            jumpList.Items.Add(task);
        }
    }
}
