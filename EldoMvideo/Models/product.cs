namespace EldoMvideo.Models
{
    public class product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public product(int id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }
    }
}