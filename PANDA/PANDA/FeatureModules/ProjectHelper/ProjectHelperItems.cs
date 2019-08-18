using MaterialDesignExtensions.Model;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PANDA.ViewModel
{
    public class ProjectHelperSourceItem
    {
        public bool IsSelected { get; set; }
        public bool IsLocal { get; set; }
        public bool IsCheckout { get; set; }
        public bool IsBranchListing { get; set; }
        public DirectoryInfo DirInfo { get; set; }
        public ProjectHelperSourceItem()
        {
            IsSelected = false;
            IsLocal = false;
            IsCheckout = false;
            IsBranchListing = false;
        }
    }

    public class VOBItem
    {
        public string Name { get; set; }
        public VOBItem() { }
    }

    public class VOBAutocompleteSource : IAutocompleteSource
    {
        private readonly List<VOBItem> m_VOBItems;
        public VOBAutocompleteSource()
        {
            m_VOBItems = new List<VOBItem>()
            {
                new VOBItem() { Name = "calculator-master" },
                new VOBItem() { Name = "evt-master" },
                new VOBItem() { Name = "GTS-GamesTaskScheduler-master" },
                new VOBItem() { Name = "mXtract-master" },
                new VOBItem() { Name = "randomCppProject" },
                new VOBItem() { Name = "zippedProjects" },
            };
        }

        public IEnumerable Search(string searchTerm)
        {
            searchTerm = searchTerm ?? string.Empty;
            searchTerm = searchTerm.ToLower();

            return m_VOBItems.Where(item => item.Name.ToLower().Contains(searchTerm));
        }
    }

    public class ViewAutocompleteSource : IAutocompleteSource
    {
        private readonly List<DirectoryInfo> m_VOBItems;
        public ViewAutocompleteSource()
        {
            m_VOBItems = new List<DirectoryInfo>();

            var ViewDirectory = @"C:\Users\Dickson\Desktop\testViews\";
            var ViewNames = Directory.GetDirectories(ViewDirectory).ToList();

            foreach (string viewName in ViewNames)
            {
                m_VOBItems.Add(new DirectoryInfo(Path.Combine(ViewDirectory, viewName)));
            }

        }

        public IEnumerable Search(string searchTerm)
        {
            searchTerm = searchTerm ?? string.Empty;
            searchTerm = searchTerm.ToLower();

            return m_VOBItems.Where(item => item.Name.ToLower().Contains(searchTerm));
        }
    }
}
