using System.ComponentModel.DataAnnotations;

namespace BrainBox.Models.DTOs.IdeaDto
{
    public class CreateIdeaDto
    {
        [Required(ErrorMessage = "Il titolo è obbligatorio")]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descrizione è obbligatoria")]
        public string Description { get; set; } = string.Empty;

        public List<int> ThemeIds { get; set; } = [];
    }
}
