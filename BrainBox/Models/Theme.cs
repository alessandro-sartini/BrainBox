using System.ComponentModel.DataAnnotations;

namespace BrainBox.Models
{
    public class Theme
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public ICollection<IdeaTheme> IdeaThemes { get; set; } = [];
    }
}
