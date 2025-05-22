using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using GalaSoft.MvvmLight;

namespace Coach_search.Models
{
    public class Client : INotifyPropertyChanged
    {
        private int _userId;
        private string _name;
        private string _email;
        private List<Booking> _bookings = new();
        private string _avatarPath;

        public int UserId
        {
            get => _userId;
            set
            {
                _userId = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        public List<Booking> Bookings
        {
            get => _bookings;
            set
            {
                _bookings = value;
                OnPropertyChanged();
            }
        }

        public string AvatarPath
        {
            get => string.IsNullOrEmpty(_avatarPath) ? "/MVVM/View/images/icons/defaultavatar.png" : _avatarPath;
            set
            {
                _avatarPath = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Booking : ObservableObject
    {
        private int _id;
        private int _clientId;
        private int _tutorId;
        private string _tutorName;
        private DateTime _dateTime;
        private string _status;
        private bool _canCancel;
        private string _clientName;
        private bool _canLeaveReview;
        private bool _isTutorBlocked;
        private bool _isClientBlocked; // Новое свойство для отслеживания блокировки репетитора

        public int Id
        {
            get => _id;
            set => Set(ref _id, value);
        }

        public int ClientId
        {
            get => _clientId;
            set => Set(ref _clientId, value);
        }

        public int TutorId
        {
            get => _tutorId;
            set => Set(ref _tutorId, value);
        }

        public string TutorName
        {
            get => _tutorName;
            set => Set(ref _tutorName, value);
        }

        public DateTime DateTime
        {
            get => _dateTime;
            set => Set(ref _dateTime, value);
        }

        public string Status
        {
            get => _status;
            set => Set(ref _status, value);
        }

        public bool CanCancel
        {
            get => _canCancel;
            set => Set(ref _canCancel, value);
        }

        public string ClientName
        {
            get => _clientName;
            set => Set(ref _clientName, value);
        }

        public bool CanLeaveReview
        {
            get => _canLeaveReview;
            set => Set(ref _canLeaveReview, value);
        }

        public bool IsTutorBlocked
        {
            get => _isTutorBlocked;
            set => Set(ref _isTutorBlocked, value); // Уведомление об изменении
        }

        public bool IsClientBlocked
        {
            get => _isClientBlocked;
            set => Set(ref _isClientBlocked, value);
        }
    }
}