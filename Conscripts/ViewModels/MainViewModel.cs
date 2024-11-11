using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
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
        private static Lazy<MainViewModel> _lazyVM = new Lazy<MainViewModel>(() => new MainViewModel());
        public static MainViewModel Instance => _lazyVM.Value;

        public Microsoft.UI.Dispatching.DispatcherQueue DispatcherQueue = null;

        public SettingsService AppSettings { get; set; } = new SettingsService();

        /// <summary>
        /// 全部脚本列表
        /// </summary>
        private List<ShortcutModel> _allShortcuts = new();

        /// <summary>
        /// 经过分类的脚本列表
        /// </summary>
        public ObservableCollection<ShortcutsGroupModel> GroupedShortcuts { get; private set; } = new();

        /// <summary>
        /// 目前已有的分类名称
        /// </summary>
        private HashSet<string> _categories = new();

        /// <summary>
        /// 目前已有的分类名称
        /// </summary>
        public ObservableCollection<string> Categories { get; private set; } = new();

        /// <summary>
        /// 图标列表
        /// </summary>
        public ObservableCollection<Character> AllIcons { get; set; } = new();

        private MainViewModel()
        {
            LoadShortcuts();
        }

        /// <summary>
        /// 启动指定的脚本快捷项
        /// </summary>
        /// <param name="shortcut"></param>
        public async void LaunchShortcut(ShortcutModel shortcut)
        {
            if (shortcut != null)
            {
                await Task.Run(async () =>
                {
                    try
                    {
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            shortcut.Running = true;
                        });

                        var processInfo = new ProcessStartInfo
                        {
                            CreateNoWindow = !shortcut.ShortcutRunas && shortcut.NoWindow,
                            UseShellExecute = shortcut.ShortcutRunas || !shortcut.NoWindow,
                            WorkingDirectory = System.IO.Path.GetDirectoryName(shortcut.ScriptFilePath),
                            RedirectStandardError = false,// !shortcut.ShortcutRunas;
                            RedirectStandardOutput = false// !shortcut.ShortcutRunas;
                        };

                        if (shortcut.ShortcutType == ShortcutTypeEnum.Ps1)
                        {
                            processInfo.FileName = "powershell.exe";
                            processInfo.Arguments = $"-NoProfile -ExecutionPolicy ByPass -File \"{shortcut.ScriptFilePath}\"";
                        }
                        else if (shortcut.ShortcutType == ShortcutTypeEnum.Bat)
                        {
                            processInfo.FileName = shortcut.ScriptFilePath;
                        }

                        if (shortcut.ShortcutRunas)
                        {
                            processInfo.Verb = "runas";
                        }

                        var process = Process.Start(processInfo);

                        process.WaitForExit();
                    }
                    catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
                    finally
                    {
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            shortcut.Running = false;
                        });
                    }
                });
            }
        }

        /// <summary>
        /// 加载本机 Segoe Fluent Icons 字体内的所有图标
        /// </summary>
        public void LoadSegoeFluentIcons()
        {
            if (this.AllIcons.Count <= 0)
            {
                var icons = FontHelper.GetAllSegoeFluentIcons();
                foreach (var icon in icons)
                {
                    this.AllIcons.Add(icon);
                }
            }
        }

        /// <summary>
        /// 加载保存的脚本快捷项
        /// </summary>
        private async void LoadShortcuts()
        {
            try
            {
                _allShortcuts.Clear();
                this.GroupedShortcuts.Clear();
                _categories.Clear();
                this.Categories.Clear();

                try
                {
                    string shortcutsJson = await StorageFilesService.ReadFileAsync("shortcuts.json");
                    if (!string.IsNullOrWhiteSpace(shortcutsJson))
                    {
                        var shortcuts = JsonSerializer.Deserialize<ObservableCollection<ShortcutModel>>(shortcutsJson);

                        foreach (var shortcut in shortcuts)
                        {
                            if (string.IsNullOrWhiteSpace(shortcut.Category))
                            {
                                shortcut.Category = "未分类";
                            }

                            // 存储已有的分类名称，用于建议用户
                            if (!_categories.Contains(shortcut.Category))
                            {
                                _categories.Add(shortcut.Category);
                                this.Categories.Add(shortcut.Category);
                            }

                            _allShortcuts.Add(shortcut);
                        }

                        var groupedList =
                            (from item in _allShortcuts
                             group item by item.Category into newItems
                             select
                             new ShortcutsGroupModel
                             {
                                 Category = newItems.Key,
                                 Shortcuts = new(newItems.ToList())
                             }).OrderBy(x => x.Category).ToList();

                        foreach (var item in groupedList)
                        {
                            this.GroupedShortcuts.Add(item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex);
                }

                // 添加系统菜单
                this.GroupedShortcuts.Add(new ShortcutsGroupModel
                {
                    Category = "",
                    Shortcuts = new ObservableCollection<ShortcutModel>
                    {
                        new ShortcutModel
                        {
                            Category = "add",
                            ShortcutColor = ShortcutColorEnum.Transparent,
                            ShortcutType = ShortcutTypeEnum.None,
                            ShortcutName = "添加",
                            ShortcutIcon = "\uE710",
                            ShortcutRunas = false,
                            NoWindow = true,
                        },
                        new ShortcutModel
                        {
                            Category = "whatsnew",
                            ShortcutColor = ShortcutColorEnum.Transparent,
                            ShortcutType = ShortcutTypeEnum.None,
                            ShortcutName = "最近更新",
                            ShortcutIcon = "\uF133",
                            ShortcutRunas = false,
                            NoWindow = true,
                        },
                        new ShortcutModel
                        {
                            Category = "settings",
                            ShortcutColor = ShortcutColorEnum.Transparent,
                            ShortcutType = ShortcutTypeEnum.None,
                            ShortcutName = "设置",
                            ShortcutIcon = "\uE713",
                            ShortcutRunas = false,
                            NoWindow = true,
                        },
                    }
                });
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// 添加新的脚本快捷项并保存
        /// </summary>
        /// <param name="color"></param>
        /// <param name="iconIndex"></param>
        /// <param name="name"></param>
        /// <param name="ext"></param>
        public async void AddShortcut(string name, string category, int color, bool runas, bool noWindow, int iconIndex, string ext, string path)
        {
            try
            {
                _allShortcuts.Insert(0, new ShortcutModel()
                {
                    ScriptFilePath = path,
                    ShortcutName = name,
                    ShortcutIcon = this.AllIcons[iconIndex].Char,
                    ShortcutType = ext == ".ps1" ? ShortcutTypeEnum.Ps1 : ext == ".bat" ? ShortcutTypeEnum.Bat : ShortcutTypeEnum.None,
                    ShortcutColor = (ShortcutColorEnum)(Math.Max(1, Math.Min(color, 9))),
                    ShortcutRunas = runas,
                    NoWindow = noWindow,
                    Category = category,
                });

                string shortcutsSaveJson = JsonSerializer.Serialize(_allShortcuts);
                await StorageFilesService.WriteFileAsync("shortcuts.json", shortcutsSaveJson);

                LoadShortcuts();
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// 移除指定脚本快捷项并保存
        /// </summary>
        /// <param name="deletingShortcut"></param>
        public async void DelShortcut(ShortcutModel deletingShortcut)
        {
            try
            {
                string deleteFilePath = deletingShortcut.ScriptFilePath;

                _allShortcuts.Remove(deletingShortcut);

                if (File.Exists(deleteFilePath))
                {
                    File.Delete(deleteFilePath);
                }

                string shortcutsSaveJson = JsonSerializer.Serialize(_allShortcuts);
                await StorageFilesService.WriteFileAsync("shortcuts.json", shortcutsSaveJson);

                LoadShortcuts();
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
            }
        }
    }
}
