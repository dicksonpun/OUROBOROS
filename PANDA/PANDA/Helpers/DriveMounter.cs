using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using PANDA.ViewModel;

namespace PANDA
{
    public class DriveMounter
    {
        private readonly string DriveLetter;
        private string DrivePath { get; set; }

        public VolumeFunctions volumeFunctions;

        public DriveMounter(string driveLetter)
        {
            volumeFunctions = new VolumeFunctions();
            DriveLetter = driveLetter;
            DrivePath = volumeFunctions.DriveIsMappedTo(driveLetter);
        }

        public void Mount(string newPath)
        {
            // Only mount if path changed
            if (DrivePath != newPath)
            {
                // Unmount old path, if it exists
                if (Directory.Exists(DrivePath))
                {
                    volumeFunctions.UnmapFolderFromDrive(DriveLetter, DrivePath);
                }

                // Update to new path
                DrivePath = newPath;

                // Mount new path, if it exists
                if (Directory.Exists(DrivePath))
                {
                    volumeFunctions.MapFolderToDrive(DriveLetter, DrivePath);
                }
            }
        }

        public class VolumeFunctions
        {
            [DllImport("kernel32.dll")]
            static extern bool DefineDosDevice(uint dwFlags, string lpDeviceName, string lpTargetPath);

            [DllImport("Kernel32.dll")]
            internal static extern uint QueryDosDevice(string lpDeviceName, StringBuilder lpTargetPath, uint ucchMax);

            // Class Constants
            internal const uint DDD_RAW_TARGET_PATH       = 0x00000001;
            internal const uint DDD_REMOVE_DEFINITION     = 0x00000002;
            internal const uint DDD_EXACT_MATCH_ON_REMOVE = 0x00000004;
            internal const uint DDD_NO_BROADCAST_SYSTEM   = 0x00000008;

            const string MAPPED_FOLDER_INDICATOR = @"\??\";

            // ----------------------------------------------------------------------------------------
            // Class       : VolumeFunctions
            // Method      : MapFolderToDrive
            // Description : Map the folder to a drive letter
            // Parameters  :
            // - driveLetter (string) : Drive letter in the format "Y:" without a back slash
            // - folderName (string)  : Folder to map without a back slash
            // ----------------------------------------------------------------------------------------
            public void MapFolderToDrive(string driveLetter, string folderName)
            {
                // Is this drive already mapped? If so, we don't remap it!
                StringBuilder volumeMap = new StringBuilder(1024);
                QueryDosDevice(driveLetter, volumeMap, (uint)1024);
                if (volumeMap.ToString().StartsWith(MAPPED_FOLDER_INDICATOR))
                    return;

                // Map the folder to the drive
                if (!DefineDosDevice(0, driveLetter, folderName))
                {
                    throw new Exception("ERROR: Mounting Drive");
                }
            }

            // ----------------------------------------------------------------------------------------
            // Class       : VolumeFunctions
            // Method      : UnmapFolderFromDrive
            // Description : Unmap a drive letter, without checking the folder name.
            // Parameters:
            // - driveLetter (string) : Drive letter to be released, the format "Y:"
            // - folderName (string)  : Folder name that the drive is mapped to.
            // ----------------------------------------------------------------------------------------
            public void UnmapFolderFromDrive(string driveLetter, string folderName)
            {
                // Unmount existing drive path
                if (!DefineDosDevice(DDD_REMOVE_DEFINITION, driveLetter, folderName))
                {
                    throw new Exception("ERROR: Unmounting Drive");
                }
            }

            // ----------------------------------------------------------------------------------------
            // Class       : VolumeFunctions
            // Method      : DriveIsMappedTo
            // Description : Returns the folder that a drive is mapped to. If not mapped, return a blank.
            // Parameters  :
            // - driveLetter (string) : Drive letter in the format "Y:"
            // ----------------------------------------------------------------------------------------
            public string DriveIsMappedTo(string driveLetter)
            {
                StringBuilder volumeMap = new StringBuilder(512);
                string mappedVolumeName = "";

                // If it's not a mapped drive, just remove it from the list
                uint mapped = QueryDosDevice(driveLetter, volumeMap, (uint)512);
                if (mapped != 0)
                {
                    if (volumeMap.ToString().StartsWith(MAPPED_FOLDER_INDICATOR))
                    {
                        // It's a mapped drive, so return the mapped folder name
                        mappedVolumeName = volumeMap.ToString().Substring(4);
                    }
                }
                return mappedVolumeName;
            }
        }
    }
}
