using MaterialDesignThemes.Wpf;
using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace OUROBOROS.Converters
{
    public class FileExtensionToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Convert input value into DirectoryInfo object
            DirectoryInfo dirInfo = (DirectoryInfo)value;

            // Return icon based on file or directory
            if (dirInfo.Attributes.HasFlag(FileAttributes.Directory))
            {
                return PackIconKind.Folder;
            }
            else
            {
                switch (dirInfo.Extension)
                {
                    case ".c"       : return PackIconKind.LanguageC;
                    case ".cs"      : return PackIconKind.LanguageCsharp;
                    case ".css"     : return PackIconKind.LanguageCss3;
                    case ".csv"     : return PackIconKind.FileCsv;
                    case ".cpp"     : return PackIconKind.LanguageCpp;
                    case ".doc"     : return PackIconKind.FileWord;
                    case ".docx"    : return PackIconKind.FileWord;
                    case ".h"       : return PackIconKind.AlphabetHBox;
                    case ".html"    : return PackIconKind.LanguageHtml5;
                    case ".java"    : return PackIconKind.LanguageJava;
                    case ".jpeg"    : return PackIconKind.FileImage;
                    case ".jpg"     : return PackIconKind.FileImage;
                    case ".js"      : return PackIconKind.LanguageJavascript;
                    case ".json"    : return PackIconKind.Json;
                    case ".one"     : return PackIconKind.Onenote;
                    case ".pcmp"    : return PackIconKind.AlphabetJBox;
                    case ".pdf"     : return PackIconKind.FilePdf;
                    case ".penv"    : return PackIconKind.AlphabetJBox;
                    case ".pjov"    : return PackIconKind.AlphabetJBox;
                    case ".php"     : return PackIconKind.LanguagePhp;
                    case ".png"     : return PackIconKind.FileImage;
                    case ".pprc"    : return PackIconKind.AlphabetJBox;
                    case ".ppt"     : return PackIconKind.FilePowerpoint;
                    case ".py"      : return PackIconKind.LanguagePythonText;
                    case ".txt"     : return PackIconKind.AlphabetTBox;
                    case ".xaml"    : return PackIconKind.Xaml;
                    case ".xml"     : return PackIconKind.FileXml;
                    case ".xslx"    : return PackIconKind.FileExcel;
                    default         : return PackIconKind.File;
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
