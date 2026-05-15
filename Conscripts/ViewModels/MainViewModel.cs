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

        private readonly Microsoft.UI.Dispatching.DispatcherQueue _dispatcherQueue;

        private readonly List<ShortcutModel> _shortcutModels = [];

        private readonly Dictionary<ShortcutItemViewModel, ShortcutModel> _shortcutMap = [];

        public ObservableCollection<ShortcutGroupViewModel> ShortcutGroups { get; } = [];

        public ObservableCollection<string> ShortcutCategories { get; } = [];

        public MainViewModel(Microsoft.UI.Dispatching.DispatcherQueue dispatcherQueue)
        {
            _dispatcherQueue = dispatcherQueue;

            _ = LoadShortcutsAsync();
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
                        Dictionary<string, ShortcutGroupViewModel> shortcutGroups = new(StringComparer.Ordinal);
                        HashSet<string> shortcutCategories = new(StringComparer.Ordinal);

                        foreach (var shortcut in shortcuts)
                        {
                            shortcut.Category = shortcut.Category?.Trim() ?? string.Empty;

                            var shortcutItem = new ShortcutItemViewModel(shortcut);

                            _shortcutModels.Add(shortcut);
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
                    }
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

        private async Task SaveShortcutsAsync()
        {
            try
            {
                string shortcutsJson = JsonSerializer.Serialize(_shortcutModels, SourceGenerationContext.Default.ListShortcutModel);
                await StorageFilesService.WriteFileAsync("shortcuts.json", shortcutsJson);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex);
            }
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

                var group = this.ShortcutGroups.FirstOrDefault(x => x.Shortcuts.Contains(movingShortcut));
                if (group is not null)
                {
                    int groupIndex = group.Shortcuts.IndexOf(movingShortcut);
                    if (groupIndex > 0)
                    {
                        group.Shortcuts.Move(groupIndex, 0);
                    }
                }

                await SaveShortcutsAsync();
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

                var group = this.ShortcutGroups.FirstOrDefault(x => x.Shortcuts.Contains(movingShortcut));
                if (group is not null)
                {
                    int groupIndex = group.Shortcuts.IndexOf(movingShortcut);
                    if (groupIndex > 0)
                    {
                        group.Shortcuts.Move(groupIndex, groupIndex - 1);
                    }
                }

                await SaveShortcutsAsync();
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

                var group = this.ShortcutGroups.FirstOrDefault(x => x.Shortcuts.Contains(movingShortcut));
                if (group is not null)
                {
                    int groupIndex = group.Shortcuts.IndexOf(movingShortcut);
                    if (groupIndex >= 0 && groupIndex < group.Shortcuts.Count - 1)
                    {
                        group.Shortcuts.Move(groupIndex, groupIndex + 1);
                    }
                }

                await SaveShortcutsAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex);
            }
        }

        public async Task AddShortcutAsync(string icon, string name, string category, int color, bool runAsAdministrator, bool runWithoutWindow, bool showInJumpList, string fileName, string extension)
        {
            // Remember to UpdateCategory

            //try
            //{
            //    _allShortcuts.Insert(0, new ShortcutModel()
            //    {
            //        ScriptFilePath = path,
            //        ShortcutName = name,
            //        ShortcutIcon = icon,
            //        ShortcutType = ext == ".ps1" ? ShortcutTypeEnum.Ps1 : ext == ".bat" ? ShortcutTypeEnum.Bat : ShortcutTypeEnum.None,
            //        ShortcutColor = (ShortcutColorEnum)(Math.Max(1, Math.Min(color, 9))),
            //        ShortcutRunas = runas,
            //        NoWindow = noWindow,
            //        Category = category,
            //    });

            //    SaveShortcuts();
            //    UpdateGroupedShortcuts();
            //}
            //catch (Exception e)
            //{
            //    Trace.WriteLine(e.Message);
            //}
        }

        public async Task DeleteShortcutAsync(ShortcutItemViewModel deletingShortcut)
        {
            // Remember to UpdateCategory

            //try
            //{
            //    string deleteFilePath = deletingShortcut.ScriptFilePath;

            //    _allShortcuts.Remove(deletingShortcut);

            //    if (File.Exists(deleteFilePath))
            //    {
            //        File.Delete(deleteFilePath);
            //    }

            //    SaveShortcuts();
            //    UpdateGroupedShortcuts();
            //}
            //catch (Exception e)
            //{
            //    Trace.WriteLine(e.Message);
            //}
        }

        //public void EditShortcut(ShortcutModel editingShortcut, string name, string category, int color, bool runas, bool noWindow, string icon)
        //{
        //    try
        //    {
        //        editingShortcut.ShortcutName = name;
        //        editingShortcut.ShortcutIcon = icon;
        //        editingShortcut.ShortcutColor = (ShortcutColorEnum)(Math.Max(1, Math.Min(color, 9)));
        //        editingShortcut.ShortcutRunas = runas;
        //        editingShortcut.NoWindow = noWindow;
        //        editingShortcut.Category = category;

        //        ! 先移除再重新追加到头部，否则可能在修改分类后出现在新分类下的随机位置

        //        SaveShortcuts();
        //        UpdateGroupedShortcuts();
        //    }
        //    catch (Exception e)
        //    {
        //        Trace.WriteLine(e.Message);
        //    }
        //}

        //public async void LaunchShortcut(ShortcutModel shortcut)
        //{
        //    if (shortcut != null)
        //    {
        //        if (string.IsNullOrWhiteSpace(shortcut.ScriptFilePath) || !File.Exists(shortcut.ScriptFilePath))
        //        {
        //            return;
        //        }

        //        if (shortcut.ShortcutType == ShortcutTypeEnum.None)
        //        {
        //            return;
        //        }

        //        await Task.Run(() =>
        //        {
        //            try
        //            {
        //                DispatcherQueue.TryEnqueue(() =>
        //                {
        //                    shortcut.Running = true;
        //                });

        //                var processInfo = new ProcessStartInfo
        //                {
        //                    CreateNoWindow = !shortcut.ShortcutRunas && shortcut.NoWindow,
        //                    UseShellExecute = shortcut.ShortcutRunas || !shortcut.NoWindow,
        //                    WorkingDirectory = System.IO.Path.GetDirectoryName(shortcut.ScriptFilePath),
        //                    RedirectStandardError = false,// !shortcut.ShortcutRunas;
        //                    RedirectStandardOutput = false// !shortcut.ShortcutRunas;
        //                };

        //                if (shortcut.ShortcutType == ShortcutTypeEnum.Ps1)
        //                {
        //                    processInfo.FileName = "powershell.exe";
        //                    processInfo.Arguments = $"-NoProfile -ExecutionPolicy ByPass -File \"{shortcut.ScriptFilePath}\"";
        //                }
        //                else if (shortcut.ShortcutType == ShortcutTypeEnum.Bat)
        //                {
        //                    processInfo.FileName = shortcut.ScriptFilePath;
        //                }

        //                if (shortcut.ShortcutRunas)
        //                {
        //                    processInfo.Verb = "runas";
        //                }

        //                var process = Process.Start(processInfo);

        //                process.WaitForExit();

        //                DispatcherQueue.TryEnqueue(() =>
        //                {
        //                    shortcut.Running = false;

        //                    // 如果启用了一次性模式，执行完毕后退出应用
        //                    if (this.AppSettings.OneShotEnabled)
        //                    {
        //                        Microsoft.UI.Xaml.Application.Current.Exit();
        //                    }
        //                });
        //            }
        //            catch (Exception ex)
        //            {
        //                System.Diagnostics.Trace.WriteLine(ex);
        //            }
        //            finally
        //            {
        //                DispatcherQueue.TryEnqueue(() =>
        //                {
        //                    shortcut.Running = false;
        //                });
        //            }
        //        });
        //    }
        //}


    }
}
