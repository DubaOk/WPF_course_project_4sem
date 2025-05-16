using Coach_search.ViewModels;
using System.Windows;

namespace Coach_search.MVVM.View
{
    public partial class TutorDetailsWindow : Window
    {
        public TutorDetailsWindow(TutorDetailsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}