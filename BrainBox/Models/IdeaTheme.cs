namespace BrainBox.Models
{
    public class IdeaTheme
    {
        public int IdeaId { get; set; }
        public Idea Idea { get; set; } = null!;

        public int ThemeId { get; set; }
        public Theme Theme { get; set; } = null!;
    }
}
