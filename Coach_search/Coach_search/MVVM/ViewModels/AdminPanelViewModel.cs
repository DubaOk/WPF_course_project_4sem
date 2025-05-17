using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Coach_search.Data;
using Coach_search.Models;
using Coach_search.MVVM.Models;
using Coach_search.ViewModels;

namespace Coach_search.MVVM.ViewModels
{
    public class AdminPanelViewModel : BaseViewModel
    {
        private readonly DatabaseHelper _dbHelper;
        private readonly int _adminId;
        private ObservableCollection<Coach_search.Models.User> _users;
        private Coach_search.Models.User _selectedUser;
        private ObservableCollection<Review> _reviews;
        private Review _selectedReview;

        public ObservableCollection<Coach_search.Models.User> Users
        {
            get => _users;
            set
            {
                _users = value;
                OnPropertyChanged();
            }
        }

        public Coach_search.Models.User SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                OnPropertyChanged();
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
            }
        }

        public ICommand BlockUserCommand { get; }
        public ICommand UpdateUserCommand { get; }
        public ICommand DeleteReviewCommand { get; }

        public AdminPanelViewModel(int adminId)
        {
            _adminId = adminId;
            _dbHelper = new DatabaseHelper();
            BlockUserCommand = new RelayCommand(_ => BlockUser(), _ => SelectedUser != null);
            UpdateUserCommand = new RelayCommand(_ => UpdateUser(), _ => SelectedUser != null);
            DeleteReviewCommand = new RelayCommand(_ => DeleteReview(), _ => SelectedReview != null);

            LoadData();
        }

        private void LoadData()
        {
            try
            {
                var userList = _dbHelper.GetAllUsers();
                Users = new ObservableCollection<Coach_search.Models.User>(userList);
                Reviews = new ObservableCollection<Review>(_dbHelper.GetAllReviews());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка");
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
                OnPropertyChanged(nameof(Users));
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
    }
}