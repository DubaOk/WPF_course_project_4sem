using System.Windows.Controls;
using Coach_search.Models;
using Coach_search.ViewModels;

namespace Coach_search.MVVM.View
{
    public partial class MainPage : Page
    {
        public MainPage(string username, UserType userType, int userId)
        {
            InitializeComponent();
            DataContext = new MainViewModel(username, userType, userId);
        }
    }
}