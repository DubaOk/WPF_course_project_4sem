using Coach_search.Data;
using Coach_search.Models;
using Coach_search.MVVM.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Coach_search.ViewModels
{
    public class SelectConversationViewModel : BaseViewModel
    {
        private readonly DatabaseHelper _dbHelper;
        private readonly int _currentUserId;
        private readonly UserType _userType;
        private ObservableCollection<User> _users;
        private User _selectedUser;

        public ObservableCollection<User> Users
        {
            get => _users;
            set
            {
                _users = value;
                OnPropertyChanged();
            }
        }

        public User SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand SelectUserCommand { get; }

        public SelectConversationViewModel(int currentUserId, UserType userType)
        {
            _dbHelper = new DatabaseHelper();
            _currentUserId = currentUserId;
            _userType = userType;
            SelectUserCommand = new RelayCommand(SelectUser, () => SelectedUser != null);
            LoadUsers();
        }

        private void LoadUsers()
        {
            var allUsers = _dbHelper.GetAllUsers().Where(u => u.Id != _currentUserId).ToList();
            System.Diagnostics.Debug.WriteLine($"Total users loaded: {allUsers.Count}");

            var messageContacts = _dbHelper.GetMessageContacts(_currentUserId);
            System.Diagnostics.Debug.WriteLine($"Message contacts: {string.Join(", ", messageContacts)}");

            var bookingContacts = new List<int>();
            if (_userType == UserType.Client)
            {
                var bookings = _dbHelper.GetClientBookings(_currentUserId);
                foreach (var booking in bookings)
                {
                    var tutor = _dbHelper.GetTutorByUserId(booking.TutorId);
                    if (tutor != null)
                    {
                        bookingContacts.Add(tutor.UserId);
                    }
                }
                System.Diagnostics.Debug.WriteLine($"Client bookings (UserIds): {string.Join(", ", bookingContacts)}");
            }
            else if (_userType == UserType.Tutor)
            {
                var bookings = _dbHelper.GetBookingsForTutor(_currentUserId, DateTime.Now.Month, DateTime.Now.Year);
                bookingContacts = bookings.Select(b => b.ClientId).Distinct().ToList();
                System.Diagnostics.Debug.WriteLine($"Tutor bookings (UserIds): {string.Join(", ", bookingContacts)}");
            }

            var contactUserIds = messageContacts.Union(bookingContacts).Distinct().ToList();
            System.Diagnostics.Debug.WriteLine($"Combined contacts: {string.Join(", ", contactUserIds)}");

            if (_userType == UserType.Tutor)
            {
                Users = new ObservableCollection<User>(allUsers
                    .Where(u => u.UserType == UserType.Client && (contactUserIds.Contains(u.Id) || _dbHelper.HasBookingBetween(_currentUserId, u.Id))));
            }
            else if (_userType == UserType.Client)
            {
                Users = new ObservableCollection<User>(allUsers
                    .Where(u => u.UserType == UserType.Tutor && (contactUserIds.Contains(u.Id) || _dbHelper.HasBookingBetween(_currentUserId, u.Id))));
            }
            else
            {
                Users = new ObservableCollection<User>(allUsers
                    .Where(u => contactUserIds.Contains(u.Id) || _dbHelper.HasBookingBetween(_currentUserId, u.Id)));
            }

            System.Diagnostics.Debug.WriteLine($"Filtered users count: {Users.Count}");
            foreach (var user in Users.Where(u => u.UserType == UserType.Tutor))
            {
                var tutor = _dbHelper.GetTutorByUserId(user.Id);
                if (tutor != null)
                {
                    user.AvatarPath = tutor.AvatarPath;
                    System.Diagnostics.Debug.WriteLine($"Set avatar for user {user.Id}: {tutor.AvatarPath}");
                }
            }
        }

        private void SelectUser()
        {
            if (SelectedUser != null)
            {
                var window = new MessagesWindow
                {
                    DataContext = new MessagesViewModel(_currentUserId, SelectedUser.Id)
                };
                window.Show();
                Window.GetWindow(Application.Current.MainWindow)?.Close();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите собеседника.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}