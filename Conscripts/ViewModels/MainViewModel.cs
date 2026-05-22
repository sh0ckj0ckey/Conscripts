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
        private const string SeparateLineSpecialCategoryName = "376C50B1-B7C1-4E7C-874A-F743DD80D95F";

        private readonly List<ShortcutModel> _shortcutModels = [];

        private readonly HashSet<string> _runningShortcutFileNames = new(StringComparer.OrdinalIgnoreCase);

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
                this.ShortcutGroups.Clear();
                this.ShortcutCategories.Clear();

                Dictionary<string, ShortcutGroupViewModel> shortcutGroups = new(StringComparer.Ordinal);
                HashSet<string> shortcutCategories = new(StringComparer.Ordinal);

                foreach (var shortcut in _shortcutModels)
                {
                    var shortcutItem = new ShortcutItemViewModel(shortcut)
                    {
                        IsRunning = !string.IsNullOrWhiteSpace(shortcut.ScriptFilePath) && _runningShortcutFileNames.Contains(shortcut.ScriptFilePath)
                    };

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

                var settingsGroup = new ShortcutGroupViewModel(SeparateLineSpecialCategoryName);
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
                JumpListHelper.RequestUpdateJumpList(_shortcutModels);
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
                if (movingShortcut is null)
                {
                    return;
                }

                var shortcutModel = _shortcutModels.FirstOrDefault(x => string.Equals(x.ScriptFilePath, movingShortcut.FileName, StringComparison.OrdinalIgnoreCase));

                if (shortcutModel is null)
                {
                    return;
                }

                int modelIndex = _shortcutModels.IndexOf(shortcutModel);
                if (modelIndex <= 0)
                {
                    return;
                }

                _shortcutModels.RemoveAt(modelIndex);
                _shortcutModels.Insert(0, shortcutModel);

                await SaveShortcutsAsync();

                RebuildShortcutCollections();
                JumpListHelper.RequestUpdateJumpList(_shortcutModels);
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
                if (movingShortcut is null)
                {
                    return;
                }

                var shortcutModel = _shortcutModels.FirstOrDefault(x => string.Equals(x.ScriptFilePath, movingShortcut.FileName, StringComparison.OrdinalIgnoreCase));

                if (shortcutModel is null)
                {
                    return;
                }

                int modelIndex = _shortcutModels.IndexOf(shortcutModel);
                if (modelIndex <= 0)
                {
                    return;
                }

                for (int i = modelIndex - 1; i >= 0; i--)
                {
                    if (string.Equals(_shortcutModels[i].Category, shortcutModel.Category, StringComparison.Ordinal))
                    {
                        (_shortcutModels[i], _shortcutModels[modelIndex]) = (_shortcutModels[modelIndex], _shortcutModels[i]);
                        break;
                    }
                }

                await SaveShortcutsAsync();

                RebuildShortcutCollections();
                JumpListHelper.RequestUpdateJumpList(_shortcutModels);
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
                if (movingShortcut is null)
                {
                    return;
                }

                var shortcutModel = _shortcutModels.FirstOrDefault(x => string.Equals(x.ScriptFilePath, movingShortcut.FileName, StringComparison.OrdinalIgnoreCase));

                if (shortcutModel is null)
                {
                    return;
                }

                int modelIndex = _shortcutModels.IndexOf(shortcutModel);
                if (modelIndex < 0 || modelIndex >= _shortcutModels.Count - 1)
                {
                    return;
                }

                for (int i = modelIndex + 1; i < _shortcutModels.Count; i++)
                {
                    if (string.Equals(_shortcutModels[i].Category, shortcutModel.Category, StringComparison.Ordinal))
                    {
                        (_shortcutModels[modelIndex], _shortcutModels[i]) = (_shortcutModels[i], _shortcutModels[modelIndex]);
                        break;
                    }
                }

                await SaveShortcutsAsync();

                RebuildShortcutCollections();
                JumpListHelper.RequestUpdateJumpList(_shortcutModels);
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
                JumpListHelper.RequestUpdateJumpList(_shortcutModels);

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
                if (deletingShortcut is null)
                {
                    return;
                }

                var shortcutModel = _shortcutModels.FirstOrDefault(x => string.Equals(x.ScriptFilePath, deletingShortcut.FileName, StringComparison.OrdinalIgnoreCase));

                if (shortcutModel is null)
                {
                    return;
                }

                string fileName = shortcutModel.ScriptFilePath;

                _shortcutModels.Remove(shortcutModel);

                await SaveShortcutsAsync();

                RebuildShortcutCollections();
                JumpListHelper.RequestUpdateJumpList(_shortcutModels);

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
                if (editingShortcut is null)
                {
                    return false;
                }

                var shortcutModel = _shortcutModels.FirstOrDefault(x => string.Equals(x.ScriptFilePath, editingShortcut.FileName, StringComparison.OrdinalIgnoreCase));

                if (shortcutModel is null)
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
                JumpListHelper.RequestUpdateJumpList(_shortcutModels);

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
            ShortcutItemViewModel? FindShortcut(string fileName) =>
                this.ShortcutGroups
                .SelectMany(group => group.Shortcuts)
                .FirstOrDefault(item => string.Equals(item.FileName, fileName, StringComparison.OrdinalIgnoreCase));

            string? runningKey = null;
            bool started = false;

            try
            {
                if (launchingShortcut is null)
                {
                    return;
                }

                var shortcutModel = _shortcutModels.FirstOrDefault(x => string.Equals(x.ScriptFilePath, launchingShortcut.FileName, StringComparison.OrdinalIgnoreCase));

                if (shortcutModel is null)
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

                if (!_runningShortcutFileNames.Add(shortcutModel.ScriptFilePath))
                {
                    return;
                }

                runningKey = shortcutModel.ScriptFilePath;

                var currentShortcut = FindShortcut(runningKey);
                currentShortcut?.IsRunning = true;

                var file = await StorageFilesService.GetFileFromDataFolderAsync(runningKey)
                    ?? throw new System.IO.FileNotFoundException($"File '{runningKey}' was not found in data folder.", runningKey);

                var result = await ScriptLauncher.RunAsync(file.Path, shortcutModel.ShortcutRunas, shortcutModel.NoWindow);
                started = result.Started;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex);
            }
            finally
            {
                if (!string.IsNullOrWhiteSpace(runningKey))
                {
                    _runningShortcutFileNames.Remove(runningKey);

                    var currentShortcut = FindShortcut(runningKey);
                    currentShortcut?.IsRunning = false;

                    if (started && App.Settings.OneShotEnabled && _runningShortcutFileNames.Count == 0)
                    {
                        if (App.MainWindow is not null)
                        {
                            App.MainWindow.Close();
                        }
                        else
                        {
                            Microsoft.UI.Xaml.Application.Current.Exit();
                        }
                    }
                }
            }
        }
    }
}
