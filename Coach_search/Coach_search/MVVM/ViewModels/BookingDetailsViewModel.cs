using Coach_search.Data;
using Coach_search.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
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
            _dbHelper = new DatabaseHelper();
            _bookings = new ObservableCollection<Booking>(bookings);
            _selectedDate = selectedDate;
            _tutorId = tutorId;
            //CloseCommand = new RelayCommand(Close);
            OpenChatCommand = new RelayCommand<int>(OpenChat);
            AcceptBookingCommand = new RelayCommand<int>(AcceptBooking);
            RejectBookingCommand = new RelayCommand<int>(RejectBooking);
        }

        public ObservableCollection<Booking> Bookings
        {
            get => _bookings;
        }

        public DateTime SelectedDate
        {
            get => _selectedDate;
        }

        public ICommand CloseCommand { get; }
        public ICommand OpenChatCommand { get; }
        public ICommand AcceptBookingCommand { get; }
        public ICommand RejectBookingCommand { get; }

        private void Close()
        {
            Application.Current.Windows.OfType<MahApps.Metro.Controls.MetroWindow>()
                .FirstOrDefault(w => w.DataContext == this)?.Close();
        }

        private void OpenChat(int clientId)
        {
            // Заглушка для переписки
            MessageBox.Show($"Функционал переписки пока не реализован. ClientId: {clientId}", "Переписка");
        }

        private void AcceptBooking(int bookingId)
        {
            try
            {
                _dbHelper.UpdateBookingStatus(bookingId, "Подтверждено");
                var booking = _bookings.FirstOrDefault(b => b.Id == bookingId);
                if (booking != null)
                {
                    booking.Status = "Подтверждено";
                    OnPropertyChanged(nameof(Bookings));
                }
                MessageBox.Show("Бронирование подтверждено!", "Успех");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при подтверждении бронирования: {ex.Message}", "Ошибка");
            }
        }

        private void RejectBooking(int bookingId)
        {
            try
            {
                _dbHelper.UpdateBookingStatus(bookingId, "Отклонено");
                var booking = _bookings.FirstOrDefault(b => b.Id == bookingId);
                if (booking != null)
                {
                    booking.Status = "Отклонено";
                    OnPropertyChanged(nameof(Bookings));
                }
                MessageBox.Show("Бронирование отклонено!", "Успех");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отклонении бронирования: {ex.Message}", "Ошибка");
            }
        }
    }
}