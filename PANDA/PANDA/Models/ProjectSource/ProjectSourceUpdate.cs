using MaterialDesignExtensions.Model;
using MaterialDesignThemes.Wpf;
using PANDA.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PANDA
{
    public partial class ProjectSourceHelper
    {
        // Members
        public Dictionary<string, ProjectSourceInfoItem> ProjectSourceDictionary = new Dictionary<string, ProjectSourceInfoItem>();

        // Instantiate a Singleton of the Semaphore with a value of 1. This means that only 1 thread can be granted access at a time.
        private static readonly SemaphoreSlim m_mutex = new SemaphoreSlim(1, 1);

        // ----------------------------------------------------------------------------------------
        // Class       : ProjectSourceHelper
        // Method      : GenerateProjectSourceInfoItem
        // Description : Determines if input path is file or folder and generates an appropriate
        //               ProjectSourceInfoItem (file or folder).
        // Parameters  :
        // - projectPath (string) : Path to project source
        // - path (string)        : Path to file or folder within project source
        // ----------------------------------------------------------------------------------------
        public ProjectSourceInfoItem GenerateProjectSourceInfoItem(string projectPath, string path)
        {
            // Create DirectoryInfo instance for populating ProjectSourceInfoItem
            DirectoryInfo dirInfo = new DirectoryInfo(path);

            // Temporary items to help initialize ProjectSourceInfoItem
            string VOBFullPath_   = dirInfo.FullName.Split(new string[] { projectPath }, StringSplitOptions.None)[1].ToString();
            string VOBParentPath_ = VOBFullPath_.Split(new string[] { dirInfo.Name }, StringSplitOptions.None)[0].ToString();

            // Create ProjectSourceInfoItem instance and then update it.
            ProjectSourceInfoItem infoItem = new ProjectSourceInfoItem
            {
                IsSelected    = false,
                DirInfo       = dirInfo,
                VOBFullPath   = VOBFullPath_,
                VOBParentPath = VOBParentPath_,
                Icon          = DetermineProjectSourceIcon(dirInfo),
                // Placeholder items
                SourceControlledStatus = SOURCE_CONTROLLED_STATUS.NOT_SOURCE_CONTROLLED,
                SourceVersion = 0
            };

            return infoItem;
        }

        // ----------------------------------------------------------------------------------------
        // Class       : ProjectSourceHelper
        // Method      : UpdateProjectSourceDictionary
        // Description : Updates the project source.
        // Parameters  :
        // - projectPath (string) : Input path to root project source folder
        // ----------------------------------------------------------------------------------------
        public void UpdateProjectSourceDictionary(string projectPath)
        {
            // The GetFiles method throws a variety of exceptions, catch them all and write to log
            try
            {
                // Clear Dictionary
                ProjectSourceDictionary.Clear();

                // Loop through all sub-directories and get all files
                foreach (var currentPath in Directory.GetFiles(projectPath, "*.*", SearchOption.AllDirectories))
                {
                    ProjectSourceDictionary.Add(currentPath, GenerateProjectSourceInfoItem(projectPath, currentPath));
                }

                // Update SearchSourceUnfiltered
                m_clearcaseViewTabCodeViewModel.SearchSourceUnfiltered = new ObservableCollection<ProjectSourceInfoItem>(ProjectSourceDictionary.Values.ToList());
            }
            // TODO: Implement console log
            catch (UnauthorizedAccessException) { }
            catch (ArgumentException) { }
            catch (DirectoryNotFoundException) { }
            catch (PathTooLongException) { }
            catch (IOException) { }
        }

        // ----------------------------------------------------------------------------------------
        // Class       : ProjectSourceHelper
        // Method      : DetermineProjectSourceIcon
        // Description : Returns the icon of the input DirectoryInfo.
        // Parameters  :
        // - infoItem (DirectoryInfo) : Input ProjectSourceInfoItem
        // ----------------------------------------------------------------------------------------
        public PackIconKind DetermineProjectSourceIcon(DirectoryInfo dirInfo)
        {
            // Update based on file or directory
            if (dirInfo.Attributes.HasFlag(FileAttributes.Directory))
            {
                return PackIconKind.Folder;
            }
            else
            {
                switch (dirInfo.Extension)
                {
                    case ".c"   : return PackIconKind.LanguageC;
                    case ".cs"  : return PackIconKind.LanguageCsharp;
                    case ".css" : return PackIconKind.LanguageCss3;
                    case ".csv" : return PackIconKind.FileCsv;
                    case ".cpp" : return PackIconKind.LanguageCpp;
                    case ".doc" : return PackIconKind.FileWord;
                    case ".docx": return PackIconKind.FileWord;
                    case ".h"   : return PackIconKind.AlphabetHBox;
                    case ".html": return PackIconKind.LanguageHtml5;
                    case ".java": return PackIconKind.LanguageJava;
                    case ".jpeg": return PackIconKind.FileImage;
                    case ".jpg" : return PackIconKind.FileImage;
                    case ".js"  : return PackIconKind.LanguageJavascript;
                    case ".json": return PackIconKind.Json;
                    case ".one" : return PackIconKind.Onenote;
                    case ".pcmp": return PackIconKind.AlphabetJBox;
                    case ".pdf" : return PackIconKind.FilePdf;
                    case ".penv": return PackIconKind.AlphabetJBox;
                    case ".pjov": return PackIconKind.AlphabetJBox;
                    case ".php" : return PackIconKind.LanguagePhp;
                    case ".png" : return PackIconKind.FileImage;
                    case ".pprc": return PackIconKind.AlphabetJBox;
                    case ".ppt" : return PackIconKind.FilePowerpoint;
                    case ".py"  : return PackIconKind.LanguagePythonText;
                    case ".txt" : return PackIconKind.AlphabetTBox;
                    case ".xaml": return PackIconKind.Xaml;
                    case ".xml" : return PackIconKind.FileXml;
                    case ".xslx": return PackIconKind.FileExcel;
                    default     : return PackIconKind.File;
                }
            }
        }
    }
}