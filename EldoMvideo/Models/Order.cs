namespace EldoMvideo.Models
{
    public class Order
    {
        public int id { get; set; }
        public int account_id { get; set; }
        public int delivery_id { get; set; }
        public DateTime order_date { get; set; }
        public int total_sum { get; set; }
    }
}
