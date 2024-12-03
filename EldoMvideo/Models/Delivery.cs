namespace EldoMvideo.Models
{
    public class Delivery
    {
        public int id { get; set; }
        public string address { get; set; }
        public DateTime delivery_date { get; set; }
        public TimeSpan delivery_time { get; set; }
    }
}
