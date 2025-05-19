using GalaSoft.MvvmLight;

namespace Coach_search.Models
{
    public class Client
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public List<Booking> Bookings { get; set; } = new();

        public string AvatarPath { get; set; }
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
    }
}