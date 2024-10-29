namespace EldoMvideo.Models
{
    public class Delivery
    {
        public int id { get; set; }
        public string address { get; set; }
        public DateTime date { get; set; }
        public TimeSpan time { get; set; }
    }
}
