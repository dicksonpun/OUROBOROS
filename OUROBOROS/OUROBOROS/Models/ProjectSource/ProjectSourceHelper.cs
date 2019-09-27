using MaterialDesignExtensions.Model;
using MaterialDesignThemes.Wpf;
using OUROBOROS.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace OUROBOROS
{
    public partial class ProjectSourceHelper
    {
        // Members
        private string m_projectPath;
        private ClearcaseViewTabCodeViewModel m_clearcaseViewTabCodeViewModel;

        // Constructors
        public ProjectSourceHelper(ClearcaseViewTabCodeViewModel clearcaseViewTabCodeViewModel, string projectPath)
        {
            m_clearcaseViewTabCodeViewModel = clearcaseViewTabCodeViewModel;
            m_projectPath = projectPath;

            UpdateProjectSourceDictionary(m_projectPath);
        }
    };

}
