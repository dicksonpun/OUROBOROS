using System.Collections.Generic;

namespace PANDA.ViewModel
{
    public class LicenseListViewModel : ViewModel
    {
        private readonly List<LicenseListItem> m_items;
        
        public List<LicenseListItem> LicenseListItems
        {
            get
            {
                return m_items;
            }
        }

        public LicenseListViewModel() : base()
        {
            m_items = new List<LicenseListItem>()
            {
                // Keep alphanumerical order:
                new LicenseListItem() { PackageName = "Application Badges",       Description = "Badge Icons",                                             License = "CC0 1.0", Version = "N/A",        ReferenceURL = "https://github.com/badges/shields"                                   },
                new LicenseListItem() { PackageName = "Application Icon",         Description = "Panda Icon",                                              License = "Free",    Version = "Low-Res",    ReferenceURL = "https://www.freelogodesign.org/"                                     },
                new LicenseListItem() { PackageName = "DocX",                     Description = ".NET library for manipulating Word 2007/2010/2013 files", License = "MS-PL",   Version = "1.3.0",      ReferenceURL = "https://github.com/xceedsoftware/DocX"                               },
                new LicenseListItem() { PackageName = "ExcelDataReader",          Description = ".NET library for reading Microsoft Excel files",          License = "MIT",     Version = "3.5.0",      ReferenceURL = "https://github.com/ExcelDataReader/ExcelDataReader"                  },
                new LicenseListItem() { PackageName = "Jot",                      Description = ".NET library for application state persistence",          License = "MIT",     Version = "2.0.1",      ReferenceURL = "https://github.com/anakic/jot"                                       },
                new LicenseListItem() { PackageName = "MaterialDesignColors",     Description = ".NET library for Material Design color system",           License = "MS-PL",   Version = "1.1.3",      ReferenceURL = "https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit" },
                new LicenseListItem() { PackageName = "MaterialDesignExtensions", Description = ".NET library for additional Material Design controls",    License = "MIT",     Version = "2.4.0 ",     ReferenceURL = "https://spiegelp.github.io/MaterialDesignExtensions/"                },
                new LicenseListItem() { PackageName = "MaterialDesignThemes",     Description = ".NET library for Material Design theme and controls",     License = "MIT",     Version = "2.5.0.1205", ReferenceURL = "https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit" },
                new LicenseListItem() { PackageName = "Newtonsoft.Json",          Description = ".NET library for high-performance JSON framework",        License = "MIT",     Version = "12.0.1",     ReferenceURL = "https://github.com/JamesNK/Newtonsoft.Json"                          },
                new LicenseListItem() { PackageName = "SharpZipLib",              Description = ".NET library for file compression and decompression",     License = "MIT",     Version = "1.1.0",      ReferenceURL = "https://github.com/icsharpcode/SharpZipLib"                          },
                new LicenseListItem() { PackageName = "SSH.NET",                  Description = ".NET library for Secure Shell (SSH-2)",                   License = "MIT",     Version = "2016.1.0",   ReferenceURL = "https://github.com/sshnet/SSH.NET"                                   },
                new LicenseListItem() { PackageName = "TinyIOC",                  Description = ".NET library for Inversion of Control Container",         License = "MIT",     Version = "1.3.0",      ReferenceURL = "https://github.com/grumpydev/TinyIoC"                                },
                new LicenseListItem() { PackageName = "TinyMessenger",            Description = ".NET library for lightweight event aggregator",           License = "MS-PL",   Version = "1.0.0",      ReferenceURL = "https://github.com/grumpydev/TinyMessenger"                          }
            };
        }
    }

    public class LicenseListItem : ViewModel
    {
        public string PackageName  { get; set; }
        public string Description  { get; set; }
        public string License      { get; set; }
        public string Version      { get; set; }
        public string ReferenceURL { get; set; }
        public LicenseListItem()   { }
    }
}