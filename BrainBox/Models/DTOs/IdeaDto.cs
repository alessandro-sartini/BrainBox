namespace BrainBox.Models.DTOs
{
    public class IdeaDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime LastModifiedAt { get; set; }
        // Lista temi 
        public List<ThemeDto> Themes { get; set; } =[];
    }
}
