using System.Windows;
using System.Windows.Threading;

namespace Coach_search
{
    public partial class App : Application
    {
        public App()
        {
            DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"Unhandled exception: {e.Exception.Message}\nStackTrace: {e.Exception.StackTrace}");
            MessageBox.Show($"Произошла ошибка: {e.Exception.Message}", "Ошибка");
            e.Handled = true;
        }
    }
}