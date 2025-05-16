using System;
using System.Windows;
using System.Windows.Input;
using Coach_search.Data;
using Coach_search.Models;
using Coach_search.MVVM.View;
using BCrypt.Net;
using System.Windows.Controls;

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
                System.Diagnostics.Debug.WriteLine($"SelectedUserType changed to: {value}");
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
            System.Diagnostics.Debug.WriteLine($"Initial SelectedUserType: {SelectedUserType}");
        }

        private void OnLogin(object parameter)
        {
            System.Diagnostics.Debug.WriteLine("OnLogin started.");

            if (string.IsNullOrEmpty(LoginEmail) || string.IsNullOrEmpty(LoginPassword))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка");
                System.Diagnostics.Debug.WriteLine("Validation failed: Email or Password is empty.");
                return;
            }

            try
            {
                System.Diagnostics.Debug.WriteLine($"Attempting to connect to database with email: {LoginEmail}");
                using (var connection = new System.Data.SqlClient.SqlConnection(_dbHelper.ConnectionString))
                {
                    System.Diagnostics.Debug.WriteLine("Opening database connection...");
                    connection.Open();
                    System.Diagnostics.Debug.WriteLine("Database connection opened successfully.");

                    string query = "SELECT Id, Name, UserType, Password FROM Users WHERE Email = @Email";
                    using (var command = new System.Data.SqlClient.SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Email", LoginEmail);
                        System.Diagnostics.Debug.WriteLine("Executing SQL query...");
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                System.Diagnostics.Debug.WriteLine("User found in database.");

                                int userId = reader.GetInt32(0);
                                string userName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                                string userTypeString = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
                                string storedPasswordHash = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);

                                System.Diagnostics.Debug.WriteLine($"User data: Id={userId}, Name={userName}, UserType={userTypeString}, PasswordHash={storedPasswordHash}");

                                if (string.IsNullOrEmpty(storedPasswordHash))
                                {
                                    MessageBox.Show("Пароль пользователя не найден в базе данных.", "Ошибка авторизации");
                                    System.Diagnostics.Debug.WriteLine("Stored password hash is empty.");
                                    return;
                                }

                                System.Diagnostics.Debug.WriteLine("Verifying password...");
                                if (BCrypt.Net.BCrypt.Verify(LoginPassword, storedPasswordHash))
                                {
                                    System.Diagnostics.Debug.WriteLine("Password verification successful.");

                                    if (string.IsNullOrEmpty(userTypeString))
                                    {
                                        MessageBox.Show("Тип пользователя не определён.", "Ошибка авторизации");
                                        System.Diagnostics.Debug.WriteLine("UserType is empty.");
                                        return;
                                    }

                                    UserType userType;
                                    try
                                    {
                                        userType = Enum.Parse<UserType>(userTypeString);
                                        System.Diagnostics.Debug.WriteLine($"Parsed UserType: {userType}");
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show("Ошибка при определении типа пользователя.", "Ошибка авторизации");
                                        System.Diagnostics.Debug.WriteLine($"Error parsing UserType: {ex.Message}");
                                        return;
                                    }

                                    System.Diagnostics.Debug.WriteLine("Creating MainWindow...");
                                    MainWindow mainWindow = new MainWindow();
                                    System.Diagnostics.Debug.WriteLine("Navigating to MainPage...");
                                    mainWindow.Frame.Navigate(new MainPage(userName, userType, userId));
                                    System.Diagnostics.Debug.WriteLine("Showing MainWindow...");
                                    mainWindow.Show();
                                    System.Diagnostics.Debug.WriteLine("MainWindow shown.");
                                    System.Diagnostics.Debug.WriteLine("Closing LoginWindow...");
                                    if (parameter is FrameworkElement element)
                                    {
                                        Window.GetWindow(element).Close();
                                    }
                                    else
                                    {
                                        System.Diagnostics.Debug.WriteLine("Parameter is not a FrameworkElement.");
                                        Application.Current.MainWindow.Close();
                                    }
                                    System.Diagnostics.Debug.WriteLine("LoginWindow closed.");
                                }
                                else
                                {
                                    MessageBox.Show("Неверный email или пароль.", "Ошибка авторизации");
                                    System.Diagnostics.Debug.WriteLine("Password verification failed.");
                                }
                            }
                            else
                            {
                                MessageBox.Show("Неверный email или пароль.", "Ошибка авторизации");
                                System.Diagnostics.Debug.WriteLine("User not found in database.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка авторизации: {ex.Message}", "Ошибка");
                System.Diagnostics.Debug.WriteLine($"Exception in OnLogin: {ex.Message}\nStackTrace: {ex.StackTrace}");
            }
        }

        private void OnRegister(object parameter)
        {
            System.Diagnostics.Debug.WriteLine("OnRegister started.");
            System.Diagnostics.Debug.WriteLine($"Current SelectedUserType before validation: {SelectedUserType}");

            if (string.IsNullOrEmpty(RegisterName) || string.IsNullOrEmpty(RegisterEmail) || string.IsNullOrEmpty(RegisterPassword) || string.IsNullOrEmpty(SelectedUserType))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка");
                System.Diagnostics.Debug.WriteLine("Validation failed: One or more fields are empty.");
                return;
            }

            System.Diagnostics.Debug.WriteLine($"Current SelectedUserType after validation: {SelectedUserType}");

            try
            {
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(RegisterPassword);
                System.Diagnostics.Debug.WriteLine($"SelectedUserType before switch: {SelectedUserType}");
                UserType userType = SelectedUserType switch
                {
                    "Клиент" => UserType.Client,
                    "Репетитор" => UserType.Tutor,
                    _ => UserType.Client
                };

                System.Diagnostics.Debug.WriteLine($"Registering user with UserType: {userType}");

                int userId = _dbHelper.AddUser(RegisterName, RegisterEmail, hashedPassword, userType);

                
                if (userType == UserType.Tutor)
                {
                    var tutor = new Tutor
                    {
                        UserId = userId,
                        Name = RegisterName,
                        Rating = 0,
                        PricePerHour = 0
                        
                    };
                    _dbHelper.AddTutor(tutor);
                    System.Diagnostics.Debug.WriteLine($"Added tutor with UserId: {userId}");
                }
                else if (userType == UserType.Client)
                {
                    var client = new Client
                    {
                        UserId = userId,
                        Name = RegisterName
                    };
                    _dbHelper.AddClient(client);
                    System.Diagnostics.Debug.WriteLine($"Added client with UserId: {userId}");
                }

                MessageBox.Show("Регистрация успешна! Теперь вы можете войти.", "Успех");
                System.Diagnostics.Debug.WriteLine("Registration successful.");
                ClearRegistrationFields();
                System.Diagnostics.Debug.WriteLine($"SelectedUserType after ClearRegistrationFields: {SelectedUserType}");

                if (parameter is TabControl tabControl)
                {
                    tabControl.SelectedIndex = 0;
                    System.Diagnostics.Debug.WriteLine("Switched to login tab.");
                }
            }
            catch (System.Data.SqlClient.SqlException ex) when (ex.Number == 2627)
            {
                MessageBox.Show("Этот email уже зарегистрирован.", "Ошибка");
                System.Diagnostics.Debug.WriteLine("Registration failed: Email already exists.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка регистрации: {ex.Message}", "Ошибка");
                System.Diagnostics.Debug.WriteLine($"Exception in OnRegister: {ex.Message}\nStackTrace: {ex.StackTrace}");
            }
        }

        private void ClearRegistrationFields()
        {
            RegisterName = string.Empty;
            RegisterEmail = string.Empty;
            RegisterPassword = string.Empty;
            SelectedUserType = "Клиент";
            System.Diagnostics.Debug.WriteLine("Registration fields cleared.");
        }
    }
}