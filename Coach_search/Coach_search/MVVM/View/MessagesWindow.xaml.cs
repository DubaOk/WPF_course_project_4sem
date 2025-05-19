using MahApps.Metro.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Coach_search.MVVM.View
{
    public partial class MessagesWindow : MetroWindow
    {
        public MessagesWindow()
        {
            InitializeComponent();
            Loaded += MessagesWindow_Loaded;
            Closing += MessagesWindow_Closing;
        }

        private void MessagesWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is Coach_search.ViewModels.MessagesViewModel)
            {
                MessageScrollViewer.ScrollToBottom();
            }
        }

        private void MessagesWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DataContext is Coach_search.ViewModels.MessagesViewModel viewModel)
            {
                viewModel.Cleanup();
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (DataContext is Coach_search.ViewModels.MessagesViewModel viewModel)
            {
                viewModel.SendMessageOnEnter(e);
            }
        }
    }
}