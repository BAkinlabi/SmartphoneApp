
namespace SmartphoneApp.ModelDTOs
{
    public class ReviewDTO
    {
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime Date { get; set; }
        public string? ReviewerName { get; set; }
        public string? ReviewerEmail { get; set; }
    }
}
