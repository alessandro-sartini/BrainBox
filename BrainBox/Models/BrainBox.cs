using System.ComponentModel.DataAnnotations;

namespace BrainBox.Models
{
    public class Idea
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime LastModifiedAt { get; set; } = DateTime.Now;

        public ICollection<IdeaTheme> IdeaThemes { get; set; } = [];
    }
}
