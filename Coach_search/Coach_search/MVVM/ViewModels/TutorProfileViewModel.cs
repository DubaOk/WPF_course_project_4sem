using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Coach_search.Data;
using Coach_search.Models;
using Coach_search.MVVM.Models;
using Coach_search.MVVM.View;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace Coach_search.ViewModels
{
    public class TutorProfileViewModel : BaseViewModel, IDataErrorInfo
    {
        private readonly DatabaseHelper _dbHelper;
        private Tutor _tutor;
        private string _name;
        private string _subject;
        private string _description;
        private string _avatarPath;
        private double _rating;
        private decimal _pricePerHour;
        private string _priceText;
        private List<string> _subjects;
        private bool _isVisible;
        private bool _hasErrors;

        public string Error => null;

        public Dictionary<string, string> ValidationErrors { get; } = new Dictionary<string, string>();

        public bool HasErrors
        {
            get => _hasErrors;
            set
            {
                _hasErrors = value;
                OnPropertyChanged();
            }
        }

        public string this[string propertyName]
        {
            get
            {
                ValidateProperty(propertyName);
                return ValidationErrors.ContainsKey(propertyName) ? ValidationErrors[propertyName] : string.Empty;
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
                ValidateProperty(nameof(Name));
            }
        }

        public string Subject
        {
            get => _subject;
            set
            {
                _subject = value;
                OnPropertyChanged();
                ValidateProperty(nameof(Subject));
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged();
                ValidateProperty(nameof(Description));
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

        public double Rating
        {
            get => _rating;
            set
            {
                _rating = value;
                OnPropertyChanged();
            }
        }

        public decimal PricePerHour
        {
            get => _pricePerHour;
            set
            {
                _pricePerHour = value;
                OnPropertyChanged();
            }
        }

        public string PriceText
        {
            get => _priceText;
            set
            {
                _priceText = value;
                if (decimal.TryParse(value, out decimal price))
                {
                    PricePerHour = price;
                }
                OnPropertyChanged();
                ValidateProperty(nameof(PriceText));
            }
        }

        public List<string> Subjects
        {
            get => _subjects;
            set
            {
                _subjects = value;
                OnPropertyChanged();
            }
        }

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                _isVisible = value;
                OnPropertyChanged();
            }
        }

        public ICommand UpdateProfileCommand { get; }
        public ICommand UploadAvatarCommand { get; }
        public ICommand GoBackCommand { get; }

        public TutorProfileViewModel(int userId)
        {
            _dbHelper = new DatabaseHelper();
            _tutor = _dbHelper.GetTutorByUserId(userId);

            if (_tutor != null)
            {
                Name = _tutor.Name;
                Subject = _tutor.Subject;
                Description = _tutor.Description;
                AvatarPath = _tutor.AvatarPath;
                Rating = _tutor.Rating;
                PricePerHour = _tutor.PricePerHour;
                PriceText = _tutor.PricePerHour.ToString();
                IsVisible = _tutor.IsVisible;
            }
            else
            {
                Name = string.Empty;
                Subject = string.Empty;
                Description = string.Empty;
                AvatarPath = string.Empty;
                Rating = 0;
                PricePerHour = 0;
                PriceText = "0";
                IsVisible = false;
            }

            Subjects = new List<string>
            {
                "Математика",
                "Физика",
                "Химия",
                "Английский язык",
                "Русский язык",
                "История",
                "Биология"
            };

            ValidateAll();

            UpdateProfileCommand = new RelayCommand(UpdateProfile, _ => !HasErrors);
            UploadAvatarCommand = new RelayCommand(UploadAvatar);
            GoBackCommand = new RelayCommand(GoBack);
        }

        private void ValidateAll()
        {
            ValidateProperty(nameof(Name));
            ValidateProperty(nameof(Subject));
            ValidateProperty(nameof(Description));
            ValidateProperty(nameof(PriceText));

            HasErrors = ValidationErrors.Any();
        }

        private void ValidateProperty(string propertyName)
        {
            string error = string.Empty;
            ValidationErrors.Remove(propertyName);

            switch (propertyName)
            {
                case nameof(Name):
                    if (string.IsNullOrWhiteSpace(Name))
                    {
                        error = "ФИО не может быть пустым";
                    }
                    else if (Name.Length < 2 || Name.Length > 50)
                    {
                        error = "ФИО должно содержать от 2 до 50 символов";
                    }
                    else if (!Regex.IsMatch(Name, @"^[а-яА-ЯёЁa-zA-Z\s-]+$"))
                    {
                        error = "ФИО может содержать только буквы, пробелы и дефисы";
                    }
                    break;

                case nameof(Subject):
                    if (string.IsNullOrWhiteSpace(Subject))
                    {
                        error = "Предмет не может быть пустым";
                    }
                    break;

                case nameof(Description):
                    if (string.IsNullOrWhiteSpace(Description))
                    {
                        error = "Описание не может быть пустым";
                    }
                    else if (Description.Length > 200)
                    {
                        error = "Описание не должно превышать 200 символов";
                    }
                    break;

                case nameof(PriceText):
                    if (string.IsNullOrWhiteSpace(PriceText))
                    {
                        error = "Цена не может быть пустой";
                    }
                    else if (!decimal.TryParse(PriceText, out decimal price) || price <= 0)
                    {
                        error = "Цена должна быть положительным числом";
                    }
                    break;
            }

            if (!string.IsNullOrEmpty(error))
            {
                ValidationErrors[propertyName] = error;
            }

            HasErrors = ValidationErrors.Any();
            OnPropertyChanged(nameof(ValidationErrors));
        }

        private void UpdateProfile(object parameter)
        {
            ValidateAll();
            if (HasErrors)
            {
                MessageBox.Show("Пожалуйста, исправьте ошибки перед сохранением.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (_tutor == null)
                {
                    _tutor = new Tutor { UserId = _tutor?.UserId ?? 0 };
                }

                _tutor.Name = Name;
                _tutor.Subject = Subject;
                _tutor.Description = Description;
                _tutor.AvatarPath = AvatarPath;
                _tutor.Rating = Rating;
                _tutor.PricePerHour = PricePerHour;
                _tutor.IsVisible = IsVisible;

                _dbHelper.UpdateTutor(_tutor);
                MessageBox.Show("Профиль успешно обновлен!", "Успех");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении профиля: {ex.Message}", "Ошибка");
                System.Diagnostics.Debug.WriteLine($"UpdateProfile error: {ex.Message}\nStackTrace: {ex.StackTrace}");
            }
        }

        private void UploadAvatar(object parameter)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Starting UploadAvatar...");

                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|All files (*.*)|*.*",
                    Title = "Выберите аватарку"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    string sourcePath = openFileDialog.FileName;
                    System.Diagnostics.Debug.WriteLine($"Selected file: {sourcePath}");

                    string fileName = Path.GetFileName(sourcePath);
                    string targetFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "Avatars");
                    string targetPath = Path.Combine(targetFolder, fileName);

                    System.Diagnostics.Debug.WriteLine($"Target folder: {targetFolder}");
                    System.Diagnostics.Debug.WriteLine($"Target path: {targetPath}");

                    Directory.CreateDirectory(targetFolder);
                    System.Diagnostics.Debug.WriteLine("Directory created (if it didn't exist).");

                    File.Copy(sourcePath, targetPath, true);
                    System.Diagnostics.Debug.WriteLine("File copied successfully.");

                    string absolutePath = Path.GetFullPath(targetPath);
                    System.Diagnostics.Debug.WriteLine($"Absolute path: {absolutePath}");

                    AvatarPath = absolutePath;
                    OnPropertyChanged(nameof(AvatarPath));
                    System.Diagnostics.Debug.WriteLine("AvatarPath updated and property changed notified.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке аватарки: {ex.Message}", "Ошибка");
                System.Diagnostics.Debug.WriteLine($"UploadAvatar error: {ex.Message}\nStackTrace: {ex.StackTrace}");
            }
        }

        private void GoBack(object parameter)
        {
            try
            {
                var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                if (mainWindow != null)
                {
                    var frame = mainWindow.FindName("MainFrame") as Frame;
                    if (frame != null)
                    {
                        frame.Navigate(new MainPage(_tutor?.Name ?? "User", UserType.Tutor, _tutor?.UserId ?? 0));
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("MainFrame not found in MainWindow.");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("MainWindow not found.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GoBack error: {ex.Message}\nStackTrace: {ex.StackTrace}");
                MessageBox.Show($"Ошибка при возврате назад: {ex.Message}", "Ошибка");
            }
        }
    }
}