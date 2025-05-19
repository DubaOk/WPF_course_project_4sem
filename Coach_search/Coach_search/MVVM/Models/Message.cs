using Coach_search.ViewModels;
using GalaSoft.MvvmLight;

namespace Coach_search.Models
{
    public class Message : BaseViewModel
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
        private bool _isSentByCurrentUser;

        public bool IsSentByCurrentUser
        {
            get => _isSentByCurrentUser;
            set
            {
                _isSentByCurrentUser = value;
                OnPropertyChanged();
            }
        }
    }
}