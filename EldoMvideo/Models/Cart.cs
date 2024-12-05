namespace EldoMvideo.Models
{
    public class Cart
    {
        public Cart()
        {
            CartLines = new List<Product>();
        }

        public List<Product> CartLines { get; set; }

        public int FinalPrice
        {
            get
            {
                int sum = 0;
                foreach (var item in CartLines)
                {
                    sum += item.price;
                }
                return sum;
            }
        }
    }
}
