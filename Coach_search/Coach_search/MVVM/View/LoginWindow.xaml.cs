using MahApps.Metro.Controls;
using System.Windows;
using System.Windows.Controls;
using Coach_search.ViewModels;

namespace Coach_search.MVVM.View
{
    public partial class LoginWindow : MetroWindow
    {
        public LoginWindow()
        {
            InitializeComponent();
            DataContext = new LoginViewModel(); // Устанавливаем DataContext
        }

        private void LoginPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel viewModel)
            {
                viewModel.LoginPassword = (sender as PasswordBox)?.Password;
            }
        }

        private void RegisterPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel viewModel)
            {
                viewModel.RegisterPassword = (sender as PasswordBox)?.Password;
            }
        }
    }
}