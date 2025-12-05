namespace BrainBox.Desktop.Model
{
    public class IdeaDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime LastModifiedAt { get; set; }
        public List<ThemeDto> Themes { get; set; } = new();
    }

    public class ThemeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class CreateIdeaDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<int> ThemeIds { get; set; } = new();
    }



    public class UpdateIdeaDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public List<int> ThemeIds { get; set; } = [];
    }
}
