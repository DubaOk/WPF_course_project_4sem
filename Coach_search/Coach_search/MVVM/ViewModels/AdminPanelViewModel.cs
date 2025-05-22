using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Coach_search.Data;
using Coach_search.Models;
using Coach_search.MVVM.Models;
using Coach_search.MVVM.View;
using Coach_search.ViewModels;
using System.Linq;

namespace Coach_search.MVVM.ViewModels
{
    public class AdminPanelViewModel : BaseViewModel
    {
        private readonly DatabaseHelper _dbHelper;
        private readonly int _adminId;
        private ObservableCollection<User> _users;
        private User _selectedUser;
        private ObservableCollection<Review> _reviews;
        private Review _selectedReview;

        private ObservableCollection<User> _firstColumnUsers;
        public ObservableCollection<User> FirstColumnUsers
        {
            get => _firstColumnUsers;
            set
            {
                _firstColumnUsers = value;
                OnPropertyChanged(nameof(FirstColumnUsers));
            }
        }

        private ObservableCollection<User> _secondColumnUsers;
        public ObservableCollection<User> SecondColumnUsers
        {
            get => _secondColumnUsers;
            set
            {
                _secondColumnUsers = value;
                OnPropertyChanged(nameof(SecondColumnUsers));
            }
        }

        private ObservableCollection<User> _thirdColumnUsers;
        public ObservableCollection<User> ThirdColumnUsers
        {
            get => _thirdColumnUsers;
            set
            {
                _thirdColumnUsers = value;
                OnPropertyChanged(nameof(ThirdColumnUsers));
            }
        }

        public ObservableCollection<User> Users
        {
            get => _users;
            set
            {
                _users = value;
                OnPropertyChanged(nameof(Users));
                UpdateColumnLists();
            }
        }

        public User SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                OnPropertyChanged();
                System.Diagnostics.Debug.WriteLine($"SelectedUser changed to: {(_selectedUser?.Name ?? "null")}");
            }
        }

        public ObservableCollection<Review> Reviews
        {
            get => _reviews;
            set
            {
                _reviews = value;
                OnPropertyChanged();
            }
        }

        public Review SelectedReview
        {
            get => _selectedReview;
            set
            {
                _selectedReview = value;
                OnPropertyChanged();
                System.Diagnostics.Debug.WriteLine($"SelectedReview changed to: {(_selectedReview?.TutorName ?? "null")}");
            }
        }

        public ICommand NavigateBackCommand { get; }
        public ICommand BlockUserCommand { get; }
        public ICommand UpdateUserCommand { get; }
        public ICommand DeleteReviewCommand { get; }
        public ICommand UpdateReviewCommand { get; }

        public AdminPanelViewModel(int adminId)
        {
            _adminId = adminId;
            _dbHelper = new DatabaseHelper();
            NavigateBackCommand = new RelayCommand(NavigateBack);
            BlockUserCommand = new RelayCommand(_ => BlockUser(), _ => SelectedUser != null);
            UpdateUserCommand = new RelayCommand(_ => UpdateUser(), _ => SelectedUser != null);
            DeleteReviewCommand = new RelayCommand(_ => DeleteReview(), _ => SelectedReview != null);
            UpdateReviewCommand = new RelayCommand(_ => UpdateReview(), _ => SelectedReview != null);

            LoadData();
        }

        private void LoadData()
        {
            try
            {
                var userList = _dbHelper.GetAllUsers();
                Users = new ObservableCollection<User>(userList);
                Reviews = new ObservableCollection<Review>(_dbHelper.GetAllReviews());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка");
            }
        }

        private void NavigateBack(object parameter)
        {
            if (parameter is NavigationService navigationService)
            {
                System.Diagnostics.Debug.WriteLine("Navigating back to MainPage");
                navigationService.Navigate(new MainPage("admin", UserType.Admin, _adminId));
            }
        }

        private void BlockUser()
        {
            if (SelectedUser == null)
            {
                MessageBox.Show("Выберите пользователя.", "Ошибка");
                return;
            }

            if (SelectedUser.Id == _adminId)
            {
                MessageBox.Show("Нельзя заблокировать самого себя.", "Ошибка");
                return;
            }

            try
            {
                bool newBlockStatus = !SelectedUser.IsBlocked;
                _dbHelper.ToggleUserBlock(SelectedUser.Id, newBlockStatus);
                SelectedUser.IsBlocked = newBlockStatus;
                
                // Обновляем UI
                var userIndex = Users.IndexOf(SelectedUser);
                Users.RemoveAt(userIndex);
                Users.Insert(userIndex, SelectedUser);
                
                // Обновляем колонки
                UpdateColumnLists();
                
                MessageBox.Show($"Пользователь {(newBlockStatus ? "заблокирован" : "разблокирован")}!", "Успех");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка блокировки/разблокировки: {ex.Message}", "Ошибка");
            }
        }

        private void UpdateUser()
        {
            if (SelectedUser == null)
            {
                MessageBox.Show("Выберите пользователя.", "Ошибка");
                return;
            }

            if (string.IsNullOrWhiteSpace(SelectedUser.Name) || string.IsNullOrWhiteSpace(SelectedUser.Email))
            {
                MessageBox.Show("Имя и email не могут быть пустыми.", "Ошибка");
                return;
            }

            if (SelectedUser.Id == _adminId && SelectedUser.UserType != Coach_search.Models.UserType.Admin)
            {
                MessageBox.Show("Нельзя изменить свой тип пользователя.", "Ошибка");
                return;
            }

            try
            {
                _dbHelper.UpdateUser(SelectedUser);
                MessageBox.Show("Данные пользователя обновлены!", "Успех");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления данных: {ex.Message}", "Ошибка");
            }
        }

        private void DeleteReview()
        {
            if (SelectedReview == null)
            {
                MessageBox.Show("Выберите отзыв.", "Ошибка");
                return;
            }

            var result = MessageBox.Show("Вы уверены, что хотите удалить этот отзыв?", "Подтверждение", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _dbHelper.DeleteReview(SelectedReview.Id);
                    Reviews.Remove(SelectedReview);
                    MessageBox.Show("Отзыв удалён!", "Успех");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления отзыва: {ex.Message}", "Ошибка");
                }
            }
        }

        private void UpdateReview()
        {
            if (SelectedReview == null)
            {
                MessageBox.Show("Выберите отзыв.", "Ошибка");
                return;
            }

            if (string.IsNullOrWhiteSpace(SelectedReview.Text) || SelectedReview.Rating < 1 || SelectedReview.Rating > 5)
            {
                MessageBox.Show("Текст отзыва не может быть пустым, рейтинг должен быть от 1 до 5.", "Ошибка");
                return;
            }

            try
            {
                _dbHelper.UpdateReview(SelectedReview);
                MessageBox.Show("Отзыв обновлён!", "Успех");
                LoadData(); // Перезагрузка данных для обновления рейтинга
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления отзыва: {ex.Message}", "Ошибка");
            }
        }

        private void UpdateColumnLists()
        {
            if (Users == null) return;

            var usersList = Users.ToList();
            int totalUsers = usersList.Count;
            int itemsPerColumn = (int)Math.Ceiling(totalUsers / 3.0);

            FirstColumnUsers = new ObservableCollection<User>(usersList.Take(itemsPerColumn));
            SecondColumnUsers = new ObservableCollection<User>(usersList.Skip(itemsPerColumn).Take(itemsPerColumn));
            ThirdColumnUsers = new ObservableCollection<User>(usersList.Skip(itemsPerColumn * 2).Take(itemsPerColumn));
        }
    }
}