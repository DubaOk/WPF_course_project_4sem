using Coach_search.MVVM.ViewModels;
using Coach_search.ViewModels;
using System.Windows.Controls;

namespace Coach_search.MVVM.View
{
    public partial class AdminPanelPage : Page
    {
        public AdminPanelPage(int adminId)
        {
            InitializeComponent();
            DataContext = new AdminPanelViewModel(adminId);
        }
    }
}