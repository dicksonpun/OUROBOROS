using System.Collections.Generic;

namespace PANDA.ViewModel
{
    public class MainWindowViewModel : ViewModel
    {
        private readonly List<MainWindowItem> m_items;
        
        public List<MainWindowItem> VersionLogItems
        {
            get
            {
                return m_items;
            }
        }

        public MainWindowViewModel() : base()
        {
            // Helpers for UNICODE characters
            string bullet = "\u2022";

            m_items = new List<MainWindowItem>()
            {
                /* Semantic Versioning is a formal convention for specifying compatibility using a three-part version number: major version; minor version; and patch version. 
                 * The major version is incremented for changes which are not backward-compatible. 
                 * The minor version is incremented for releases which add new, but backward-compatible features. 
                 * The patch version is incremented for minor changes and bug fixes which do not change the software's features. 
                 */

                // Note: Keep TODO entry as top entry
                new MainWindowItem() { Version     = "TODOs",
                                       UpdateNotes = bullet + "Make data persistent between views."                                     + "\n" +
                                                     bullet + "Make data persistent between application launches"                       + "\n" +
                                                     bullet + "ADD: Central Location for storing meta files"                            + "\n" +
                                                     bullet + "ADD: Clearcase Branch Helper layout"                                     + "\n" +
                                                     bullet + "ADD: Buttons to support: EDIT, VTREE, DIFF, REVIEW, CONFIGSPEC EDIT"     + "\n" +
                                                     bullet + "ADD: Clearcase Branch Manager layout"                                    + "\n" +
                                                     bullet + "ADD: function to add and remove views"                                   + "\n" +
                                                     bullet + "ADD: function update views"                                              + "\n" +
                                                     bullet + "ADD: export and import project"                                          + "\n" +
                                                     bullet + "ADD: FUNCTION: Build Integrity Analyzer"                                 + "\n" +
                                                     bullet + "ADD: snackbar for notifications"                                         + "\n" +
                                                     bullet + "ADD: file review functionality"                                          + "\n" +
                                                     bullet + "ADD: Mount to Drive functionality"                                       + "\n" +
                                                     bullet + "ADD: AUTOSTAMP"                                                          + "\n" +
                                                     bullet + "ADD: Test Descriptor Verifier and Queue"                                 + "\n" +
                                                     bullet + "ADD: Test ETA"                                                           + "\n" +
                                                     bullet + "ADD: User Settings"                                                      + "\n" +
                                                     bullet + "ADD: HOW TO STEPPER SERIES"                                              + "\n" +
                                                     bullet + "ADD: GITHUB to state the branching strategy"                             + "\n" +
                                                     bullet + "ADD: Analysis tool: Octal scratchpad"                                    + "\n" +
                                                     bullet + "ADD: Analysis tool: CSV parser"                                          + "\n" +
                                                     bullet + "ADD: BranchList change log document generator"                           + "\n" +
                                                     bullet + "ADD: Dashboard for branch statistics"                                    + "\n" +
                                                     bullet + "ADD: Learning Seminars - Links to crashcourse PowerPoint"                + "\n" +
                                                     bullet + "CONSIDER: ADD PIDGIN LIBRARY FOR PARSING SUPPORT"                        + "\n" +
                                                     bullet + "CONSIDER: ADD DYNAMIC DATA LIBRARY FOR reactive (rx) SUPPORT"            + "\n" +
                                                     bullet + "REQUEST : FOSS APPROVALS FOR 3RD PARTY LIBRARY SUPPORT"
                                                     }, // PLACEHOLDER

                // Note: Version log entries are in descending order (latest version should be the top-most, following the TODO entry).
                new MainWindowItem() { Version     = "1.0.0",
                                       UpdateNotes = "Initial release"                                                                   + "\n" +
                                                     bullet + "Implemented Navigation Window"                                            + "\n" +
                                                     "\t" + bullet + "Implemented Navigation categories based on current detected mode." + "\n" +
                                                     bullet + "Implemented Licenses documentation."                                      + "\n" +
                                                     bullet + "Implemented Version log."
                                                     + "" /* PLACEHOLDER */ },
            };
        }
    }

    public class MainWindowItem : ViewModel
    {
        public string Version     { get; set; }
        public string UpdateNotes { get; set; }
        public MainWindowItem()   { }
    }
}