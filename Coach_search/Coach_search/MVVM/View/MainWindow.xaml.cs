using MahApps.Metro.Controls;
using System.Windows.Controls;
using Coach_search.MVVM.View;
using Coach_search.Models;

namespace Coach_search
{
    public partial class MainWindow : MetroWindow
    {
        public Frame Frame => MainFrame;

        public MainWindow()
        {
            System.Diagnostics.Debug.WriteLine("MainWindow constructor called.");
            InitializeComponent();
            System.Diagnostics.Debug.WriteLine("MainWindow initialized.");
        }

        public void NavigateToMainPage(string userName, UserType userType, int userId)
        {
            System.Diagnostics.Debug.WriteLine($"Navigating to MainPage with userName={userName}, userType={userType}, userId={userId}");
            MainFrame.Navigate(new MainPage(userName, userType, userId));
            System.Diagnostics.Debug.WriteLine("Navigation to MainPage completed.");
        }
    }
}