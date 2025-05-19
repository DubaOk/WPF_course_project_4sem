using Coach_search.Data;
using Coach_search.Models;
using Coach_search.MVVM.View;
using Coach_search.MVVM.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Microsoft.Data.SqlClient;
using Microsoft.Win32;
using System;

namespace Coach_search.ViewModels
{
    public class ClientProfileViewModel : BaseViewModel
    {
        private readonly DatabaseHelper _dbHelper = new DatabaseHelper();
        private Client _currentClient;
        public Client CurrentClient
        {
            get => _currentClient;
            set
            {
                _currentClient = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Booking> Bookings { get; set; } = new ObservableCollection<Booking>();
        public ObservableCollection<Review> Reviews { get; set; } = new ObservableCollection<Review>();
        public ICommand SaveProfileCommand { get; }
        public ICommand CancelBookingCommand { get; }
        public ICommand GoBackCommand { get; }
        public ICommand AddReviewCommand { get; }
        public ICommand AddReviewForBookingCommand { get; }
        public ICommand UploadAvatarCommand { get; }

        public ClientProfileViewModel(int userId)
        {
            GoBackCommand = new RelayCommand(_ => GoBack());
            SaveProfileCommand = new RelayCommand(_ => SaveProfile());
            CancelBookingCommand = new RelayCommand(CancelBooking);
            AddReviewCommand = new RelayCommand(_ => AddReview());
            AddReviewForBookingCommand = new RelayCommand(AddReviewForBooking);
            UploadAvatarCommand = new RelayCommand(UploadAvatar);

            if (TestDatabaseConnection())
            {
                LoadData(userId);
            }
            else
            {
                GoBack();
            }
        }

        private bool TestDatabaseConnection()
        {
            try
            {
                using (var connection = new SqlConnection(_dbHelper.ConnectionString))
                {
                    connection.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось подключиться к базе данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private void LoadData(int userId)
        {
            try
            {
                CurrentClient = _dbHelper.GetClient(userId) ?? new Client { UserId = userId, AvatarPath = "pack://application:,,,/MVVM/View/images/icons/defaultavatar.png" };
                if (CurrentClient == null)
                {
                    MessageBox.Show("Клиент не найден. Возможно, пользователь не существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    GoBack();
                    return;
                }

                var bookings = _dbHelper.GetClientBookings(userId) ?? new List<Booking>();
                Bookings.Clear();
                foreach (var booking in bookings)
                {
                    booking.TutorName = _dbHelper.GetTutorByUserId(booking.TutorId)?.Name ?? "Неизвестный репетитор";
                    booking.CanCancel = booking.Status == "Ожидает";
                    booking.CanLeaveReview = booking.Status == "Подтверждено" && !_dbHelper.HasReviewForBooking(booking.Id);
                    Bookings.Add(booking);
                    System.Diagnostics.Debug.WriteLine($"Booking ID: {booking.Id}, Status: {booking.Status}, CanCancel: {booking.CanCancel}, CanLeaveReview: {booking.CanLeaveReview}");
                }

                var reviews = _dbHelper.GetClientReviews(userId) ?? new List<Review>();
                Reviews.Clear();
                foreach (var review in reviews)
                {
                    Reviews.Add(review);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                GoBack();
            }
        }

        private void SaveProfile()
        {
            if (CurrentClient == null)
            {
                MessageBox.Show("Данные клиента не загружены.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(CurrentClient.Name))
            {
                MessageBox.Show("Имя не может быть пустым.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                _dbHelper.UpdateClient(CurrentClient);
                MessageBox.Show("Данные успешно сохранены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения профиля: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelBooking(object parameter)
        {
            if (parameter is Booking booking)
            {
                var result = MessageBox.Show("Вы уверены, что хотите отменить запись?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _dbHelper.UpdateBookingStatus(booking.Id, "Отменено");
                        LoadData(CurrentClient.UserId);
                        MessageBox.Show("Запись успешно отменена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка отмены записи: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Не удалось определить запись для отмены.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddReview()
        {
            try
            {
                var dialog = new ReviewDialog();
                if (dialog.ShowDialog() == true)
                {
                    var review = new Review
                    {
                        ClientId = CurrentClient.UserId,
                        TutorId = 0, // TODO: Добавить выбор репетитора
                        Text = dialog.ReviewText ?? string.Empty,
                        Rating = dialog.Rating,
                        CreatedAt = DateTime.Now
                    };

                    if (string.IsNullOrWhiteSpace(review.Text))
                    {
                        MessageBox.Show("Текст отзыва не может быть пустым.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (review.Rating < 1 || review.Rating > 5)
                    {
                        MessageBox.Show("Рейтинг должен быть от 1 до 5.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    _dbHelper.AddReview(review);
                    LoadData(CurrentClient.UserId);
                    MessageBox.Show("Отзыв успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления отзыва: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GoBack()
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null && mainWindow.Frame != null)
            {
                mainWindow.Frame.Navigate(new MainPage(
                    CurrentClient?.Name ?? "Гость",
                    UserType.Client,
                    CurrentClient?.UserId ?? 0
                ));
            }
            else
            {
                var page = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive) as MainWindow;
                if (page != null && page.Frame != null)
                {
                    page.Frame.Navigate(new MainPage(
                        CurrentClient?.Name ?? "Гость",
                        UserType.Client,
                        CurrentClient?.UserId ?? 0
                    ));
                }
                else
                {
                    MessageBox.Show("Не удалось вернуться на главную страницу. Проверь настройки навигации.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void AddReviewForBooking(object parameter)
        {
            if (parameter is Booking booking)
            {
                try
                {
                    var dialog = new ReviewDialog(booking.TutorName);
                    if (dialog.ShowDialog() == true)
                    {
                        var review = new Review
                        {
                            ClientId = CurrentClient.UserId,
                            TutorId = booking.TutorId,
                            Text = dialog.ReviewText ?? string.Empty,
                            Rating = dialog.Rating,
                            CreatedAt = DateTime.Now,
                            BookingId = booking.Id
                        };

                        if (string.IsNullOrWhiteSpace(review.Text))
                        {
                            MessageBox.Show("Текст отзыва не может быть пустым.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        if (review.Rating < 1 || review.Rating > 5)
                        {
                            MessageBox.Show("Рейтинг должен быть от 1 до 5.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        _dbHelper.AddReview(review);
                        LoadData(CurrentClient.UserId);
                        MessageBox.Show("Отзыв успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка добавления отзыва: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void UploadAvatar()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpg)|*.png;*.jpg|All files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                CurrentClient.AvatarPath = openFileDialog.FileName; // Сохраняем временный путь
                _dbHelper.UpdateClientAvatar(CurrentClient.UserId, CurrentClient.AvatarPath);
                MessageBox.Show("Аватар обновлен!", "Успех");
            }
        }
    }
}