using System.Collections.Generic;

namespace PANDA.ViewModel
{
    public class VersionLogViewModel : ViewModel
    {
        // Databinding
        private readonly List<VersionLogItem> m_items;
        public List<VersionLogItem> VersionLogItems
        {
            get { return m_items; }
        }

        public VersionLogViewModel() : base()
        {
            string END_OF_STRING = "";

            /* Semantic Versioning is a formal convention for specifying compatibility using a three-part version number: MAJOR.MINOR.PATCH
             * The major version is incremented for changes which are not backward-compatible. 
             * The minor version is incremented for releases which add new, but backward-compatible features. 
             * The patch version is incremented for minor changes and bug fixes which do not change the software's features. 
             */

            // Note: Keep KNOWN ISSUES and TODO entry as top entries until they are fully resolved
            m_items = new List<VersionLogItem>()
            {
                new VersionLogItem()
                {
                    Version     = "KNOWN ISSUES",
                    UpdateNotes = new List<VersionLogNoteItem>()
                    {
                        new VersionLogNoteItem()
                        {
                            Header       =  "CURRENTLY IN DEVELOPMENT",
                            Description  =  "N/A"                                                                       + "\n" +
                                            END_OF_STRING
                        },
                        new VersionLogNoteItem()
                        {
                            Header       =  "HIGH PRIORITY",
                            Description  =  "N/A"                                                                       + "\n" +
                                            END_OF_STRING
                        },
                        new VersionLogNoteItem()
                        {
                            Header       =  "MEDIUM PRIORITY",
                            Description  =  "N/A"                                                                       + "\n" +
                                            END_OF_STRING
                        },
                        new VersionLogNoteItem()
                        {
                            Header       =  "LOW PRIORITY",
                            Description  =  "Dark Mode in Navigation Menu is not fully supported yet."                  + "\n" +
                                            END_OF_STRING
                        }
                    }
                },
                new VersionLogItem()
                {
                    Version     = "TODO - GUI Prototype",
                    UpdateNotes = new List<VersionLogNoteItem>()
                    {
                        new VersionLogNoteItem()
                        {
                            Header       =  "CURRENTLY IN DEVELOPMENT",
                            Description  =  "Add App State Persistence"                                                 + "\n" +
                                            "+ Central Location for storing meta files"                                 + "\n" +
                                            END_OF_STRING
                        },
                        new VersionLogNoteItem()
                        {
                            Header       =  "HIGH PRIORITY",
                            Description  =  "Add FileSystemWatcher for monitoring file and folder changes"              + "\n" +
                                            "ADD: Text Search"                                                          + "\n" +
                                            END_OF_STRING
                        },
                        new VersionLogNoteItem()
                        {
                            Header       =  "MEDIUM PRIORITY",
                            Description  =  "ADD: function update views - config spec"                                  + "\n" +
                                            "Tidy up all files prior to official release (Documentation)"               + "\n" +
                                            END_OF_STRING
                        },
                        new VersionLogNoteItem()
                        {
                            Header       =  "LOW PRIORITY",
                            Description  =  "ADD: HOW TO STEPPER SERIES"                                                + "\n" +
                                            "ADD: Analysis tool: Octal scratchpad"                                      + "\n" +
                                            "ADD: Analysis tool: CSV parser"                                            + "\n" +
                                            END_OF_STRING
                        }
                    }
                },
                new VersionLogItem()
                {
                    Version     = "TODO - Functionality",
                    UpdateNotes = new List<VersionLogNoteItem>()
                    {
                        new VersionLogNoteItem()
                        {
                            Header       =  "CURRENTLY IN DEVELOPMENT",
                            Description  =  "Consider re-designing update algorithm"                                    + "\n" +
                                            END_OF_STRING
                        },
                        new VersionLogNoteItem()
                        {
                            Header       =  "HIGH PRIORITY",
                            Description  =  "Update logic to support updates via cleartool commands"                    + "\n" +
                                            END_OF_STRING
                        },
                        new VersionLogNoteItem()
                        {
                            Header       =  "MEDIUM PRIORITY",
                            Description  =  "ADD: FUNCTION: import project"                                             + "\n" +
                                            "ADD: FUNCTION: Build Integrity Analyzer"                                   + "\n" +
                                            "ADD: FUNCTION: BranchList CHANGE log document generator"                   + "\n" +
                                            "ADD: Dashboard for branch statistics"                                      + "\n" +
                                            "ADD: file review functionality"                                            + "\n" +
                                            "ADD: AUTOSTAMP"                                                            + "\n" +
                                            "ADD: Test Descriptor Verifier and Queue"                                   + "\n" +
                                            "ADD: Test ETA"                                                             + "\n" +
                                            END_OF_STRING
                        },
                        new VersionLogNoteItem()
                        {
                            Header       =  "LOW PRIORITY",
                            Description  =  "ADD: HOW TO Wiki (Using Stepper)"                                          + "\n" +
                                            "ADD: Powerpoint directory"                                                 + "\n" +
                                            "Button to create user views / branch"                                      + "\n" +
                                            END_OF_STRING
                        }
                    }
                },
                /* Add new versions ascending from here. */
                new VersionLogItem()
                {
                    Version     = "1.0.0",
                    UpdateNotes = new List<VersionLogNoteItem>()
                    {
                        new VersionLogNoteItem()
                        {
                            Header       =  "Main Window",
                            Description  =  "Navigation Menu - Clearcase Views support asynchronous periodic updates"   + "\n" +
                                            "+ Updates list of available views and external sources referencing views"  + "\n" +
                                            "Snackbar operational for notifications"                                    + "\n" +
                                            END_OF_STRING
                        },
                        new VersionLogNoteItem()
                        {
                            Header       =  "User Profile",
                            Description  =  "GUI for UNIX login"                                                        + "\n" +
                                            "Theme Picker"                                                              + "\n" +
                                            END_OF_STRING
                        },
                        new VersionLogNoteItem()
                        {
                            Header       =  "Clearcase Manager",
                            Description  =  "Asynchronous periodic updates"                                             + "\n" +
                                            "+ Updates list of available views externally via Navigation Menu updates"  + "\n" +
                                            "Searchbar to add views"                                                    + "\n" +
                                            "Button to mount and unmount folder to Y-Drive"                             + "\n" +
                                            "Button to remove views"                                                    + "\n" +
                                            "+ Nonuser views are removed from navigation menu"                          + "\n" +
                                            "+ User views tentatively only collect user confirmation to delete"         + "\n" +
                                            END_OF_STRING
                        },
                        new VersionLogNoteItem()
                        {
                            Header       =  "Project Source Helper",
                            Description  =  "Searchbar for project source"                                              + "\n" +
                                            "Button to open selected item(s)"                                           + "\n" +
                                            "Double click to open selected item"                                        + "\n" +
                                            END_OF_STRING
                        },
                        new VersionLogNoteItem()
                        {
                            Header       =  "Version Log",
                            Description  =  "Add TODOs and pending V1.0.0 notes"                                        + "\n" +
                                            END_OF_STRING
                        },
                        new VersionLogNoteItem()
                        {
                            Header       =  "Licenses",
                            Description  =  "Add documentation"                                                         + "\n" +
                                            END_OF_STRING
                        },
                    }
                }
            };
        }

        public class VersionLogItem : ViewModel
        {
            public string Version { get; set; }
            public List<VersionLogNoteItem> UpdateNotes { get; set; }
            public VersionLogItem() { }
        }

        public class VersionLogNoteItem : ViewModel
        {
            public string Header { get; set; }
            public string Description { get; set; }
            public VersionLogNoteItem() { }
        }
    }
}