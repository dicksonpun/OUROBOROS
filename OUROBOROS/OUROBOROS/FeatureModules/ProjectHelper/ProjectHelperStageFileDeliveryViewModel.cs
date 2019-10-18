using MaterialDesignExtensions.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUROBOROS.ViewModel
{
    public class ProjectHelperStageFileDeliveryViewModel : ViewModel
    {
        #region Databinding
        public ObservableCollection<ReviewerItem> SelectedFiles { get; }

        public IAutocompleteSource ReviewersAutocompleteSource { get; }

        private ReviewerItem m_selectedReviewer;
        public ReviewerItem SelectedReviewer
        {
            get { return m_selectedReviewer; }

            set
            {
                m_selectedReviewer = value;
                OnPropertyChanged(nameof(SelectedReviewer));

                AddToSelectedReviewers(value);
            }
        }

        public ObservableCollection<ReviewerItem> SelectedReviewers { get; }

        private bool m_isReviewerSelectionVisible;
        public bool IsReviewerSelectionVisible
        {
            get { return m_isReviewerSelectionVisible; }

            set
            {
                m_isReviewerSelectionVisible = value;
                OnPropertyChanged(nameof(IsReviewerSelectionVisible));
            }
        }

        private string m_deliveryComments;
        public string DeliveryComments
        {
            get { return m_deliveryComments; }

            set
            {
                m_deliveryComments = value;
                OnPropertyChanged(nameof(DeliveryComments));
            }
        }

        private bool m_isFormalCodeReview;
        public bool IsFormalCodeReview
        {
            get { return m_isFormalCodeReview; }

            set
            {
                m_isFormalCodeReview = value;
                OnPropertyChanged(nameof(IsFormalCodeReview));
            }
        }

        private bool m_isIgnoreIdenticalPredecessor;
        public bool IsIgnoreIdenticalPredecessor
        {
            get { return m_isIgnoreIdenticalPredecessor; }

            set
            {
                m_isIgnoreIdenticalPredecessor = value;
                OnPropertyChanged(nameof(IsIgnoreIdenticalPredecessor));
            }
        }
        #endregion

        #region Methods
        public void AddToSelectedReviewers(ReviewerItem item)
        {
            // Add to SelectedReviewers if the value is non-null and not already on the list
            if (!SelectedReviewers.Contains(item) && item != null)
            {
                SelectedReviewers.Add(item);
                OnPropertyChanged(nameof(SelectedReviewers));

                UpdateIsReviewerSelectionVisible();
            }
        }
        public void RemoveFromSelectedReviewers(ReviewerItem item)
        {
            // Remove From SelectedVOBs if the value is on the list
            if (SelectedReviewers.Contains(item))
            {
                SelectedReviewers.Remove(item);
                OnPropertyChanged(nameof(SelectedReviewers));

                UpdateIsReviewerSelectionVisible();
            }
        }
        public void UpdateIsReviewerSelectionVisible()
        {
            int maxNumberOfReviewers = 3;
            IsReviewerSelectionVisible = SelectedReviewers.Count().Equals(maxNumberOfReviewers) ? false : true;
        }
        #endregion

        #region Constructor
        public ProjectHelperStageFileDeliveryViewModel()
        {
            // Selected Files
            SelectedFiles = new ObservableCollection<ReviewerItem>() 
            {
                new ReviewerItem() { Name = @"/Default/Path/LOOOOOOOOOOOOOOOOOOOOOOONG/filename1" },
                new ReviewerItem() { Name = @"/Default/Path/LOOOOOOOOOOOOOOOOOOOOOOONG/filename2" },
                new ReviewerItem() { Name = @"/Default/Path/LOOOOOOOOOOOOOOOOOOOOOOONG/filename3" },
                new ReviewerItem() { Name = @"/Default/Path/LOOOOOOOOOOOOOOOOOOOOOOONG/filename4" },
                new ReviewerItem() { Name = @"/Default/Path/LOOOOOOOOOOOOOOOOOOOOOOONG/filename5" },
                new ReviewerItem() { Name = @"/Default/Path/LOOOOOOOOOOOOOOOOOOOOOOONG/filename6" },
                new ReviewerItem() { Name = @"/Default/Path/LOOOOOOOOOOOOOOOOOOOOOOONG/filename7" },
                new ReviewerItem() { Name = @"/Default/Path/LOOOOOOOOOOOOOOOOOOOOOOONG/filename8" },
                new ReviewerItem() { Name = @"/Default/Path/LOOOOOOOOOOOOOOOOOOOOOOONG/filename9" },
                new ReviewerItem() { Name = @"/Default/Path/LOOOOOOOOOOOOOOOOOOOOOOONG/filename10" },
                new ReviewerItem() { Name = @"/Default/Path/SHORT/filename1" },
                new ReviewerItem() { Name = @"/Default/Path/SHORT/filename2" },
                new ReviewerItem() { Name = @"/Default/Path/SHORT/filename3" },
                new ReviewerItem() { Name = @"/Default/Path/SHORT/filename4" },
            };

            // Reviewers
            m_isReviewerSelectionVisible = true;
            m_selectedReviewer = null;
            ReviewersAutocompleteSource = new ReviewerAutocompleteSource();
            SelectedReviewers = new ObservableCollection<ReviewerItem>();

            // Checkbox Options
            m_isFormalCodeReview = false;
            m_isIgnoreIdenticalPredecessor = false;
        }
        #endregion
    }

    public class ReviewerItem
    {
        public string Name { get; set; }
        public ReviewerItem() { }
    }

    public class ReviewerAutocompleteSource : IAutocompleteSource
    {
        private readonly List<ReviewerItem> m_reviewerList;
        public ReviewerAutocompleteSource()
        {
            m_reviewerList = new List<ReviewerItem>()
            {
                new ReviewerItem() { Name = "Reviewer One" },
                new ReviewerItem() { Name = "Reviewer 2" },
                new ReviewerItem() { Name = "Reviewer 3" },
                new ReviewerItem() { Name = "Reviewer LOOOOOOOOOOOONG NAME 1" },
                new ReviewerItem() { Name = "Reviewer LOOOOOOOOOOOONG NAME 2" },
                new ReviewerItem() { Name = "Reviewer LOOOOOOOOOOOONG NAME 3" }
            };
        }

        public IEnumerable Search(string searchTerm)
        {
            searchTerm = searchTerm ?? string.Empty;
            searchTerm = searchTerm.ToLower();

            return m_reviewerList.Where(item => item.Name.ToLower().Contains(searchTerm));
        }
    }
}
