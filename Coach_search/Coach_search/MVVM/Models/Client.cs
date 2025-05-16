namespace Coach_search.Models
{
    public class Client
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public List<Booking> Bookings { get; set; } = new();
    }

    public class Booking
    {
        public int Id { get; set; }
        public int ClientId { get; set; } // Добавляем ClientId
        public int TutorId { get; set; }
        public string TutorName { get; set; }
        public DateTime DateTime { get; set; }
        public string Status { get; set; }
        public bool CanCancel { get; set; }
        public string ClientName { get; set; }

        public bool CanLeaveReview { get; set; }
    }
}