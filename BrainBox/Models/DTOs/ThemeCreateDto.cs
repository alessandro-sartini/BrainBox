using System.ComponentModel.DataAnnotations;

namespace BrainBox.Models.DTOs
{
    public class ThemeCreateDto
    {
        [Required(ErrorMessage = "Il nome del tema è obbligatorio")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
    }
}
