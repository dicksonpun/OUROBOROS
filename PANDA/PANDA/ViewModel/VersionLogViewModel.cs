using System.Collections.Generic;

namespace PANDA.ViewModel
{
    public class VersionLogViewModel : ViewModel
    {
        private readonly List<VersionLogItem> m_items;
        
        public List<VersionLogItem> VersionLogItems
        {
            get
            {
                return m_items;
            }
        }

        public VersionLogViewModel() : base()
        {
            // Helpers for UNICODE characters
            string bullet      = "\u2022";
            string placeholder = "";

            m_items = new List<VersionLogItem>()
            {
                /* Semantic Versioning is a formal convention for specifying compatibility using a three-part version number: major version; minor version; and patch version. 
                 * The major version is incremented for changes which are not backward-compatible. 
                 * The minor version is incremented for releases which add new, but backward-compatible features. 
                 * The patch version is incremented for minor changes and bug fixes which do not change the software's features. 
                 */

                // Note: Keep TODO entry as top entry
                new VersionLogItem() { Version     = "TODOs - GUI Only",
                                       UpdateNotes = "CURRENTLY IN DEVELOPMENT"                                                          + "\n" +
                                                     bullet + "Create a helper class for initializing the Navigation Menu"               + "\n" +
                                                     "\t" + bullet + "Update: navigation menu to handle dynamic updates"                 + "\n" +
                                                     "\t" + bullet + "Update: navigation menu to respond to dynamic updates"             + "\n" +
                                                     bullet + "ADD: Clearcase Branch Manager layout, handle updates and load status"     + "\n" +
                                                     bullet + "Add vob periodic search"                                                  + "\n" +
                                                     "\nHIGH PRIORITY"                                                                   + "\n" +
                                                     bullet + "ADD: Clearcase Branch Helper layout"                                      + "\n" +
                                                     bullet + "ADD: function to add and remove views"                                    + "\n" +
                                                     bullet + "ADD: function update views"                                               + "\n" +                                                     
                                                     bullet + "ADD: Buttons to support: EDIT, VTREE, DIFF, REVIEW, CONFIGSPEC EDIT"      + "\n" +
                                                     bullet + "ADD: User Settings"                                                       + "\n" +
                                                     bullet + "Make data persistent between application launches"                        + "\n" + 
                                                     bullet + "ADD: Central Location for storing meta files"                             + "\n" +
                                                     bullet + "ADD: GITHUB to state the branching strategy"                              + "\n" +
                                                     bullet + "Tidy up includes section in all files prior to official release"          + "\n" +
                                                     "\nMEDIUM PRIORITY"                                                                 + "\n" +
                                                     bullet + "ADD: export and import project"                                           + "\n" +
                                                     bullet + "ADD: FUNCTION: Build Integrity Analyzer"                                  + "\n" +
                                                     bullet + "ADD: FUNCTION: BranchList CHANGE log document generator"                  + "\n" +
                                                     bullet + "ADD: export and import project"                                           + "\n" +
                                                     bullet + "ADD: Dashboard for branch statistics"                                     + "\n" +
                                                     bullet + "ADD: file review functionality"                                           + "\n" +
                                                     bullet + "ADD: Mount to Drive functionality"                                        + "\n" +
                                                     bullet + "ADD: AUTOSTAMP"                                                           + "\n" +
                                                     bullet + "ADD: Test Descriptor Verifier and Queue"                                  + "\n" +
                                                     bullet + "ADD: Test ETA"                                                            + "\n" +
                                                     "\nLOW PRIORITY"                                                                    + "\n" +
                                                     bullet + "ADD: HOW TO STEPPER SERIES"                                               + "\n" +
                                                     bullet + "ADD: Analysis tool: Octal scratchpad"                                     + "\n" +
                                                     bullet + "ADD: Analysis tool: CSV parser"                                           + "\n" +
                                                     bullet + "ADD: snackbar for notifications"                                          + "\n" +
                                                     bullet + "CONSIDER: ADD PIDGIN LIBRARY FOR PARSING SUPPORT"                         + "\n" +
                                                     bullet + "CONSIDER: ADD DYNAMIC DATA LIBRARY FOR reactive (rx) SUPPORT"             + "\n" +
                                                     placeholder },
                // Note: Keep TODO entry as top entry
                new VersionLogItem() { Version     = "Pinocchio Endgame",
                                       UpdateNotes = "CURRENTLY IN DEVELOPMENT"                                                          + "\n" +
                                                     "\nHIGH PRIORITY"                                                                   + "\n" +
                                                     bullet + "Implement functionality for real for the following:"                      + "\n" +
                                                     "\t" + bullet + "FUNCTION: "                         + "\n" +
                                                     "\t" + bullet + "FUNCTION: "                         + "\n" +
                                                     "\t" + bullet + "FUNCTION: "                         + "\n" +
                                                     "\t" + bullet + "FUNCTION: "                         + "\n" +
                                                     "\t" + bullet + "FUNCTION: "                         + "\n" +
                                                     "\t" + bullet + "FUNCTION: "                         + "\n" +
                                                     bullet + "ADD: Clearcase Branch Helper layout"                                      + "\n" +
                                                     bullet + "ADD: Clearcase Branch Manager layout"                                     + "\n" +
                                                     bullet + "ADD: function to add and remove views"                                    + "\n" +
                                                     bullet + "ADD: function update views"                                               + "\n" +
                                                     bullet + "ADD: Buttons to support: EDIT, VTREE, DIFF, REVIEW, CONFIGSPEC EDIT"      + "\n" +
                                                     bullet + "ADD: User Settings"                                                       + "\n" +
                                                     bullet + "Make data persistent between application launches"                        + "\n" +
                                                     bullet + "ADD: Central Location for storing meta files"                             + "\n" +
                                                     bullet + "ADD: GITHUB to state the branching strategy"                              + "\n" +
                                                     bullet + "Tidy up all files prior to official release"                              + "\n" +
                                                     "\nMEDIUM PRIORITY"                                                                 + "\n" +
                                                     bullet + "ADD: export and import project"                                           + "\n" +
                                                     bullet + "ADD: FUNCTION: Build Integrity Analyzer"                                  + "\n" +
                                                     bullet + "ADD: FUNCTION: BranchList CHANGE log document generator"                  + "\n" +
                                                     bullet + "ADD: Dashboard for branch statistics"                                     + "\n" +
                                                     bullet + "ADD: file review functionality"                                           + "\n" +
                                                     bullet + "ADD: Mount to Drive functionality"                                        + "\n" +
                                                     bullet + "ADD: AUTOSTAMP"                                                           + "\n" +
                                                     bullet + "ADD: Test Descriptor Verifier and Queue"                                  + "\n" +
                                                     bullet + "ADD: Test ETA"                                                            + "\n" +
                                                     "\nLOW PRIORITY"                                                                    + "\n" +
                                                     bullet + "ADD: HOW TO STEPPER SERIES"                                               + "\n" +
                                                     bullet + "ADD: Learning Seminars - Links to crashcourse PowerPoint"                 + "\n" +
                                                     bullet + "CONSIDER: ADD PIDGIN LIBRARY FOR PARSING SUPPORT"                         + "\n" +
                                                     bullet + "CONSIDER: ADD DYNAMIC DATA LIBRARY FOR reactive (rx) SUPPORT"             + "\n" +
                                                     bullet + "REQUEST : FOSS APPROVALS FOR 3RD PARTY LIBRARY SUPPORT"                   + "\n" +
                                                     placeholder },

                // Note: Version log entries are in descending order (latest version should be the top-most, following the TODO entry).
                new VersionLogItem() { Version     = "1.0.0",
                                       UpdateNotes = "INITIAL RELEASE"                                                                   + "\n" +
                                                     bullet + "Implemented persistent data between viewModel context switches"           + "\n" +
                                                     bullet + "Implemented Clearcase Manager"                                            + "\n" +
                                                     "\t" + bullet + "Implemented asynchronous periodic updates"                         + "\n" +
                                                     bullet + "Implemented Navigation Window"                                            + "\n" +
                                                     "\t" + bullet + "Implemented Navigation categories based on current detected mode"  + "\n" +
                                                     bullet + "Implemented Licenses log documentation"                                   + "\n" +
                                                     bullet + "Implemented Version log documentation"                                    + "\n" +
                                                     bullet + "Implemented Message Hub"                                                  + "\n" +
                                                     placeholder },
            };
        }
    }

    public class VersionLogItem : ViewModel
    {
        public string Version     { get; set; }
        public string UpdateNotes { get; set; }
        public VersionLogItem()   { }
    }
}