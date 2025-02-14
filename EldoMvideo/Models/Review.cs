namespace EldoMvideoAPI.Models;

public class Review
{
    public int id { get; set; }
    public int user_id { get; set; }
    public int product_id { get; set; }
    public int rating { get; set; }
    public string review { get; set; }
    public DateTime date { get; set; }
}