using MaterialDesignThemes.Wpf;
using PANDA.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PANDA.ViewModel
{
    public enum ThemeColors
    {
        Blue = 1,
        Cyan,
        DeepOrange,
        DeepPurple,
        Green,
        Indigo,
        LightBlue,
        Pink,
        Purple,
        Red,
        Teal
    }

    public class ColorSettingsViewModel : ViewModel
    {
        public ColorSettingsViewModel() : base()
        {
            LinkCommand();
            ResetSettings();
            SetColorThemeStatus();
        }

        #region Color Properties

        private ThemeColors _selectedColor;
        public ThemeColors SelectedColor
        {
            get { return _selectedColor; }
            set
            {
                _selectedColor = value;
                OnPropertyChanged(nameof(SelectedColor));
            }
        }

        private bool _deepOrangeActive;
        public bool DeepOrangeActive
        {
            get { return _deepOrangeActive; }
            set
            {
                _deepOrangeActive = value;
                OnPropertyChanged(nameof(DeepOrangeActive));
            }
        }

        private bool _lightBlueActive;
        public bool LightBlueActive
        {
            get { return _lightBlueActive; }
            set
            {
                _lightBlueActive = value;
                OnPropertyChanged(nameof(LightBlueActive));
            }
        }

        private bool _tealActive;
        public bool TealActive
        {
            get { return _tealActive; }
            set
            {
                _tealActive = value;
                OnPropertyChanged(nameof(TealActive));
            }
        }

        private bool _cyanActive;
        public bool CyanActive
        {
            get { return _cyanActive; }
            set
            {
                _cyanActive = value;
                OnPropertyChanged(nameof(CyanActive));
            }
        }

        private bool _pinkActive;
        public bool PinkActive
        {
            get { return _pinkActive; }
            set
            {
                _pinkActive = value;
                OnPropertyChanged(nameof(PinkActive));
            }
        }

        private bool _greenActive;
        public bool GreenActive
        {
            get { return _greenActive; }
            set
            {
                _greenActive = value;
                OnPropertyChanged(nameof(GreenActive));
            }
        }

        private bool _deepPurpleActive;
        public bool DeepPurpleActive
        {
            get { return _deepPurpleActive; }
            set
            {
                _deepPurpleActive = value;
                OnPropertyChanged(nameof(DeepPurpleActive));
            }
        }

        private bool _indigoActive;
        public bool IndigoActive
        {
            get { return _indigoActive; }
            set
            {
                _indigoActive = value;
                OnPropertyChanged(nameof(IndigoActive));
            }
        }

        private bool _blueActive;
        public bool BlueActive
        {
            get { return _blueActive; }
            set
            {
                _blueActive = value;
                OnPropertyChanged(nameof(BlueActive));
            }
        }

        private bool _redActive;
        public bool RedActive
        {
            get { return _redActive; }
            set
            {
                _redActive = value;
                OnPropertyChanged(nameof(RedActive));
            }
        }

 

        private bool _purpleActive;
        public bool PurpleActive
        {
            get { return _purpleActive; }
            set
            {
                _purpleActive = value;
                OnPropertyChanged(nameof(PurpleActive));
            }
        }


        private string _themeName;
        public string ThemeName
        {
            get
            {
                return _themeName;
            }
            set
            {
                if (_themeName != value)
                {
                    _themeName = value;
                    OnPropertyChanged(nameof(ThemeName));
                }
            }
        }

        private bool _darkMode;
        public bool DarkMode
        {
            get { return _darkMode; }
            set
            {
                _darkMode = value;
                OnPropertyChanged(nameof(DarkMode));
            }
        }


        // RelayCommands
        public RelayCommand SetThemeCommand { get; private set; }
        public RelayCommand SetDarkModeCommand { get; private set; }
        public RelayCommand SaveSettingsCommand { get; private set; }

        #endregion

        #region Events

        private void SetTheme(object o)
        {
            Enum.TryParse(o.ToString(), out ThemeColors selectedColor);
            _selectedColor = selectedColor;
            SetTheme();
            SetColorThemeStatus();
            SaveSettings();
        }

        private void SetDarkMode(object o)
        {
            SetDarkMode(); 
            SetColorThemeStatus();
            SaveSettings();
        }

        private void SaveSettings(object o)
        {
            SaveSettings();
        }

        #endregion

        #region Methods

        private void LinkCommand()
        {
            SetThemeCommand     = new RelayCommand(SetTheme);
            SetDarkModeCommand  = new RelayCommand(SetDarkMode);
            SaveSettingsCommand = new RelayCommand(SaveSettings);
        }

        private void SetColorThemeStatus()
        {
            ThemeName        = _darkMode ? "Dark" : "Light";

            RedActive        = _selectedColor == ThemeColors.Red;
            PinkActive       = _selectedColor == ThemeColors.Pink;
            DeepOrangeActive = _selectedColor == ThemeColors.DeepOrange;
            GreenActive      = _selectedColor == ThemeColors.Green;
            TealActive       = _selectedColor == ThemeColors.Teal;
            BlueActive       = _selectedColor == ThemeColors.Blue;
            LightBlueActive  = _selectedColor == ThemeColors.LightBlue;
            CyanActive       = _selectedColor == ThemeColors.Cyan;
            PurpleActive     = _selectedColor == ThemeColors.Purple;
            DeepPurpleActive = _selectedColor == ThemeColors.DeepPurple; 
            IndigoActive     = _selectedColor == ThemeColors.Indigo;
        }

        public void ResetSettings()
        {
            DarkMode      = true;
            SelectedColor = ThemeColors.Indigo;
        }

        public void SetTheme()
        {
            try
            {
                new PaletteHelper().ReplaceAccentColor(_selectedColor.ToString());
                new PaletteHelper().ReplacePrimaryColor(_selectedColor.ToString());
            }
            catch
            {
                ResetSettings();
                new PaletteHelper().ReplaceAccentColor(_selectedColor.ToString());
                new PaletteHelper().ReplacePrimaryColor(_selectedColor.ToString());
            }
        }

        public void SetDarkMode()
        {
            try
            {
                new PaletteHelper().SetLightDark(DarkMode);
            }
            catch
            {
                ResetSettings();
                new PaletteHelper().SetLightDark(DarkMode);
            }
        }

        public static void SaveSettings()
        {
            try
            {
                // TODO:  USE JOT
            }
            catch
            {

            }
        }

        #endregion
    }
}
