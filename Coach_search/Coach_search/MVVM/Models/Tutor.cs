namespace Coach_search.Models
{
    public class Tutor
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; } 
        public string AvatarPath { get; set; } 
        public double Rating { get; set; }
        public decimal PricePerHour { get; set; }
        

    }
}