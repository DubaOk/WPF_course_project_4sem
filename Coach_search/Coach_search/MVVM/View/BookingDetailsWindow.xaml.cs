using MahApps.Metro.Controls;
using Coach_search.ViewModels;

namespace Coach_search.MVVM.View
{
    public partial class BookingDetailsWindow : MetroWindow
    {
        public BookingDetailsWindow(BookingDetailsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}