using System.Windows.Controls;
using Coach_search.ViewModels;

namespace Coach_search.MVVM.View
{
    public partial class SchedulePage : Page
    {
        public SchedulePage(int tutorId, Frame navigationFrame)
        {
            InitializeComponent();
            DataContext = new ScheduleViewModel(tutorId, navigationFrame);
        }
    }
}