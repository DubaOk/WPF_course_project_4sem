using Coach_search.Data;
using Coach_search.Models;
using Coach_search.MVVM.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging; // Добавляем для работы с Messenger
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

            // Подписка на событие удаления бронирования
            Messenger.Default.Register<NotificationMessage>(this, "BookingDeleted", message =>
            {
                LoadUsers(); // Перезагружаем список пользователей
            });
        }

        private void LoadUsers()
        {
            try
            {
                var allUsers = _dbHelper.GetAllUsers().Where(u => u.Id != _currentUserId && !u.IsBlocked).ToList();
                System.Diagnostics.Debug.WriteLine($"[LoadUsers] Current user ID: {_currentUserId}, UserType: {_userType}");
                System.Diagnostics.Debug.WriteLine($"[LoadUsers] Total unblocked users loaded: {allUsers.Count}");
                foreach (var user in allUsers)
                {
                    System.Diagnostics.Debug.WriteLine($"[LoadUsers] Found user: Id={user.Id}, Name={user.Name}, Type={user.UserType}");
                }

                if (_userType == UserType.Admin)
                {
                    System.Diagnostics.Debug.WriteLine("[LoadUsers] Loading users for Admin");
                    Users = new ObservableCollection<User>(allUsers);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[LoadUsers] Loading users for {_userType}");
                    var messageContacts = _dbHelper.GetMessageContacts(_currentUserId);
                    System.Diagnostics.Debug.WriteLine($"[LoadUsers] Message contacts: {string.Join(", ", messageContacts)}");

                    var filteredUsers = new List<User>();
                    foreach (var user in allUsers)
                    {
                        System.Diagnostics.Debug.WriteLine($"[LoadUsers] Checking user {user.Id} ({user.Name}, {user.UserType})");
                        
                        bool shouldCheck = (_userType == UserType.Client && user.UserType == UserType.Tutor) ||
                                         (_userType == UserType.Tutor && user.UserType == UserType.Client);
                                         
                        if (shouldCheck)
                        {
                            bool hasBooking = _dbHelper.HasBookingBetween(_currentUserId, user.Id);
                            System.Diagnostics.Debug.WriteLine($"[LoadUsers] User {user.Id} ({user.Name}): shouldCheck=true, hasBooking={hasBooking}");

                            if (hasBooking)
                            {
                                filteredUsers.Add(user);
                                System.Diagnostics.Debug.WriteLine($"[LoadUsers] Added user {user.Id} ({user.Name}) to filtered list");
                            }
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"[LoadUsers] Skipping user {user.Id} ({user.Name}): wrong user type combination");
                        }
                    }

                    System.Diagnostics.Debug.WriteLine($"[LoadUsers] Filtered users count: {filteredUsers.Count}");
                    foreach (var user in filteredUsers)
                    {
                        System.Diagnostics.Debug.WriteLine($"[LoadUsers] Final filtered user: Id={user.Id}, Name={user.Name}, Type={user.UserType}");
                    }

                    Users = new ObservableCollection<User>(filteredUsers);
                }

                System.Diagnostics.Debug.WriteLine($"[LoadUsers] Final users count in collection: {Users.Count}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[LoadUsers] Error: {ex.Message}\nStackTrace: {ex.StackTrace}");
                MessageBox.Show($"Ошибка при загрузке пользователей: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Users = new ObservableCollection<User>();
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