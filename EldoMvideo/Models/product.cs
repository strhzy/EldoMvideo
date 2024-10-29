namespace EldoMvideo.Models
{
    public class Product
    {
        public int id { get; set; }
        public string product_name { get; set; }
        public int category_id { get; set; }
        public string pic_link { get; set; }
        public int price { get; set; }
    }
}
