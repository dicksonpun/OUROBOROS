using System.Collections.Generic;

namespace PANDA.ViewModel
{
    public class LicenseLogViewModel : ViewModel
    {
        private readonly List<LicenseLogItem> m_items;
        
        public List<LicenseLogItem> LicenseLogItems
        {
            get
            {
                return m_items;
            }
        }

        public LicenseLogViewModel() : base()
        {
            m_items = new List<LicenseLogItem>()
            {
                // Keep alphanumerical order:
                new LicenseLogItem() { PackageName = "Application Icon",         Description = "Panda Icon",                                              License = "Free",    Version = "LOW-RES",  ReferenceURL = "https://www.freelogodesign.org/"                                     },
                new LicenseLogItem() { PackageName = "DocX",                     Description = ".NET library for manipulating Word 2007/2010/2013 files", License = "MS-PL",   Version = "1.3.0",    ReferenceURL = "https://github.com/xceedsoftware/DocX"                               },
                new LicenseLogItem() { PackageName = "ExcelDataReader",          Description = ".NET library for reading Microsoft Excel files",          License = "MIT",     Version = "3.6.0",    ReferenceURL = "https://github.com/ExcelDataReader/ExcelDataReader"                  },
                new LicenseLogItem() { PackageName = "Jot",                      Description = ".NET library for application state persistence",          License = "MIT",     Version = "2.0.1",    ReferenceURL = "https://github.com/anakic/jot"                                       },
                new LicenseLogItem() { PackageName = "MaterialDesignColors",     Description = ".NET library for Material Design color system",           License = "MS-PL",   Version = "1.2.0",    ReferenceURL = "https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit" },
                new LicenseLogItem() { PackageName = "MaterialDesignExtensions", Description = ".NET library for additional Material Design controls",    License = "MIT",     Version = "2.7.0 ",   ReferenceURL = "https://github.com/spiegelp/MaterialDesignExtensions"                },
                new LicenseLogItem() { PackageName = "MaterialDesignThemes",     Description = ".NET library for Material Design theme and controls",     License = "MIT",     Version = "2.6.0",    ReferenceURL = "https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit" },
                new LicenseLogItem() { PackageName = "Newtonsoft.Json",          Description = ".NET library for high-performance JSON framework",        License = "MIT",     Version = "12.0.2",   ReferenceURL = "https://github.com/JamesNK/Newtonsoft.Json"                          },
                new LicenseLogItem() { PackageName = "SharpZipLib",              Description = ".NET library for file compression and decompression",     License = "MIT",     Version = "1.1.0",    ReferenceURL = "https://github.com/icsharpcode/SharpZipLib"                          },
                new LicenseLogItem() { PackageName = "SSH.NET",                  Description = ".NET library for Secure Shell (SSH-2)",                   License = "MIT",     Version = "2016.1.0", ReferenceURL = "https://github.com/sshnet/SSH.NET"                                   }
            };
        }
    }

    public class LicenseLogItem : ViewModel
    {
        public string PackageName  { get; set; }
        public string Description  { get; set; }
        public string License      { get; set; }
        public string Version      { get; set; }
        public string ReferenceURL { get; set; }
        public LicenseLogItem()   { }
    }
}