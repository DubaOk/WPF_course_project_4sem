using Coach_search.Data;
using Coach_search.Models;
using Coach_search.MVVM.View;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;

namespace Coach_search.ViewModels
{
    public class ScheduleViewModel : BaseViewModel
    {
        private readonly DatabaseHelper _dbHelper;
        private readonly int _tutorId;
        private readonly Frame _navigationFrame;
        private DateTime _currentDate = DateTime.Now;
        private ObservableCollection<CalendarDay> _calendarDays;

        public ObservableCollection<CalendarDay> CalendarDays
        {
            get => _calendarDays;
            set
            {
                _calendarDays = value;
                OnPropertyChanged(nameof(CalendarDays));
            }
        }

        public string CurrentMonth => _currentDate.ToString("MMMM yyyy", new System.Globalization.CultureInfo("ru-RU"));

        public ICommand ShowBookingsCommand { get; }

        public ScheduleViewModel(int userId, Frame navigationFrame)
        {
            _navigationFrame = navigationFrame;
            _dbHelper = new DatabaseHelper();
            if (!_dbHelper.TestConnection())
            {
                System.Windows.MessageBox.Show("Не удалось подключиться к базе данных.", "Ошибка");
                return;
            }
            var tutor = _dbHelper.GetTutorByUserId(userId);
            if (tutor == null)
            {
                System.Windows.MessageBox.Show($"Репетитор с UserId = {userId} не найден.", "Ошибка");
                return;
            }
            _tutorId = tutor.Id;
            System.Diagnostics.Debug.WriteLine($"Tutor found: UserId={userId}, TutorId={_tutorId}");
            ShowBookingsCommand = new RelayCommand<DateTime>(ShowBookings);
            GenerateCalendar();
        }

        private void GenerateCalendar()
        {
            var days = new ObservableCollection<CalendarDay>();
            DateTime firstOfMonth = new DateTime(_currentDate.Year, _currentDate.Month, 1);
            DateTime firstDayOfCalendar = firstOfMonth.AddDays(-(int)firstOfMonth.DayOfWeek + 1);
            DateTime lastDayOfCalendar = firstOfMonth.AddMonths(1).AddDays(-1).AddDays(6 - (int)firstOfMonth.AddMonths(1).AddDays(-1).DayOfWeek);

            var bookings = _dbHelper.GetBookingsForTutor(_tutorId, _currentDate.Month, _currentDate.Year);
            System.Diagnostics.Debug.WriteLine($"Bookings count for tutor {_tutorId}, month {_currentDate.Month}, year {_currentDate.Year}: {bookings.Count}");

            int row = 0, col = 0;
            for (DateTime date = firstDayOfCalendar; date <= lastDayOfCalendar; date = date.AddDays(1))
            {
                var day = new CalendarDay
                {
                    Day = date.Day,
                    Date = date,
                    IsCurrentMonth = date.Month == _currentDate.Month,
                    Row = row,
                    Column = col,
                    Bookings = new ObservableCollection<Booking>(bookings.Where(b => b.DateTime.Date == date.Date))
                };
                System.Diagnostics.Debug.WriteLine($"Date {date.ToShortDateString()}: {day.Bookings.Count} bookings");
                days.Add(day);

                col++;
                if (col > 6)
                {
                    col = 0;
                    row++;
                }
            }

            CalendarDays = days;
        }

        private void ShowBookings(DateTime selectedDate)
        {
            var bookings = _dbHelper.GetBookingsForTutor(_tutorId, selectedDate.Month, selectedDate.Year)
                .Where(b => b.DateTime.Date == selectedDate.Date)
                .ToList();
            var bookingDetailsViewModel = new BookingDetailsViewModel(bookings, selectedDate, _tutorId);
            var bookingDetailsWindow = new BookingDetailsWindow(bookingDetailsViewModel);
            bookingDetailsWindow.ShowDialog();
        }

        public ICommand GoBackCommand => new RelayCommand(_ =>
        {
            if (_navigationFrame.CanGoBack)
                _navigationFrame.GoBack();
        });

        public ICommand PreviousMonthCommand => new RelayCommand(_ =>
        {
            _currentDate = _currentDate.AddMonths(-1);
            OnPropertyChanged(nameof(CurrentMonth));
            GenerateCalendar();
        });

        public ICommand NextMonthCommand => new RelayCommand(_ =>
        {
            _currentDate = _currentDate.AddMonths(1);
            OnPropertyChanged(nameof(CurrentMonth));
            GenerateCalendar();
        });
    }

    public class CalendarDay : BaseViewModel
    {
        private int _day;
        private DateTime _date;
        private bool _isCurrentMonth;
        private int _row;
        private int _column;
        private ObservableCollection<Booking> _bookings;

        public int Day
        {
            get => _day;
            set
            {
                _day = value;
                OnPropertyChanged(nameof(Day));
            }
        }

        public DateTime Date
        {
            get => _date;
            set
            {
                _date = value;
                OnPropertyChanged(nameof(Date));
            }
        }

        public bool IsCurrentMonth
        {
            get => _isCurrentMonth;
            set
            {
                _isCurrentMonth = value;
                OnPropertyChanged(nameof(IsCurrentMonth));
            }
        }

        public int Row
        {
            get => _row;
            set
            {
                _row = value;
                OnPropertyChanged(nameof(Row));
            }
        }

        public int Column
        {
            get => _column;
            set
            {
                _column = value;
                OnPropertyChanged(nameof(Column));
            }
        }

        public ObservableCollection<Booking> Bookings
        {
            get => _bookings;
            set
            {
                _bookings = value;
                System.Diagnostics.Debug.WriteLine($"Bookings set for {Date.ToShortDateString()}: Count = {_bookings?.Count ?? 0}");
                OnPropertyChanged(nameof(Bookings));
                OnPropertyChanged(nameof(Bookings.Count)); // Явно уведомляем о Count
            }
        }
    }
}