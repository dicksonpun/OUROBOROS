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

            // Note: Keep "Known Issues" and "TODO" entry as top entries.
            m_items = new List<VersionLogItem>()
            {
                #region Known Issues
                new VersionLogItem()
                {
                    Version     = "Known Issues",
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
                            Description  =  "N/A"                                                                       + "\n" +
                                            END_OF_STRING
                        }
                    }
                },
            #endregion
            #region TODO
                new VersionLogItem()
                {
                    Version     = "TODO",
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
                            Description  =  "Tidy up all files prior to official release (Documentation)"               + "\n" +
                                            END_OF_STRING
                        },
                        new VersionLogNoteItem()
                        {
                            Header       =  "MEDIUM PRIORITY",
                            Description  =  "[Add] File review functionality"                                           + "\n" +
                                            "[Add] State Persistence for View and VOBs"                                 + "\n" +
                                            END_OF_STRING
                        },
                        new VersionLogNoteItem()
                        {
                            Header       =  "LOW PRIORITY",
                            Description  =  "[Add] FUNCTION: BranchList CHANGE log document generator"                   + "\n" +
                                            "[Add] Text Search"                                                          + "\n" +
                                            "[Add] HOW TO Helper"                                                        + "\n" +
                                            "[Add] Powerpoint directory"                                                 + "\n" +
                                            "[Add] AUTOSTAMP"                                                            + "\n" +
                                            "[Add] Test Descriptor Verifier and Queue"                                   + "\n" +
                                            "[Add] Test ETA"                                                             + "\n" +
                                            "[Add] FUNCTION: Build Integrity Analyzer"                                   + "\n" +
                                            END_OF_STRING
                        }
                    }
                },
                #endregion
                #region 0.0.1
                /* Add new versions in descending order (latest on top). */
                new VersionLogItem()
                {
                    Version     = "0.0.1",
                    UpdateNotes = new List<VersionLogNoteItem>()
                    {
                        new VersionLogNoteItem()
                        {
                            Header       =  "Main Window",
                            Description  =  "[Added] Navigation Menu"                                                   + "\n" +
                                            "[Added] Snackbar notifications"                                            + "\n" +
                                            END_OF_STRING
                        },
                        new VersionLogNoteItem()
                        {
                            Header       =  "User Profile",
                            Description  =  "[Added] GUI for UNIX login"                                                + "\n" +
                                            END_OF_STRING
                        },
                        new VersionLogNoteItem()
                        {
                            Header       =  "Palette Picker",
                            Description  =  "[Added] GUI"                                                               + "\n" +
                                            "[Added] State Persistence for Color and Theme"                             + "\n" +
                                            END_OF_STRING
                        },
                        new VersionLogNoteItem()
                        {
                            Header       =  "Project Helper",
                            Description  =  "[Added] Searchbar"                                                         + "\n" +
                                            "[Added] Filter toggles for checkouts and branchlisting (not operational)"  + "\n" +
                                            "[Added] Review Status button (not operational)"                            + "\n" +
                                            "[Added] Settings (to select view and vobs)"                                + "\n" +
                                            "[Added] Mount Y Drive button (must select view)"                           + "\n" +
                                            "[Added] Launch IDE button (must select view) (not operational)"            + "\n" +
                                            "[Added] Context Menu (only open files is operational)"                     + "\n" +
                                            END_OF_STRING
                        },
                        new VersionLogNoteItem()
                        {
                            Header       =  "Changelog",
                            Description  =  "[Added] Categories: Known Issues, TODOs, and Version notes"                + "\n" +
                                            END_OF_STRING
                        },
                        new VersionLogNoteItem()
                        {
                            Header       =  "Licenses",
                            Description  =  "[Added] Documentation"                                                     + "\n" +
                                            END_OF_STRING
                        },
                    }
                }
                #endregion
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