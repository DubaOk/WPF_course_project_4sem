using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Coach_search.Data;
using Coach_search.Models;
using Coach_search.MVVM.View;
using BCrypt.Net;
using Coach_search.ViewModels;

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
            if (string.IsNullOrEmpty(LoginEmail) || string.IsNullOrEmpty(LoginPassword))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка");
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
                                    MessageBox.Show("Ваш аккаунт заблокирован.", "Ошибка авторизации");
                                    return;
                                }

                                if (BCrypt.Net.BCrypt.Verify(LoginPassword, storedPasswordHash))
                                {
                                    Coach_search.Models.UserType userType = Enum.Parse<Coach_search.Models.UserType>(userTypeString);
                                    MainWindow mainWindow = new MainWindow();
                                    if (userType == Coach_search.Models.UserType.Admin)
                                    {
                                        mainWindow.Frame.Navigate(new MainPage(userName, userType, userId));
                                    }
                                    else
                                    {
                                        mainWindow.Frame.Navigate(new MainPage(userName, userType, userId));
                                    }
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
                                    MessageBox.Show("Неверный email или пароль.", "Ошибка авторизации");
                                }
                            }
                            else
                            {
                                MessageBox.Show("Неверный email или пароль.", "Ошибка авторизации");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка авторизации: {ex.Message}", "Ошибка");
            }
        }

        private void OnRegister(object parameter)
        {
            if (string.IsNullOrEmpty(RegisterName) || string.IsNullOrEmpty(RegisterEmail) || string.IsNullOrEmpty(RegisterPassword) || string.IsNullOrEmpty(SelectedUserType))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка");
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

                MessageBox.Show("Регистрация успешна! Теперь вы можете войти.", "Успех");
                ClearRegistrationFields();
                if (parameter is TabControl tabControl)
                {
                    tabControl.SelectedIndex = 0;
                }
            }
            catch (System.Data.SqlClient.SqlException ex) when (ex.Number == 2627)
            {
                MessageBox.Show("Этот email уже зарегистрирован.", "Ошибка");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка регистрации: {ex.Message}", "Ошибка");
            }
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