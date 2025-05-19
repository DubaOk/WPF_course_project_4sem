using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Coach_search.Data;
using Coach_search.Models;
using Coach_search.MVVM.View;
using Microsoft.Win32;

namespace Coach_search.ViewModels
{
    public class TutorProfileViewModel : BaseViewModel
    {
        private readonly DatabaseHelper _dbHelper;
        private Tutor _tutor;
        private string _name;
        private string _subject;
        private string _description;
        private string _avatarPath;
        private double _rating;
        private decimal _pricePerHour;
        private List<string> _subjects;
        private bool _isVisible;
        private bool _canMakeVisible;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
                UpdateCanMakeVisible();
            }
        }

        public string Subject
        {
            get => _subject;
            set
            {
                _subject = value;
                OnPropertyChanged();
                UpdateCanMakeVisible();
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged();
                UpdateCanMakeVisible();
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
                UpdateCanMakeVisible();
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
                if (_canMakeVisible) // Разрешаем изменение только если все поля заполнены
                {
                    _isVisible = value;
                    OnPropertyChanged();
                }
                else if (value) // Если пытаются включить видимость, но нельзя
                {
                    MessageBox.Show("Заполните все обязательные поля (ФИО, предмет, описание, стоимость), чтобы сделать профиль видимым.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    _isVisible = false;
                    OnPropertyChanged();
                }
            }
        }

        public bool CanMakeVisible
        {
            get => _canMakeVisible;
            set
            {
                _canMakeVisible = value;
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

            UpdateCanMakeVisible(); // Инициализируем состояние видимости

            UpdateProfileCommand = new RelayCommand(UpdateProfile, CanUpdateProfile);
            UploadAvatarCommand = new RelayCommand(UploadAvatar);
            GoBackCommand = new RelayCommand(GoBack);
        }

        private void UpdateProfile(object parameter)
        {
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

        private bool CanUpdateProfile(object parameter)
        {
            return !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Subject);
        }

        private void UpdateCanMakeVisible()
        {
            CanMakeVisible = !string.IsNullOrWhiteSpace(Name) &&
                             !string.IsNullOrWhiteSpace(Subject) &&
                             !string.IsNullOrWhiteSpace(Description) &&
                             PricePerHour > 0;
            if (!CanMakeVisible && IsVisible)
            {
                IsVisible = false; // Отключаем видимость, если поля стали незаполненными
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

                    // Преобразуем путь в абсолютный для корректного отображения в WPF
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