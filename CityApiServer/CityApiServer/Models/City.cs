namespace CityApiServer.Models
{
    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Uri { get; set; }
        public int? Population { get; set; }
        public string? Description { get; set; }
        public string? History { get; set; }
        public string? ImageUrl { get; set; } // Додаткове поле для зображення
    }
}
