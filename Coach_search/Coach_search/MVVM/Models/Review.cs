using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coach_search.MVVM.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int TutorId { get; set; }
        public int ClientId { get; set; }
        public string Text { get; set; }
        public int Rating { get; set; } // 1-5
        public DateTime CreatedAt { get; set; }
        public int BookingId { get; set; }
        public string TutorName { get; set; }

    }
}
