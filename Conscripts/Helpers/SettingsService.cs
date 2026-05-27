using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Windows.Storage;

namespace Conscripts.Helpers
{
    public partial class SettingsService : ObservableObject
    {
        private const string Settings_Appearance = "AppearanceIndex";
        private const string Settings_Backdrop = "BackdropIndex";
        private const string Settings_OneShot = "IsOneShotModeEnabled";

        private readonly ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;

        public event EventHandler<int>? AppearanceSettingChanged;

        public event EventHandler<int>? BackdropSettingChanged;

        private int _appearanceIndex = -1;

        private int _backdropIndex = -1;

        private bool? _isOneShotModeEnabled = null;

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
                        if (_localSettings.Values[Settings_Appearance] == null)
                        {
                            _appearanceIndex = 0;
                        }
                        else if (_localSettings.Values[Settings_Appearance]?.ToString() == "0")
                        {
                            _appearanceIndex = 0;
                        }
                        else if (_localSettings.Values[Settings_Appearance]?.ToString() == "1")
                        {
                            _appearanceIndex = 1;
                        }
                        else if (_localSettings.Values[Settings_Appearance]?.ToString() == "2")
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
                _localSettings.Values[Settings_Appearance] = _appearanceIndex;
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
                        if (_localSettings.Values[Settings_Backdrop] == null)
                        {
                            _backdropIndex = 0;
                        }
                        else if (_localSettings.Values[Settings_Backdrop]?.ToString() == "0")
                        {
                            _backdropIndex = 0;
                        }
                        else if (_localSettings.Values[Settings_Backdrop]?.ToString() == "1")
                        {
                            _backdropIndex = 1;
                        }
                        else if (_localSettings.Values[Settings_Backdrop]?.ToString() == "2")
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
                _localSettings.Values[Settings_Backdrop] = _backdropIndex;
                BackdropSettingChanged?.Invoke(this, _backdropIndex);
            }
        }

        /// <summary>
        /// Indicates whether the one-shot mode is enabled.
        /// </summary>
        public bool IsOneShotModeEnabled
        {
            get
            {
                try
                {
                    if (_isOneShotModeEnabled is null)
                    {
                        if (_localSettings.Values[Settings_OneShot] == null)
                        {
                            _isOneShotModeEnabled = false;
                        }
                        else if (_localSettings.Values[Settings_OneShot]?.ToString() == "True")
                        {
                            _isOneShotModeEnabled = true;
                        }
                        else
                        {
                            _isOneShotModeEnabled = false;
                        }
                    }
                }
                catch (Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
                _isOneShotModeEnabled ??= false;
                return _isOneShotModeEnabled ?? false;
            }
            set
            {
                SetProperty(ref _isOneShotModeEnabled, value);
                _localSettings.Values[Settings_OneShot] = _isOneShotModeEnabled;
            }
        }
    }
}
