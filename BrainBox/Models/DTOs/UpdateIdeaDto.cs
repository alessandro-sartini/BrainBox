using System.ComponentModel.DataAnnotations;

namespace BrainBox.Models.DTOs
{
    public class UpdateIdeaDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;
    }
}
