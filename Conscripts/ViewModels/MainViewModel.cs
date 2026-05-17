using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Conscripts.Helpers;
using Conscripts.Models;

namespace Conscripts.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        /// <summary>
        /// If a category name is this value, it will be displayed as a separator line
        /// </summary>
        private const string SeperateLineSpecialCategoryName = "376C50B1-B7C1-4E7C-874A-F743DD80D95F";

        private readonly List<ShortcutModel> _shortcutModels = [];

        private readonly Dictionary<ShortcutItemViewModel, ShortcutModel> _shortcutMap = [];

        public ObservableCollection<ShortcutGroupViewModel> ShortcutGroups { get; } = [];

        public ObservableCollection<string> ShortcutCategories { get; } = [];

        public MainViewModel()
        {
            _ = LoadShortcutsAsync();
        }

        private void RebuildShortcutCollections()
        {
            try
            {
                _shortcutMap.Clear();
                this.ShortcutGroups.Clear();
                this.ShortcutCategories.Clear();

                Dictionary<string, ShortcutGroupViewModel> shortcutGroups = new(StringComparer.Ordinal);
                HashSet<string> shortcutCategories = new(StringComparer.Ordinal);

                foreach (var shortcut in _shortcutModels)
                {
                    shortcut.Category = shortcut.Category?.Trim() ?? string.Empty;

                    var shortcutItem = new ShortcutItemViewModel(shortcut);

                    _shortcutMap[shortcutItem] = shortcut;

                    if (!shortcutGroups.TryGetValue(shortcut.Category, out var group))
                    {
                        group = new ShortcutGroupViewModel(shortcut.Category);
                        shortcutGroups[shortcut.Category] = group;
                    }
                    group.Shortcuts.Add(shortcutItem);

                    if (!string.IsNullOrWhiteSpace(shortcut.Category))
                    {
                        shortcutCategories.Add(shortcut.Category);
                    }
                }

                foreach (var group in shortcutGroups.OrderBy(x => x.Key, StringComparer.CurrentCulture))
                {
                    this.ShortcutGroups.Add(group.Value);
                }

                foreach (var category in shortcutCategories.OrderBy(x => x, StringComparer.CurrentCulture))
                {
                    this.ShortcutCategories.Add(category);
                }

                var settingsGroup = new ShortcutGroupViewModel(SeperateLineSpecialCategoryName);
                settingsGroup.Shortcuts.Add(new ShortcutItemViewModel(new ShortcutModel()
                {
                    ShortcutName = "AppFeatureButtonNameAdd".GetLocalized(),
                    ShortcutIcon = "\uE710",
                    ShortcutType = ShortcutType.None,
                    ShortcutColor = ShortcutColor.Transparent,
                    Category = "add",
                    ShortcutRunas = false,
                    NoWindow = true,
                    ShowInJumpList = false,
                }));
                settingsGroup.Shortcuts.Add(new ShortcutItemViewModel(new ShortcutModel()
                {
                    ShortcutName = "AppFeatureButtonNameWhatsNew".GetLocalized(),
                    ShortcutIcon = "\uF133",
                    ShortcutType = ShortcutType.None,
                    ShortcutColor = ShortcutColor.Transparent,
                    Category = "whatsnew",
                    ShortcutRunas = false,
                    NoWindow = true,
                    ShowInJumpList = false,
                }));
                settingsGroup.Shortcuts.Add(new ShortcutItemViewModel(new ShortcutModel()
                {
                    ShortcutName = "AppFeatureButtonNameSettings".GetLocalized(),
                    ShortcutIcon = "\uE713",
                    ShortcutType = ShortcutType.None,
                    ShortcutColor = ShortcutColor.Transparent,
                    Category = "settings",
                    ShortcutRunas = false,
                    NoWindow = true,
                    ShowInJumpList = false,
                }));

                this.ShortcutGroups.Add(settingsGroup);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex);
            }
        }

        private async Task LoadShortcutsAsync()
        {
            try
            {
                _shortcutModels.Clear();
                _shortcutMap.Clear();
                this.ShortcutGroups.Clear();
                this.ShortcutCategories.Clear();

                string shortcutsJson = await StorageFilesService.ReadFileAsync("shortcuts.json");
                if (!string.IsNullOrWhiteSpace(shortcutsJson))
                {
                    var shortcuts = JsonSerializer.Deserialize(shortcutsJson, SourceGenerationContext.Default.ListShortcutModel);
                    if (shortcuts is not null)
                    {
                        foreach (var shortcut in shortcuts)
                        {
                            shortcut.Category = shortcut.Category?.Trim() ?? string.Empty;
                            _shortcutModels.Add(shortcut);
                        }
                    }
                }

                RebuildShortcutCollections();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex);
            }
        }

        private async Task SaveShortcutsAsync()
        {
            string shortcutsJson = JsonSerializer.Serialize(_shortcutModels, SourceGenerationContext.Default.ListShortcutModel);
            await StorageFilesService.WriteFileAsync("shortcuts.json", shortcutsJson);
        }

        public async Task MoveShortcutToFrontAsync(ShortcutItemViewModel movingShortcut)
        {
            try
            {
                if (!_shortcutMap.TryGetValue(movingShortcut, out var shortcutModel))
                {
                    return;
                }

                int modelIndex = _shortcutModels.IndexOf(shortcutModel);
                if (modelIndex > 0)
                {
                    _shortcutModels.RemoveAt(modelIndex);
                    _shortcutModels.Insert(0, shortcutModel);
                }

                await SaveShortcutsAsync();

                RebuildShortcutCollections();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex);
            }
        }

        public async Task MoveShortcutLeftAsync(ShortcutItemViewModel movingShortcut)
        {
            try
            {
                if (!_shortcutMap.TryGetValue(movingShortcut, out var shortcutModel))
                {
                    return;
                }

                int modelIndex = _shortcutModels.IndexOf(shortcutModel);
                if (modelIndex > 0)
                {
                    for (int i = modelIndex - 1; i >= 0; i--)
                    {
                        if (string.Equals(_shortcutModels[i].Category, shortcutModel.Category, StringComparison.Ordinal))
                        {
                            (_shortcutModels[i], _shortcutModels[modelIndex]) = (_shortcutModels[modelIndex], _shortcutModels[i]);
                            break;
                        }
                    }
                }

                await SaveShortcutsAsync();

                RebuildShortcutCollections();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex);
            }
        }

        public async Task MoveShortcutRightAsync(ShortcutItemViewModel movingShortcut)
        {
            try
            {
                if (!_shortcutMap.TryGetValue(movingShortcut, out var shortcutModel))
                {
                    return;
                }

                int modelIndex = _shortcutModels.IndexOf(shortcutModel);
                if (modelIndex >= 0 && modelIndex < _shortcutModels.Count - 1)
                {
                    for (int i = modelIndex + 1; i < _shortcutModels.Count; i++)
                    {
                        if (string.Equals(_shortcutModels[i].Category, shortcutModel.Category, StringComparison.Ordinal))
                        {
                            (_shortcutModels[modelIndex], _shortcutModels[i]) = (_shortcutModels[i], _shortcutModels[modelIndex]);
                            break;
                        }
                    }
                }

                await SaveShortcutsAsync();

                RebuildShortcutCollections();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex);
            }
        }

        public async Task<bool> AddShortcutAsync(string sourceFilePath, string desiredFileName, string icon, string name, string category, int color, bool runAsAdministrator, bool runWithoutWindow, bool showInJumpList)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sourceFilePath) || !System.IO.File.Exists(sourceFilePath))
                {
                    return false;
                }

                string extension = System.IO.Path.GetExtension(sourceFilePath);
                if (!string.Equals(extension, ".ps1", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(extension, ".bat", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                desiredFileName = string.IsNullOrWhiteSpace(desiredFileName) ? System.IO.Path.GetFileNameWithoutExtension(sourceFilePath) : desiredFileName.Trim();
                name = string.IsNullOrWhiteSpace(name) ? System.IO.Path.GetFileNameWithoutExtension(sourceFilePath) : name;
                category = category?.Trim() ?? string.Empty;

                var fileName = await StorageFilesService.CopyFileToDataFolderAsync(sourceFilePath, $"{desiredFileName}{extension}");
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    return false;
                }

                _shortcutModels.Insert(0, new ShortcutModel()
                {
                    ScriptFilePath = fileName,
                    ShortcutType = string.Equals(extension, ".ps1", StringComparison.OrdinalIgnoreCase) ? ShortcutType.Ps1 : ShortcutType.Bat,
                    ShortcutIcon = icon,
                    ShortcutName = name,
                    Category = category,
                    ShortcutColor = Enum.IsDefined(typeof(ShortcutColor), color) ? (ShortcutColor)color : ShortcutColor.Transparent,
                    ShortcutRunas = runAsAdministrator,
                    NoWindow = runWithoutWindow,
                    ShowInJumpList = showInJumpList,
                });

                await SaveShortcutsAsync();

                RebuildShortcutCollections();

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex);
                return false;
            }
        }

        public async Task DeleteShortcutAsync(ShortcutItemViewModel deletingShortcut)
        {
            try
            {
                if (!_shortcutMap.TryGetValue(deletingShortcut, out var shortcutModel))
                {
                    return;
                }

                string fileName = shortcutModel.ScriptFilePath;

                _shortcutModels.Remove(shortcutModel);

                await SaveShortcutsAsync();

                RebuildShortcutCollections();

                if (!string.IsNullOrWhiteSpace(fileName))
                {
                    var file = await StorageFilesService.GetFileFromDataFolderAsync(fileName);
                    if (file is not null)
                    {
                        try
                        {
                            await file.DeleteAsync();
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Trace.WriteLine(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex);
            }
        }

        public async Task<bool> EditShortcutAsync(ShortcutItemViewModel editingShortcut, string icon, string name, string category, int color, bool runAsAdministrator, bool runWithoutWindow, bool showInJumpList)
        {
            try
            {
                if (!_shortcutMap.TryGetValue(editingShortcut, out var shortcutModel))
                {
                    return false;
                }

                name = string.IsNullOrWhiteSpace(name) ? shortcutModel.ShortcutName : name;
                category = category?.Trim() ?? string.Empty;

                int modelIndex = _shortcutModels.IndexOf(shortcutModel);
                if (modelIndex >= 0)
                {
                    _shortcutModels.RemoveAt(modelIndex);
                }

                shortcutModel.ShortcutIcon = icon;
                shortcutModel.ShortcutName = name;
                shortcutModel.Category = category;
                shortcutModel.ShortcutColor = Enum.IsDefined(typeof(ShortcutColor), color) ? (ShortcutColor)color : ShortcutColor.Transparent;
                shortcutModel.ShortcutRunas = runAsAdministrator;
                shortcutModel.NoWindow = runWithoutWindow;
                shortcutModel.ShowInJumpList = showInJumpList;

                _shortcutModels.Insert(0, shortcutModel);

                await SaveShortcutsAsync();

                RebuildShortcutCollections();

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex);
                return false;
            }
        }

        public async Task LaunchShortcutAsync(ShortcutItemViewModel launchingShortcut)
        {
            try
            {
                if (launchingShortcut is null || launchingShortcut.IsRunning)
                {
                    return;
                }

                if (!_shortcutMap.TryGetValue(launchingShortcut, out var shortcutModel))
                {
                    return;
                }

                if (shortcutModel.ShortcutType == ShortcutType.None)
                {
                    return;
                }

                if (string.IsNullOrWhiteSpace(shortcutModel.ScriptFilePath))
                {
                    return;
                }

                launchingShortcut.IsRunning = true;

                try
                {
                    var file = await StorageFilesService.GetFileFromDataFolderAsync(shortcutModel.ScriptFilePath);
                    if (file is null)
                    {
                        return;
                    }

                    var result = await ScriptLauncher.RunAsync(file.Path, shortcutModel.ShortcutRunas, shortcutModel.NoWindow);

                    if (result.Started && App.Settings.OneShotEnabled)
                    {
                        Microsoft.UI.Xaml.Application.Current.Exit();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex);
                }
                finally
                {
                    launchingShortcut.IsRunning = false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex);
                launchingShortcut?.IsRunning = false;
            }
        }
    }
}
