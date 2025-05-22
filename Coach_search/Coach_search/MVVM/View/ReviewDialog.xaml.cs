using System.Windows;
using System.Windows.Controls;

namespace Coach_search.MVVM.View
{
    public partial class ReviewDialog : Window
    {
        private const int MaxCharacters = 100;
        
        public string ReviewText => ReviewTextBox.Text;
        public int Rating => int.Parse((RatingComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "1");
        public string TutorName { get; set; }

        public ReviewDialog(string tutorName = null)
        {
            InitializeComponent();
            TutorName = tutorName ?? "Не указан";
            DataContext = this;
        }

        private void ReviewTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int remainingChars = MaxCharacters - ReviewTextBox.Text.Length;
            CharacterCounterTextBlock.Text = $"Осталось символов: {remainingChars}";
            
            if (remainingChars <= 20)
            {
                CharacterCounterTextBlock.Foreground = remainingChars <= 0 
                    ? System.Windows.Media.Brushes.Red 
                    : System.Windows.Media.Brushes.Orange;
            }
            else
            {
                CharacterCounterTextBlock.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ReviewText))
            {
                MessageBox.Show("Пожалуйста, введите текст отзыва.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

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