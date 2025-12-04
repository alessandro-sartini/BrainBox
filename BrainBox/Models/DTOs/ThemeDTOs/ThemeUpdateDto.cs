using System.ComponentModel.DataAnnotations;

namespace BrainBox.Models.DTOs.ThemeDTOs
{
    public class ThemeUpdateDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
    }
}
