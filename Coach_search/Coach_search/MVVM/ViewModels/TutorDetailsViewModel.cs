using Coach_search.Data;
using Coach_search.Models;
using Coach_search.MVVM.Models;
using Coach_search.MVVM.View;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Coach_search.ViewModels
{
    public class TutorDetailsViewModel : BaseViewModel
    {
        private readonly Tutor _tutor;
        private readonly DatabaseHelper _dbHelper;
        private ObservableCollection<Review> _reviews;

        public string Name => _tutor.Name;
        public string Subject => _tutor.Subject;
        public string Description => _tutor.Description;
        public string AvatarPath => _tutor.AvatarPath;
        public double Rating => _tutor.Rating;
        public decimal PricePerHour => _tutor.PricePerHour;

        public ObservableCollection<Review> Reviews
        {
            get => _reviews;
            set
            {
                _reviews = value;
                OnPropertyChanged();
            }
        }

        public ICommand CloseCommand { get; }

        public TutorDetailsViewModel(Tutor tutor)
        {
            _tutor = tutor ?? throw new ArgumentNullException(nameof(tutor));
            _dbHelper = new DatabaseHelper();

            // Загружаем отзывы о репетиторе
            LoadReviews();

            CloseCommand = new RelayCommand(CloseWindow);
        }

        private void LoadReviews()
        {
            try
            {
                // Здесь нам нужно получить отзывы о репетиторе (по TutorId)
                // В DatabaseHelper нет метода для получения отзывов о репетиторе, поэтому добавим его позже
                var reviews = _dbHelper.GetTutorReviews(_tutor.Id);
                Reviews = new ObservableCollection<Review>(reviews);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading reviews: {ex.Message}");
                Reviews = new ObservableCollection<Review>();
            }
        }

        private void CloseWindow(object parameter)
        {
            var window = Application.Current.Windows.OfType<TutorDetailsWindow>().FirstOrDefault();
            window?.Close();
        }
    }
}