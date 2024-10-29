namespace EldoMvideo.Models
{
    public class Order
    {
        public int id { get; set; }
        public int account_id { get; set; }
        public int delivery_id { get; set; }
        public DateTime date { get; set; }
        public int sum { get; set; }
    }
}
