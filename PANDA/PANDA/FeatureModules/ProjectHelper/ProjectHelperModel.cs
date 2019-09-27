using OUROBOROS.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Threading;

namespace OUROBOROS.Model
{
    public class ProjectHelperModel
    {
        // Supporting class
        public class UpdateQueueItem
        {
            // Members
            public WatcherChangeTypes WatcherChangeType;
            public ProjectHelperSourceItem ProjectSourceItem;
            // Constructor
            public UpdateQueueItem(WatcherChangeTypes watcherChangeType_, ProjectHelperSourceItem primarySourceItem_)
            {
                WatcherChangeType = watcherChangeType_;
                ProjectSourceItem = primarySourceItem_;
            }
        }

        // Members
        public string ViewFullPath;
        public DirectoryInfo DirInfo;
        public FileSystemWatcher FileSystemWatcher;

        // The following requires access under lock:
        private static readonly SemaphoreSlim m_mutex = new SemaphoreSlim(1, 1);
        public Queue<UpdateQueueItem> UpdateQueue;

        // Default constructor
        public ProjectHelperModel()
        {
            ViewFullPath = string.Empty;
            FileSystemWatcher = new FileSystemWatcher();
            UpdateQueue = new Queue<UpdateQueueItem>();
        }
        public ProjectHelperModel(string viewFullPath)
        {
            ViewFullPath = viewFullPath;
            DirInfo = new DirectoryInfo(ViewFullPath);
            UpdateQueue = new Queue<UpdateQueueItem>();

            // Generate initial list of items to add
            List<string> FilePathsToAdd = Directory.GetFiles(ViewFullPath, "*.*", SearchOption.AllDirectories).ToList();
            List<string> FolderPathsToAdd = Directory.GetDirectories(ViewFullPath, "*", SearchOption.AllDirectories).ToList();
            List<string> CombinedPathsToAdd = FilePathsToAdd.Concat(FolderPathsToAdd).ToList();

            foreach (string itemPath in CombinedPathsToAdd)
            {
                GenerateAndAddEntryToUpdateQueue(itemPath, WatcherChangeTypes.All); // NOTE: All is used for initialization ONLY.
            }

            InitializeFileSystemWatcher();
        }

        // Reference: https://weblogs.asp.net/ashben/31773 (FileSystemWatcher Tips)
        // Reference: https://www.c-sharpcorner.com/uploadfile/puranindia/filesystemwatcher-in-C-Sharp/
        // Reference: https://stackoverflow.com/questions/1764809/filesystemwatcher-changed-event-is-raised-twice
        private void InitializeFileSystemWatcher()
        {
            try
            {
                // Create a new FileSystemWatcher and set its properties.
                FileSystemWatcher = new FileSystemWatcher
                {
                    Path = ViewFullPath,

                    // Watch both files and subdirectories.
                    IncludeSubdirectories = true,

                    // Watch for all changes specified in the NotifyFilters enumeration.
                    NotifyFilter = NotifyFilters.Attributes |
                                   NotifyFilters.CreationTime |
                                   NotifyFilters.DirectoryName |
                                   NotifyFilters.FileName |
                                   NotifyFilters.LastWrite,

                    // Watch all files.
                    Filter = "*.*",

                    // Set buffer size to 64KB (Max allowable buffer).
                    // Note: A 4 KB (DEFAULT) buffer can track changes on approximately 80 files in a directory
                    // Note: A 64 KB buffer can track changes on approximately 1280 files in a directory
                    InternalBufferSize = 65536
                };

                // Register a handler that gets called when a file is created, changed, or deleted.
                FileSystemWatcher.Changed += new FileSystemEventHandler(OnChanged);
                FileSystemWatcher.Created += new FileSystemEventHandler(OnChanged);
                FileSystemWatcher.Deleted += new FileSystemEventHandler(OnChanged);

                // Register a handler that gets called when a file is renamed.
                FileSystemWatcher.Renamed += new RenamedEventHandler(OnRenamed);

                // Register a handler that gets called if the FileSystemWatcher needs to report an error.
                FileSystemWatcher.Error += new ErrorEventHandler(OnError);

                // Start monitoring.
                FileSystemWatcher.EnableRaisingEvents = true;

            }
            catch (Exception e)
            {
                Console.WriteLine("A Exception Occurred :" + e.ToString());
            }
        }

        // Event Handlers
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed.
            Console.WriteLine("{0}: {1} ON PATH: {2}", e.ChangeType, e.Name, e.FullPath);

            GenerateAndAddEntryToUpdateQueue(e.FullPath, e.ChangeType);
        }
        private void OnRenamed(object source, RenamedEventArgs e)
        {
            // Specify what is done when a file is renamed.
            Console.WriteLine("{0}: {1} to {2}", WatcherChangeTypes.Renamed, e.OldFullPath, e.FullPath);

            // Remove the old item and add the new item.
            GenerateAndAddEntryToUpdateQueue(e.OldFullPath, WatcherChangeTypes.Deleted);
            GenerateAndAddEntryToUpdateQueue(e.FullPath, WatcherChangeTypes.Created);
        }
        private void OnError(object source, ErrorEventArgs e)
        {
            // Show that an error has been detected.
            Console.WriteLine("The FileSystemWatcher has detected an error");
            // Give more information if the error is due to an internal buffer overflow.
            if (e.GetException().GetType() == typeof(InternalBufferOverflowException))
            {
                // Reference: https://stackoverflow.com/questions/22116374/filesystemwatcher-internalbufferoverflow
                // This can happen if Windows is reporting many file system events quickly 
                // and internal buffer of the  FileSystemWatcher is not large enough to handle this
                // rate of events. The InternalBufferOverflowException error informs the application
                // that some of the file system events are being lost.
                Console.WriteLine(("The file system watcher experienced an internal buffer overflow: " + e.GetException().Message));
            }
        }

        public void GenerateAndAddEntryToUpdateQueue(string itemPath, WatcherChangeTypes watcherChangeType)
        {
            // TODO
            // Determine SourceControlStatus
            // Determine SourceVersion
            UpdateQueueItem updateQueueItem = new UpdateQueueItem(watcherChangeType, new ProjectHelperSourceItem()
            {
                DirInfo = new DirectoryInfo(itemPath),
                IsSelected = false,
            });
            // Add to Queue under lock
            UpdateQueue.Enqueue(updateQueueItem);
        }
    }
}
