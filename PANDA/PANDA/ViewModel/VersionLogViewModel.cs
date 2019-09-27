using System.Collections.Generic;

namespace OUROBOROS.ViewModel
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