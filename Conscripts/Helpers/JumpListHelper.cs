using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Conscripts.Models;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.StartScreen;

namespace Conscripts.Helpers
{
    /// <summary>
    /// Manages the app's taskbar JumpList, providing quick access to custom shortcuts.
    /// </summary>
    /// <seealso cref="https://github.com/microsoft/WinUI-Gallery/blob/main/WinUIGallery/Helpers/JumpListHelper.cs"/>
    internal static class JumpListHelper
    {
        private const string IconCacheFolderName = "JumpListIcons";
        private const string IconFontFamily = "Segoe Fluent Icons";
        private const int IconSize = 64;
        private const float CornerRadius = 24f;
        private const float FontSize = 46f;

        private static readonly Dictionary<string, Uri> _iconsCache = new(StringComparer.Ordinal);

        /// <summary>
        /// Rebuilds the JumpList from the supplied shortcuts.
        /// Safe to call at any time; silently returns if JumpList is not supported.
        /// </summary>
        public static async Task UpdateJumpListAsync(IEnumerable<ShortcutModel> shortcuts)
        {
            if (shortcuts is null)
            {
                return;
            }

            if (!JumpList.IsSupported())
            {
                return;
            }

            try
            {
                JumpList jumpList = await JumpList.LoadCurrentAsync();
                jumpList.Items.Clear();
                jumpList.SystemGroupKind = JumpListSystemGroupKind.None;

                StorageFolder iconFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(IconCacheFolderName, CreationCollisionOption.OpenIfExists);

                foreach (var shortcut in shortcuts)
                {
                    if (shortcut?.ShowInJumpList != true)
                    {
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(shortcut.ScriptFilePath))
                    {
                        continue;
                    }

                    string key = $"{(int)shortcut.ShortcutColor}_{GetGlyphKey(shortcut.ShortcutIcon)}";

                    if (!_iconsCache.TryGetValue(key, out var logoUri))
                    {
                        string fileName = key;
                        foreach (char invalidChar in System.IO.Path.GetInvalidFileNameChars())
                        {
                            fileName = fileName.Replace(invalidChar, '_');
                        }
                        fileName += ".png";

                        var file = await iconFolder.TryGetItemAsync(fileName) as StorageFile;
                        if (file is null)
                        {
                            file = await iconFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                            await RenderIconAsync(file, shortcut);
                        }

                        logoUri = new Uri($"ms-appdata:///local/{IconCacheFolderName}/{fileName}");
                        _iconsCache[key] = logoUri;
                    }

                    JumpListItem task = JumpListItem.CreateWithArguments(shortcut.ScriptFilePath, shortcut.ShortcutName);
                    // task.GroupName = shortcut.Category;
                    task.Description = $"Run {shortcut.ShortcutName}";
                    task.Logo = logoUri;

                    jumpList.Items.Add(task);
                }

                await jumpList.SaveAsync();
            }
            catch
            {
                // JumpList updates are best-effort; don't crash the app.
            }
        }

        private static async Task RenderIconAsync(StorageFile file, ShortcutModel shortcut)
        {
            string glyph = string.IsNullOrWhiteSpace(shortcut?.ShortcutIcon) ? "\uE756" : shortcut.ShortcutIcon;

            Color backgroundColor = shortcut?.ShortcutColor switch
            {
                ShortcutColor.Transparent => Microsoft.UI.Colors.LightSlateGray,
                ShortcutColor.Red => Microsoft.UI.Colors.Firebrick,
                ShortcutColor.Orange => Microsoft.UI.Colors.Tomato,
                ShortcutColor.Yellow => Microsoft.UI.Colors.Goldenrod,
                ShortcutColor.Green => Microsoft.UI.Colors.ForestGreen,
                ShortcutColor.Blue => Microsoft.UI.Colors.DodgerBlue,
                ShortcutColor.Purple => Microsoft.UI.Colors.Orchid,
                ShortcutColor.Pink => Microsoft.UI.Colors.DeepPink,
                ShortcutColor.Brown => Microsoft.UI.Colors.Sienna,
                ShortcutColor.Gray => Microsoft.UI.Colors.DimGray,
                _ => Microsoft.UI.Colors.LightSlateGray,
            };

            using CanvasDevice device = CanvasDevice.GetSharedDevice();
            using CanvasRenderTarget renderTarget = new(device, IconSize, IconSize, 96);

            using (var ds = renderTarget.CreateDrawingSession())
            {
                ds.Clear(Microsoft.UI.Colors.Transparent);
                ds.FillRoundedRectangle(0, 0, IconSize, IconSize, CornerRadius, CornerRadius, backgroundColor);

                var textFormat = new CanvasTextFormat()
                {
                    FontFamily = IconFontFamily,
                    FontSize = FontSize,
                    HorizontalAlignment = CanvasHorizontalAlignment.Center,
                    VerticalAlignment = CanvasVerticalAlignment.Center,
                    WordWrapping = CanvasWordWrapping.NoWrap
                };

                ds.DrawText(glyph, new Rect(0, 0, IconSize, IconSize), Microsoft.UI.Colors.White, textFormat);
            }

            using IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite);
            stream.Size = 0;
            await renderTarget.SaveAsync(stream, CanvasBitmapFileFormat.Png);
        }

        private static string GetGlyphKey(string? glyph)
        {
            try
            {
                if (string.IsNullOrEmpty(glyph))
                {
                    return "0000";
                }

                if (char.IsSurrogatePair(glyph, 0))
                {
                    return char.ConvertToUtf32(glyph, 0).ToString("X");
                }

                return ((int)glyph[0]).ToString("X4");
            }
            catch
            {
                return "0000";
            }
        }
    }
}
