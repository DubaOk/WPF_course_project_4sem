using Coach_search.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Coach_search.ViewModels
{
    public class EnrollViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseHelper _dbHelper;
        private readonly int _clientId;
        private readonly int _tutorId;
        private readonly string _tutorName;
        private DateTime? _selectedDate;
        private string _selectedTime;
        private List<string> _availableTimes;

        public string TutorName => _tutorName;

        public DateTime? SelectedDate
        {
            get => _selectedDate;
            set
            {
                _selectedDate = value;
                OnPropertyChanged(nameof(SelectedDate));
                UpdateAvailableTimes();
            }
        }

        public string SelectedTime
        {
            get => _selectedTime;
            set
            {
                _selectedTime = value;
                OnPropertyChanged(nameof(SelectedTime));
            }
        }

        public List<string> AvailableTimes
        {
            get => _availableTimes;
            set
            {
                _availableTimes = value;
                OnPropertyChanged(nameof(AvailableTimes));
            }
        }

        public ICommand ConfirmBookingCommand { get; }
        public ICommand GoBackCommand { get; }

        public EnrollViewModel(int clientId, int tutorId, string tutorName)
        {
            _dbHelper = new DatabaseHelper();
            _clientId = clientId;
            _tutorId = tutorId;
            _tutorName = tutorName;
            _availableTimes = new List<string>();

            ConfirmBookingCommand = new RelayCommand(ExecuteConfirmBooking, CanExecuteConfirmBooking);
            GoBackCommand = new RelayCommand(ExecuteGoBack);
            UpdateAvailableTimes(); // Инициализация доступных времён
        }

        private void UpdateAvailableTimes()
        {
            // Здесь можно добавить логику получения доступных временных слотов от репетитора
            AvailableTimes = new List<string>
            {
                "09:00", "10:00", "11:00", "12:00", "13:00", "14:00", "15:00", "16:00", "17:00", "18:00"
            };
        }

        private void ExecuteConfirmBooking(object parameter)
        {
            if (SelectedDate.HasValue && !string.IsNullOrEmpty(SelectedTime))
            {
                try
                {
                    var dateTime = DateTime.Parse($"{SelectedDate.Value:yyyy-MM-dd} {SelectedTime}");
                    if (dateTime < DateTime.Now)
                    {
                        MessageBox.Show("Нельзя записаться на прошедшую дату или время.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    _dbHelper.AddBooking(_clientId, _tutorId, dateTime, "Ожидает");
                    MessageBox.Show("Запись успешно создана!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Возвращаемся назад после успешной записи
                    ExecuteGoBack(null);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при создании записи: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите дату и время.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private bool CanExecuteConfirmBooking(object parameter)
        {
            if (!SelectedDate.HasValue || string.IsNullOrEmpty(SelectedTime))
                return false;

            var dateTime = DateTime.Parse($"{SelectedDate.Value:yyyy-MM-dd} {SelectedTime}");
            return dateTime >= DateTime.Now;
        }

        private void ExecuteGoBack(object parameter)
        {
            var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            if (mainWindow == null)
            {
                System.Diagnostics.Debug.WriteLine("MainWindow not found.");
                MessageBox.Show("Ошибка: Главное окно не найдено.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var frame = mainWindow.FindName("MainFrame") as Frame;
            if (frame == null)
            {
                System.Diagnostics.Debug.WriteLine("MainFrame not found in MainWindow.");
                MessageBox.Show("Ошибка: Frame не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!frame.CanGoBack)
            {
                System.Diagnostics.Debug.WriteLine("Cannot go back: navigation history is empty.");
                MessageBox.Show("Ошибка: Нельзя вернуться назад, история навигации пуста.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            frame.GoBack();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}