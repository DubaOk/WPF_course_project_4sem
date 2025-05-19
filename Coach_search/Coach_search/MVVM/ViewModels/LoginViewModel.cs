using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Coach_search.Data;
using Coach_search.Models;
using Coach_search.MVVM.View;
using BCrypt.Net;

namespace Coach_search.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly DatabaseHelper _dbHelper;
        private string _loginEmail;
        private string _loginPassword;
        private string _registerName;
        private string _registerEmail;
        private string _registerPassword;
        private string _selectedUserType;

        public string LoginEmail
        {
            get => _loginEmail;
            set
            {
                _loginEmail = value;
                OnPropertyChanged();
            }
        }

        public string LoginPassword
        {
            get => _loginPassword;
            set
            {
                _loginPassword = value;
                OnPropertyChanged();
            }
        }

        public string RegisterName
        {
            get => _registerName;
            set
            {
                _registerName = value;
                OnPropertyChanged();
            }
        }

        public string RegisterEmail
        {
            get => _registerEmail;
            set
            {
                _registerEmail = value;
                OnPropertyChanged();
            }
        }

        public string RegisterPassword
        {
            get => _registerPassword;
            set
            {
                _registerPassword = value;
                OnPropertyChanged();
            }
        }

        public string SelectedUserType
        {
            get => _selectedUserType;
            set
            {
                _selectedUserType = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }

        public LoginViewModel()
        {
            _dbHelper = new DatabaseHelper();
            LoginCommand = new RelayCommand(OnLogin);
            RegisterCommand = new RelayCommand(OnRegister);
            SelectedUserType = "Клиент";
        }

        private void OnLogin(object parameter)
        {
            // Валидация полей
            if (string.IsNullOrWhiteSpace(LoginEmail))
            {
                MessageBox.Show("Пожалуйста, введите email.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!IsValidEmail(LoginEmail))
            {
                MessageBox.Show("Пожалуйста, введите корректный email (пример: user@example.com).", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(LoginPassword))
            {
                MessageBox.Show("Пожалуйста, введите пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (LoginPassword.Length < 6)
            {
                MessageBox.Show("Пароль должен содержать минимум 6 символов.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var connection = new System.Data.SqlClient.SqlConnection(_dbHelper.ConnectionString))
                {
                    connection.Open();
                    string query = "SELECT Id, Name, UserType, Password, IsBlocked FROM Users WHERE Email = @Email";
                    using (var command = new System.Data.SqlClient.SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Email", LoginEmail);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int userId = reader.GetInt32(0);
                                string userName = reader.GetString(1);
                                string userTypeString = reader.GetString(2);
                                string storedPasswordHash = reader.GetString(3);
                                bool isBlocked = reader.GetBoolean(4);

                                if (isBlocked)
                                {
                                    MessageBox.Show("Ваш аккаунт заблокирован.", "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Error);
                                    return;
                                }

                                if (BCrypt.Net.BCrypt.Verify(LoginPassword, storedPasswordHash))
                                {
                                    Coach_search.Models.UserType userType = Enum.Parse<Coach_search.Models.UserType>(userTypeString);
                                    MainWindow mainWindow = new MainWindow();
                                    mainWindow.Frame.Navigate(new MainPage(userName, userType, userId));
                                    mainWindow.Show();
                                    if (parameter is FrameworkElement element)
                                    {
                                        Window.GetWindow(element).Close();
                                    }
                                    else
                                    {
                                        Application.Current.MainWindow.Close();
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Неверный email или пароль.", "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                            else
                            {
                                MessageBox.Show("Неверный email или пароль.", "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка авторизации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnRegister(object parameter)
        {
            // Валидация полей
            if (string.IsNullOrWhiteSpace(RegisterName))
            {
                MessageBox.Show("Пожалуйста, введите имя.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (RegisterName.Length < 2)
            {
                MessageBox.Show("Имя должно содержать минимум 2 символа.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(RegisterEmail))
            {
                MessageBox.Show("Пожалуйста, введите email.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!IsValidEmail(RegisterEmail))
            {
                MessageBox.Show("Пожалуйста, введите корректный email (пример: user@example.com).", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(RegisterPassword))
            {
                MessageBox.Show("Пожалуйста, введите пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!IsValidPassword(RegisterPassword))
            {
                MessageBox.Show("Пароль должен содержать минимум 6 символов, включая хотя бы одну букву и одну цифру.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(SelectedUserType))
            {
                MessageBox.Show("Пожалуйста, выберите тип пользователя.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SelectedUserType != "Клиент" && SelectedUserType != "Репетитор")
            {
                MessageBox.Show("Тип пользователя должен быть 'Клиент' или 'Репетитор'.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Проверка существующего email
            try
            {
                if (_dbHelper.EmailExists(RegisterEmail))
                {
                    MessageBox.Show("Этот email уже зарегистрирован. Пожалуйста, используйте другой email.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка проверки email: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(RegisterPassword);
                Coach_search.Models.UserType userType = SelectedUserType switch
                {
                    "Клиент" => Coach_search.Models.UserType.Client,
                    "Репетитор" => Coach_search.Models.UserType.Tutor,
                    _ => Coach_search.Models.UserType.Client
                };

                int userId = _dbHelper.AddUser(RegisterName, RegisterEmail, hashedPassword, userType);

                if (userType == Coach_search.Models.UserType.Tutor)
                {
                    var tutor = new Tutor
                    {
                        UserId = userId,
                        Name = RegisterName,
                        Rating = 0,
                        PricePerHour = 0
                    };
                    _dbHelper.AddTutor(tutor);
                }
                else if (userType == Coach_search.Models.UserType.Client)
                {
                    var client = new Client
                    {
                        UserId = userId,
                        Name = RegisterName
                    };
                    _dbHelper.AddClient(client);
                }

                MessageBox.Show("Регистрация успешна! Теперь вы можете войти.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                ClearRegistrationFields();
                if (parameter is TabControl tabControl)
                {
                    tabControl.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка регистрации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            // Простая регулярка для проверки формата email
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailPattern);
        }

        private bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
                return false;

            // Пароль должен содержать хотя бы одну букву и одну цифру
            string passwordPattern = @"^(?=.*[A-Za-z])(?=.*\d).+$";
            return Regex.IsMatch(password, passwordPattern);
        }

        private void ClearRegistrationFields()
        {
            RegisterName = string.Empty;
            RegisterEmail = string.Empty;
            RegisterPassword = string.Empty;
            SelectedUserType = "Клиент";
        }
    }
}