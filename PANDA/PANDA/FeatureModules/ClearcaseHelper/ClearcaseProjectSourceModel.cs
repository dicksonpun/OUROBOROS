using OUROBOROS.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Threading;

namespace OUROBOROS.Model
{
    public class ClearcaseProjectSourceModel
    {
        // Supporting class
        public class UpdateQueueItem
        {
            // Members
            public WatcherChangeTypes WatcherChangeType;
            public ClearcaseProjectSourceItem ProjectSourceItem;
            // Constructor
            public UpdateQueueItem(WatcherChangeTypes watcherChangeType_, ClearcaseProjectSourceItem primarySourceItem_)
            {
                WatcherChangeType = watcherChangeType_;
                ProjectSourceItem = primarySourceItem_;
            }
        }

        // Members
        public string ViewName;
        public string ViewDirectory;
        public string ViewFullPath;
        public DirectoryInfo DirInfo;
        public FileSystemWatcher FileSystemWatcher;

        // The following requires access under lock:
        private static readonly SemaphoreSlim m_mutex = new SemaphoreSlim(1, 1);
        private readonly Queue<UpdateQueueItem> m_updateQueue;

        // Default constructor
        public ClearcaseProjectSourceModel(string viewName)
        {
            ViewName      = viewName;
            ViewDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            ViewFullPath  = ViewDirectory;
            DirInfo       = new DirectoryInfo(ViewFullPath);

            m_updateQueue   = new Queue<UpdateQueueItem>();

            // Generate initial list of items to add
            List<string> FilePathsToAdd     = Directory.GetFiles(ViewFullPath, "*.*", SearchOption.AllDirectories).ToList();
            List<string> FolderPathsToAdd   = Directory.GetDirectories(ViewFullPath, "*", SearchOption.AllDirectories).ToList();
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
                                   NotifyFilters.LastAccess |
                                   NotifyFilters.LastWrite |
                                   NotifyFilters.Security |
                                   NotifyFilters.Size,

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

        // ----------------------------------------------------------------------------------------
        // Class       : ClearcaseProjectSourceModel
        // Method      : RequestLock
        // Description : Asynchronously wait to enter the Semaphore. 
        //               If no-one has been granted access to the Semaphore, code execution will proceed.
        //               Otherwise, this thread waits here until the m_mutex is released.
        // ----------------------------------------------------------------------------------------
        public async void RequestLock()
        {
            await m_mutex.WaitAsync();
        }
        // ----------------------------------------------------------------------------------------
        // Class       : ClearcaseProjectSourceModel
        // Method      : ReleaseLock
        // Description : TLDR: Make sure you ALWAYS call this function from the 'finally' clause of a try-catch-finally statement.
        //               When the task is finished, release the m_mutex.
        //               It is vital to ALWAYS release the m_mutex when ready or else the Semaphore is deadlocked.
        //               This is why it is important to do the Release within a try...finally clause; program execution may crash or 
        //               take a different path, this way you are guaranteed execution of release.
        // ----------------------------------------------------------------------------------------
        public void ReleaseLock()
        {
            m_mutex.Release();
        }
        // ----------------------------------------------------------------------------------------
        // Class       : ClearcaseProjectSourceModel
        // Method      : EnqueueUnderLock
        // Description : Enqueue new item to UpdateQueue under lock.
        // ----------------------------------------------------------------------------------------
        public void EnqueueUnderLock(UpdateQueueItem item)
        {
            RequestLock();
            m_updateQueue.Enqueue(item);
            ReleaseLock();
        }
        // ----------------------------------------------------------------------------------------
        // Class       : ClearcaseProjectSourceModel
        // Method      : DequeueUnderLock
        // Description : Dequeue next item to UpdateQueue under lock.
        // ----------------------------------------------------------------------------------------
        public UpdateQueueItem DequeueUnderLock()
        {
            RequestLock();
            UpdateQueueItem item = m_updateQueue.Dequeue();
            ReleaseLock();
            return item;
        }
        // ----------------------------------------------------------------------------------------
        // Class       : ClearcaseProjectSourceModel
        // Method      : GetUpdateQueueCountUnderLock
        // Description : Get UpdateQueue's count under lock.
        // ----------------------------------------------------------------------------------------
        public int GetUpdateQueueCountUnderLock()
        {
            RequestLock();
            int count = m_updateQueue.Count();
            ReleaseLock();
            return count;
        }

        public void GenerateAndAddEntryToUpdateQueue(string itemPath, WatcherChangeTypes watcherChangeType)
        {
            // TODO
            // Determine SourceControlStatus
            // Determine SourceVersion
            UpdateQueueItem updateQueueItem = new UpdateQueueItem(watcherChangeType, new ClearcaseProjectSourceItem()
            {
                DirInfo = new DirectoryInfo(itemPath),
                IsSelected = false,
                SourceControlStatus = SOURCE_CONTROL_STATUS.NOT_IN_SOURCE_CONTROL, // TODO
                SourceVersion = 0 // TODO
            });
            // Add to Queue under lock
            EnqueueUnderLock(updateQueueItem);
        }
    }
}
