using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Windows.Storage;

namespace Conscripts.Helpers
{
    public partial class SettingsService : ObservableObject
    {
        private const string SETTING_NAME_APPEARANCEINDEX = "AppearanceIndex";
        private const string SETTING_NAME_BACKDROPINDEX = "BackdropIndex";
        private const string SETTING_NAME_ONESHOTMODE = "IsOneShotModeEnabled";

        private readonly ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;

        public event EventHandler<int>? AppearanceSettingChanged;

        public event EventHandler<int>? BackdropSettingChanged;

        private int _appearanceIndex = -1;

        private int _backdropIndex = -1;

        private bool? _oneShotEnabled = null;

        /// <summary>
        /// App's Theme, 0-System 1-Dark 2-Light.
        /// </summary>
        public int AppearanceIndex
        {
            get
            {
                try
                {
                    if (_appearanceIndex < 0)
                    {
                        if (_localSettings.Values[SETTING_NAME_APPEARANCEINDEX] == null)
                        {
                            _appearanceIndex = 0;
                        }
                        else if (_localSettings.Values[SETTING_NAME_APPEARANCEINDEX]?.ToString() == "0")
                        {
                            _appearanceIndex = 0;
                        }
                        else if (_localSettings.Values[SETTING_NAME_APPEARANCEINDEX]?.ToString() == "1")
                        {
                            _appearanceIndex = 1;
                        }
                        else if (_localSettings.Values[SETTING_NAME_APPEARANCEINDEX]?.ToString() == "2")
                        {
                            _appearanceIndex = 2;
                        }
                        else
                        {
                            _appearanceIndex = 0;
                        }
                    }
                }
                catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
                if (_appearanceIndex < 0) _appearanceIndex = 0;
                return _appearanceIndex < 0 ? 0 : _appearanceIndex;
            }
            set
            {
                SetProperty(ref _appearanceIndex, value);
                ApplicationData.Current.LocalSettings.Values[SETTING_NAME_APPEARANCEINDEX] = _appearanceIndex;
                AppearanceSettingChanged?.Invoke(this, _appearanceIndex);
            }
        }

        /// <summary>
        /// App's Backdrop Material, 0-Mica 1-MicaAlt 2-Acrylic.
        /// </summary>
        public int BackdropIndex
        {
            get
            {
                try
                {
                    if (_backdropIndex < 0)
                    {
                        if (_localSettings.Values[SETTING_NAME_BACKDROPINDEX] == null)
                        {
                            _backdropIndex = 0;
                        }
                        else if (_localSettings.Values[SETTING_NAME_BACKDROPINDEX]?.ToString() == "0")
                        {
                            _backdropIndex = 0;
                        }
                        else if (_localSettings.Values[SETTING_NAME_BACKDROPINDEX]?.ToString() == "1")
                        {
                            _backdropIndex = 1;
                        }
                        else if (_localSettings.Values[SETTING_NAME_BACKDROPINDEX]?.ToString() == "2")
                        {
                            _backdropIndex = 2;
                        }
                        else
                        {
                            _backdropIndex = 0;
                        }
                    }
                }
                catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
                if (_backdropIndex < 0) _backdropIndex = 0;
                return _backdropIndex < 0 ? 0 : _backdropIndex;
            }
            set
            {
                SetProperty(ref _backdropIndex, value);
                ApplicationData.Current.LocalSettings.Values[SETTING_NAME_BACKDROPINDEX] = _backdropIndex;
                BackdropSettingChanged?.Invoke(this, _backdropIndex);
            }
        }

        /// <summary>
        /// Indicates whether the one-shot mode is enabled.
        /// </summary>
        public bool OneShotEnabled
        {
            get
            {
                try
                {
                    if (_oneShotEnabled is null)
                    {
                        if (_localSettings.Values[SETTING_NAME_ONESHOTMODE] == null)
                        {
                            _oneShotEnabled = false;
                        }
                        else if (_localSettings.Values[SETTING_NAME_ONESHOTMODE]?.ToString() == "True")
                        {
                            _oneShotEnabled = true;
                        }
                        else
                        {
                            _oneShotEnabled = false;
                        }
                    }
                }
                catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
                _oneShotEnabled ??= false;
                return _oneShotEnabled ?? false;
            }
            set
            {
                SetProperty(ref _oneShotEnabled, value);
                ApplicationData.Current.LocalSettings.Values[SETTING_NAME_ONESHOTMODE] = _oneShotEnabled;
            }
        }
    }

}
