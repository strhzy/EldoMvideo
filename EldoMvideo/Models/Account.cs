namespace EldoMvideo.Models
{
    public class Account
    {
        public int id { get; set; }
        public string acc_login { get; set; }
        public string password { get; set; }
        public int role_id { get; set; }
        public int user_id { get; set; }
    }
}
