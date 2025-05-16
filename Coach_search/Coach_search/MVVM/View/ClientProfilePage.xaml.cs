using Coach_search.ViewModels;
using System.Windows.Controls;
using System.Windows;

namespace Coach_search.MVVM.View
{
    public partial class ClientProfilePage : Page
    {
        public ClientProfilePage(int userId)
        {
            InitializeComponent();
            if (userId <= 0)
            {
                MessageBox.Show("Неверный ID пользователя.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            DataContext = new ClientProfileViewModel(userId);
        }
    }
}