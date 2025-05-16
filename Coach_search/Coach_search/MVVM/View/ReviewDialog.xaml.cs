using System.Windows;
using System.Windows.Controls;

namespace Coach_search.MVVM.View
{
    public partial class ReviewDialog : Window
    {
        public string ReviewText => ReviewTextBox.Text;
        public int Rating => int.Parse((RatingComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "1");
        public string TutorName { get; set; }

        public ReviewDialog(string tutorName = null)
        {
            InitializeComponent();
            TutorName = tutorName ?? "Не указан";
            DataContext = this;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}