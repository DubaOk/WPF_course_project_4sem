using Coach_search.Data;
using Coach_search.Models;
using Coach_search.MVVM.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Coach_search.ViewModels
{
    public class BookingDetailsViewModel : BaseViewModel
    {
        private readonly DatabaseHelper _dbHelper;
        private readonly ObservableCollection<Booking> _bookings;
        private readonly DateTime _selectedDate;
        private readonly int _tutorId;

        public BookingDetailsViewModel(System.Collections.Generic.List<Booking> bookings, DateTime selectedDate, int tutorId)
        {
            try
            {
                _dbHelper = new DatabaseHelper();
                _bookings = new ObservableCollection<Booking>(bookings ?? new System.Collections.Generic.List<Booking>());
                _selectedDate = selectedDate;
                _tutorId = tutorId;

                if (_bookings == null)
                {
                    System.Diagnostics.Debug.WriteLine("Warning: Bookings collection is null, initializing empty collection.");
                    _bookings = new ObservableCollection<Booking>();
                }

                System.Diagnostics.Debug.WriteLine($"BookingDetailsViewModel initialized with {_bookings.Count} bookings for tutorId {_tutorId} on {_selectedDate:dd/MM/yyyy}");
                foreach (var booking in _bookings)
                {
                    System.Diagnostics.Debug.WriteLine($"Booking: Id={booking.Id}, ClientId={booking.ClientId}, DateTime={booking.DateTime}, Status={booking.Status}");
                }

                OpenChatCommand = new RelayCommand<int>(OpenChat, CanOpenChat);
                AcceptBookingCommand = new RelayCommand<int>(AcceptBooking);
                RejectBookingCommand = new RelayCommand<int>(RejectBooking);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing BookingDetailsViewModel: {ex.Message}\nInner Exception: {ex.InnerException?.Message}\nStackTrace: {ex.StackTrace}");
                MessageBox.Show($"Ошибка при инициализации окна: {ex.Message}\nПодробности: {ex.InnerException?.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                _bookings = new ObservableCollection<Booking>();
            }
        }

        public ObservableCollection<Booking> Bookings
        {
            get => _bookings;
        }

        public DateTime SelectedDate
        {
            get => _selectedDate;
        }

        public ICommand OpenChatCommand { get; }
        public ICommand AcceptBookingCommand { get; }
        public ICommand RejectBookingCommand { get; }

        private bool CanOpenChat(int clientId)
        {
            if (clientId <= 0)
            {
                System.Diagnostics.Debug.WriteLine($"CanOpenChat failed: Invalid ClientId {clientId}");
                return false;
            }

            try
            {
                var user = _dbHelper.GetUserById(clientId);
                if (user == null)
                {
                    System.Diagnostics.Debug.WriteLine($"CanOpenChat failed: User with ClientId {clientId} not found in database.");
                    return false;
                }

                bool hasValidBooking = _bookings.Any(b =>
                    b.ClientId == clientId &&
                    b.TutorId == _tutorId &&
                    (b.Status == "Ожидает" || b.Status == "Подтверждено"));

                if (!hasValidBooking)
                {
                    System.Diagnostics.Debug.WriteLine($"CanOpenChat failed: No bookings with status 'Ожидает' or 'Подтверждено' found for ClientId {clientId} and TutorId {_tutorId}.");
                    return false;
                }

                System.Diagnostics.Debug.WriteLine($"CanOpenChat succeeded for ClientId {clientId} and TutorId {_tutorId}.");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CanOpenChat error: {ex.Message}\nInner Exception: {ex.InnerException?.Message}\nStackTrace: {ex.StackTrace}");
                return false;
            }
        }

        private void OpenChat(int clientId)
        {
            try
            {
                if (clientId <= 0)
                {
                    System.Diagnostics.Debug.WriteLine($"OpenChat aborted: Invalid ClientId {clientId}");
                    MessageBox.Show("Невозможно открыть чат: некорректный идентификатор клиента.", "Ошибка");
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"Opening chat between tutorId: {_tutorId} and clientId: {clientId}");
                var viewModel = new MessagesViewModel(_tutorId, clientId);
                var window = new MessagesWindow
                {
                    DataContext = viewModel
                };
                window.Show();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error opening chat: {ex.Message}\nInner Exception: {ex.InnerException?.Message}\nStackTrace: {ex.StackTrace}");
                MessageBox.Show($"Ошибка при открытии переписки: {ex.Message}\nПодробности: {ex.InnerException?.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AcceptBooking(int bookingId)
        {
            try
            {
                _dbHelper.UpdateBookingStatus(bookingId, "Подтверждено");
                var booking = _bookings.FirstOrDefault(b => b.Id == bookingId);
                if (booking != null)
                {
                    booking.Status = "Подтверждено"; // Это уведомит UI через INotifyPropertyChanged
                }
                Messenger.Default.Send(new NotificationMessage("BookingUpdated"));
                MessageBox.Show("Бронирование подтверждено!", "Успех");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error accepting booking: {ex.Message}\nStackTrace: {ex.StackTrace}");
                MessageBox.Show($"Ошибка при подтверждении бронирования: {ex.Message}", "Ошибка");
            }
        }

        private void RejectBooking(int bookingId)
        {
            try
            {
                _dbHelper.DeleteBooking(bookingId);
                var booking = _bookings.FirstOrDefault(b => b.Id == bookingId);
                if (booking != null)
                {
                    _bookings.Remove(booking); // Это уведомит UI через ObservableCollection
                    Messenger.Default.Send(new NotificationMessage("BookingDeleted"));

                    // Если бронирований больше нет, закрываем окно
                    if (!_bookings.Any())
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            var window = Application.Current.Windows.OfType<BookingDetailsWindow>().FirstOrDefault();
                            window?.Close();
                        });
                    }
                }
                MessageBox.Show("Бронирование отклонено!", "Успех");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error rejecting booking: {ex.Message}\nStackTrace: {ex.StackTrace}");
                MessageBox.Show($"Ошибка при отклонении бронирования: {ex.Message}", "Ошибка");
            }
        }
    }
}