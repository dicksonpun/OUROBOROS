using System;
using System.IO;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;

namespace PANDA
{
    public class PaletteSelectorViewModel
    {
        public PaletteSelectorViewModel()
        {
            Swatches = new SwatchesProvider().Swatches;
        }

        public ICommand ToggleStyleCommand { get; } = new AnotherCommandImplementation(o => ApplyStyle((bool)o));

        public ICommand ToggleBaseCommand { get; } = new AnotherCommandImplementation(o => ApplyBase((bool)o));

        public IEnumerable<Swatch> Swatches { get; }

        public ICommand ApplyPrimaryCommand { get; } = new AnotherCommandImplementation(o => ApplyPrimary((Swatch)o));

        public ICommand ApplyAccentCommand { get; } = new AnotherCommandImplementation(o => ApplyAccent((Swatch)o));

        public ICommand LoadPaletteCommand { get; } = new AnotherCommandImplementation(o => LoadPalette());

        public ICommand SavePrimarySwatchCommand { get; } = new AnotherCommandImplementation(o => SavePrimarySwatch((Swatch)o));

        private static void ApplyStyle(bool alternate)
        {
            var resourceDictionary = new ResourceDictionary
            {
                Source = new Uri(@"pack://application:,,,/Dragablz;component/Themes/materialdesign.xaml")
            };

            var styleKey = alternate ? "MaterialDesignAlternateTabablzControlStyle" : "MaterialDesignTabablzControlStyle";
            var style = (Style)resourceDictionary[styleKey];

            foreach (var tabablzControl in Dragablz.TabablzControl.GetLoadedInstances())
            {
                tabablzControl.Style = style;
            }
        }

        public static void ApplyDarkBase(bool isDark = true)
        {
            new PaletteHelper().SetLightDark(isDark);
        }

        private static void ApplyBase(bool isDark)
        {
            new PaletteHelper().SetLightDark(isDark);
        }

        private static void ApplyPrimary(Swatch swatch)
        {
            new PaletteHelper().ReplacePrimaryColor(swatch);
        }

        private static void ApplyAccent(Swatch swatch)
        {
            new PaletteHelper().ReplaceAccentColor(swatch);
        }

        public static void SavePrimarySwatch(Swatch swatch)
        {
            new PaletteHelper().ReplacePrimaryColor(swatch);
            Properties.Settings.Default.UserPrimarySwatch = swatch.ToString();
            Properties.Settings.Default.Save();
        }

        public static void LoadPrimarySwatch()
        {
            new PaletteHelper().ReplacePrimaryColor(Properties.Settings.Default.UserPrimarySwatch);
        }

        private static void SavePalette()
        {
            /*
            //open file stream
            StreamWriter file = File.CreateText(@"C:\Users\Dickson\Desktop\test.txt");
            try
            {
                JsonSerializer serializer = new JsonSerializer();
                //serialize object directly into file stream
                serializer.Serialize(file, currentPalette);
            }
            finally
            {
                file.Close();
            }
            */
        }

        public static void LoadPalette()
        {
            string jsonString = File.ReadAllText(@"C:\Users\Dickson\Desktop\test.txt");
            Palette currentPalette = JsonConvert.DeserializeObject<Palette>(jsonString);
            new PaletteHelper().ReplacePalette(currentPalette);
        }
    }
}
