using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Coach_search.Data;
using Coach_search.Models;
using Coach_search.MVVM.View;

namespace Coach_search.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly DatabaseHelper _dbHelper;
        private ObservableCollection<Tutor> _tutors;
        private ObservableCollection<Tutor> _allTutors;
        private Tutor _selectedTutor;
        private readonly string _userName;
        private readonly UserType _userType;
        private readonly int _userId;
        private string _searchSubject;
        private string _searchRating;
        private string _searchPriceRange;
        private string _avatarPath;
        private ObservableCollection<string> _subjects;
        private ObservableCollection<string> _ratings;
        private ObservableCollection<string> _priceRanges;

        public ObservableCollection<Tutor> Tutors
        {
            get => _tutors;
            set
            {
                _tutors = value;
                OnPropertyChanged();
            }
        }

        public Tutor SelectedTutor
        {
            get => _selectedTutor;
            set
            {
                _selectedTutor = value;
                OnPropertyChanged();
            }
        }

        public string UserName => _userName;
        public UserType UserType => _userType;
        public int UserId => _userId;

        public string SearchSubject
        {
            get => _searchSubject;
            set
            {
                _searchSubject = value;
                OnPropertyChanged();
                FilterTutors();
            }
        }

        public string SearchRating
        {
            get => _searchRating;
            set
            {
                _searchRating = value;
                OnPropertyChanged();
                FilterTutors();
            }
        }

        public string SearchPriceRange
        {
            get => _searchPriceRange;
            set
            {
                _searchPriceRange = value;
                OnPropertyChanged();
                FilterTutors();
            }
        }

        public string AvatarPath
        {
            get => _avatarPath;
            set
            {
                _avatarPath = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> Subjects
        {
            get => _subjects;
            set
            {
                _subjects = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> Ratings
        {
            get => _ratings;
            set
            {
                _ratings = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> PriceRanges
        {
            get => _priceRanges;
            set
            {
                _priceRanges = value;
                OnPropertyChanged();
            }
        }

        public bool IsAdmin => UserType == UserType.Admin;
        public bool IsClient => UserType == UserType.Client;
        public bool IsTutor => UserType == UserType.Tutor;

        public ICommand ShowTutorDetailsCommand { get; }
        public ICommand ShowTutorProfileCommand { get; }
        public ICommand NavigateCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand EnrollCommand { get; }
        public ICommand ClientProfilePage { get; }

        public MainViewModel(string userName, UserType userType, int userId)
        {
            _dbHelper = new DatabaseHelper();
            _userName = userName;
            _userType = userType;
            _userId = userId;
            

            _allTutors = new ObservableCollection<Tutor>(_dbHelper.GetTutors());
            Tutors = new ObservableCollection<Tutor>(_allTutors);

            if (_userType == UserType.Tutor)
            {
                var tutor = _dbHelper.GetTutorByUserId(_userId);
                if (tutor != null)
                {
                    AvatarPath = tutor.AvatarPath;
                }
            }

            Subjects = new ObservableCollection<string>
            {
                "Все предметы",
                "Математика",
                "Физика",
                "Химия",
                "Английский язык",
                "Русский язык",
                "История",
                "Биология"
            };

            Ratings = new ObservableCollection<string>
            {
                "Любой рейтинг",
                "5",
                "4",
                "3",
                "2",
                "1"
            };

            PriceRanges = new ObservableCollection<string>
            {
                "Любая цена",
                "0-500",
                "500-1000",
                "1000-2000",
                "2000+"
            };

            ShowTutorDetailsCommand = new RelayCommand(ShowTutorDetails, _ => SelectedTutor != null);
            ShowTutorProfileCommand = new RelayCommand(ShowTutorProfile, _ => UserType == UserType.Tutor);
            NavigateCommand = new RelayCommand(Navigate);
            SearchCommand = new RelayCommand(_ => FilterTutors());
            LogoutCommand = new RelayCommand(Logout);
            EnrollCommand = new RelayCommand(Enroll);
            ClientProfilePage = new RelayCommand(Navigate);

            System.Diagnostics.Debug.WriteLine($"MainViewModel initialized: userId={_userId}, userType={_userType}, IsClient={IsClient}, IsAdmin={IsAdmin}");
        }

        private void ShowTutorDetails(object parameter)
        {
            if (SelectedTutor != null)
            {
                System.Diagnostics.Debug.WriteLine($"Opening TutorDetailsWindow for tutor: {SelectedTutor.Name}");
                var viewModel = new TutorDetailsViewModel(SelectedTutor);
                var window = new TutorDetailsWindow(viewModel);
                window.ShowDialog();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("ShowTutorDetails failed: no tutor selected");
                MessageBox.Show("Ошибка: Репетитор не выбран.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowTutorProfile(object parameter)
        {
            if (parameter is Frame frame)
            {
                System.Diagnostics.Debug.WriteLine($"Navigating to TutorProfilePage for userId: {_userId}");
                frame.Navigate(new TutorProfilePage(UserId));
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("ShowTutorProfile failed: invalid frame");
            }
        }
        private void Navigate(object parameter)
        {
            System.Diagnostics.Debug.WriteLine($"Navigate called with parameter: {parameter ?? "null"}");

            if (!(parameter is string pageName) || string.IsNullOrEmpty(pageName))
            {
                System.Diagnostics.Debug.WriteLine("Navigate failed: parameter is null, empty, or not a string.");
                MessageBox.Show("Ошибка: Неверный или пустой параметр навигации.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

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

            System.Diagnostics.Debug.WriteLine($"Navigating to page: {pageName}, userType: {_userType}, userId: {_userId}");

            switch (pageName)
            {
                case "MainPage":
                    frame.Navigate(new MainPage(UserName, UserType, UserId));
                    break;
                case "SearchPage":
                    System.Diagnostics.Debug.WriteLine("SearchPage navigation not implemented.");
                    MessageBox.Show("Страница поиска пока не реализована.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case "SchedulePage":
                    if (UserType == UserType.Tutor)
                    {
                        frame.Navigate(new SchedulePage(UserId, frame));
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("SchedulePage access denied: user is not a tutor.");
                        MessageBox.Show("Расписание доступно только для репетиторов.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    break;
                case "MessagesPage":
                    System.Diagnostics.Debug.WriteLine("MessagesPage navigation not implemented.");
                    MessageBox.Show("Страница сообщений пока не реализована.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case "SettingsPage":
                    if (UserType == UserType.Tutor)
                    {
                        frame.Navigate(new TutorProfilePage(UserId));
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("SettingsPage access denied: user is not a tutor.");
                        MessageBox.Show("Настройки доступны только для репетиторов.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    break;
                case "ClientProfilePage":
                    if (UserType == UserType.Client)
                    {
                        frame.Navigate(new ClientProfilePage(UserId));
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("ClientProfilePage access denied: user is not a client.");
                        MessageBox.Show("Личный кабинет доступен только для клиентов.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    break;
                case "AdminPanelPage":
                    if (UserType == UserType.Admin)
                    {
                        frame.Navigate(new AdminPanelPage(UserId));
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("AdminPanelPage access denied: user is not an admin.");
                        MessageBox.Show("Админ-панель доступна только для администраторов.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    break;
                default:
                    System.Diagnostics.Debug.WriteLine($"Unknown pageName: {pageName}");
                    MessageBox.Show($"Ошибка: Неизвестная страница '{pageName}'.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
            }
        }

        private void Logout(object parameter)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Logout initiated.");
                var loginWindow = new LoginWindow();
                Window currentWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                if (currentWindow != null)
                {
                    Application.Current.MainWindow = loginWindow;
                    if (currentWindow.FindName("MainFrame") is Frame mainFrame)
                    {
                        mainFrame.Content = null;
                    }
                    currentWindow.Close();
                }
                loginWindow.Show();
                System.Diagnostics.Debug.WriteLine("Logout completed.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error during logout: {ex.Message}\nStackTrace: {ex.StackTrace}");
                MessageBox.Show($"Ошибка при выходе: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Enroll(object parameter)
        {
            if (parameter is Tutor tutor)
            {
                System.Diagnostics.Debug.WriteLine($"Enroll called for tutor: {tutor.Name}");

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

                System.Diagnostics.Debug.WriteLine($"Navigating to EnrollPage for tutor: {tutor.Name}, userId: {_userId}");
                frame.Navigate(new EnrollPage(_userId, tutor.Id, tutor.Name));
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Enroll failed: invalid tutor parameter");
                MessageBox.Show("Ошибка: Репетитор не выбран.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FilterTutors()
        {
            System.Diagnostics.Debug.WriteLine($"FilterTutors called: Subject={SearchSubject}, Rating={SearchRating}, PriceRange={SearchPriceRange}");
            var filteredTutors = _allTutors.AsEnumerable();

            if (!string.IsNullOrEmpty(SearchSubject) && SearchSubject != "Все предметы")
            {
                filteredTutors = filteredTutors.Where(t => t.Subject != null && t.Subject.Equals(SearchSubject, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(SearchRating) && SearchRating != "Любой рейтинг" && double.TryParse(SearchRating, out double rating))
            {
                filteredTutors = filteredTutors.Where(t => t.Rating >= rating);
            }

            if (!string.IsNullOrEmpty(SearchPriceRange) && SearchPriceRange != "Любая цена")
            {
                switch (SearchPriceRange)
                {
                    case "0-500":
                        filteredTutors = filteredTutors.Where(t => t.PricePerHour >= 0 && t.PricePerHour <= 500);
                        break;
                    case "500-1000":
                        filteredTutors = filteredTutors.Where(t => t.PricePerHour > 500 && t.PricePerHour <= 1000);
                        break;
                    case "1000-2000":
                        filteredTutors = filteredTutors.Where(t => t.PricePerHour > 1000 && t.PricePerHour <= 2000);
                        break;
                    case "2000+":
                        filteredTutors = filteredTutors.Where(t => t.PricePerHour > 2000);
                        break;
                }
            }

            Tutors = new ObservableCollection<Tutor>(filteredTutors);
            System.Diagnostics.Debug.WriteLine($"FilterTutors completed: {Tutors.Count} tutors found.");
        }
    }
}