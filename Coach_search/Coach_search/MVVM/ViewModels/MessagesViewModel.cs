using Coach_search.Data;
using Coach_search.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Coach_search.MVVM.View;

namespace Coach_search.ViewModels
{
    public class MessagesViewModel : BaseViewModel
    {
        private readonly DatabaseHelper _dbHelper;
        private readonly int _currentUserId;
        private readonly int _otherUserId;
        private readonly UserType _currentUserType; // Добавляем для хранения типа пользователя
        private ObservableCollection<Message> _messages;
        private string _newMessageContent;
        private string _otherUserName;
        private readonly DispatcherTimer _refreshTimer;

        public ObservableCollection<Message> Messages
        {
            get => _messages;
            set
            {
                _messages = value;
                OnPropertyChanged();
            }
        }

        public string NewMessageContent
        {
            get => _newMessageContent;
            set
            {
                _newMessageContent = value;
                OnPropertyChanged();
            }
        }

        public string OtherUserName
        {
            get => _otherUserName;
            set
            {
                _otherUserName = value;
                OnPropertyChanged();
            }
        }

        public ICommand SendMessageCommand { get; }
        public ICommand SendMessageOnEnterCommand { get; }
        public ICommand GoBackCommand { get; } // Новая команда для возврата

        public MessagesViewModel(int currentUserId, int otherUserId)
        {
            _dbHelper = new DatabaseHelper();
            _currentUserId = currentUserId;
            _otherUserId = otherUserId;

            var currentUser = _dbHelper.GetUserById(currentUserId);
            if (currentUser == null || _dbHelper.GetUserById(otherUserId) == null)
            {
                throw new ArgumentException("One or both user IDs are invalid.");
            }

            _currentUserType = currentUser.UserType; // Сохраняем тип текущего пользователя

            Messages = new ObservableCollection<Message>();
            SendMessageCommand = new RelayCommand(SendMessage, () => !string.IsNullOrEmpty(NewMessageContent?.Trim()));
            SendMessageOnEnterCommand = new RelayCommand<KeyEventArgs>(SendMessageOnEnter);
            GoBackCommand = new RelayCommand(GoBack); // Инициализируем команду возврата

            LoadOtherUserName();
            LoadMessages();

            _refreshTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
            _refreshTimer.Tick += (s, e) => LoadMessages();
            _refreshTimer.Start();

            System.Diagnostics.Debug.WriteLine($"MessagesViewModel initialized: CurrentUserId={_currentUserId}, OtherUserId={_otherUserId}");
        }

        private void LoadOtherUserName()
        {
            var user = _dbHelper.GetUserById(_otherUserId);
            OtherUserName = user?.Name ?? "Неизвестный пользователь";

            var hasBooking = _dbHelper.HasBookingBetween(_currentUserId, _otherUserId);
            if (!hasBooking)
            {
                System.Diagnostics.Debug.WriteLine($"No booking found between {_currentUserId} and {_otherUserId}.");
                
            }
        }

        private void LoadMessages()
        {
            try
            {
                var messages = _dbHelper.GetMessagesBetween(_currentUserId, _otherUserId);
                Messages.Clear();
                foreach (var message in messages)
                {
                    message.IsSentByCurrentUser = message.SenderId == _currentUserId;
                    Messages.Add(message);
                }
                ScrollToBottom();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading messages: {ex.Message}");
            }
        }

        private void SendMessage()
        {
            if (string.IsNullOrEmpty(NewMessageContent?.Trim())) return;

            if (_dbHelper.GetUserById(_currentUserId) == null || _dbHelper.GetUserById(_otherUserId) == null)
            {
                MessageBox.Show("Один из пользователей не найден.", "Ошибка");
                return;
            }

            var message = new Message
            {
                SenderId = _currentUserId,
                ReceiverId = _otherUserId,
                Content = NewMessageContent.Trim(),
                SentAt = DateTime.Now,
                IsSentByCurrentUser = true
            };

            try
            {
                _dbHelper.AddMessage(message.SenderId, message.ReceiverId, message.Content, message.SentAt);
                Messages.Add(message);
                NewMessageContent = string.Empty;
                ScrollToBottom();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error sending message: {ex.Message}");
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка");
            }
        }

        public void SendMessageOnEnter(KeyEventArgs args)
        {
            if (args.Key == Key.Enter && !string.IsNullOrEmpty(NewMessageContent?.Trim()))
            {
                SendMessage();
                args.Handled = true;
            }
        }

        private void GoBack()
        {
            // Создаем новое окно выбора собеседника
            var selectConversationWindow = new SelectConversationWindow
            {
                DataContext = new SelectConversationViewModel(_currentUserId, _currentUserType)
            };
            selectConversationWindow.Show();

            // Закрываем текущее окно переписки
            Application.Current.Dispatcher.Invoke(() =>
            {
                var currentWindow = Application.Current.Windows.OfType<MessagesWindow>().FirstOrDefault();
                currentWindow?.Close();
            });
        }

        private void ScrollToBottom()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var window = Application.Current.Windows.OfType<MahApps.Metro.Controls.MetroWindow>()
                    .FirstOrDefault(w => w is Coach_search.MVVM.View.MessagesWindow);
                if (window != null)
                {
                    var scrollViewer = window.FindName("MessageScrollViewer") as ScrollViewer;
                    scrollViewer?.ScrollToBottom();
                }
            });
        }

        public override void Cleanup()
        {
            _refreshTimer?.Stop();
            base.Cleanup();
        }
    }
}