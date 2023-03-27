using System.ComponentModel.DataAnnotations;

namespace API_Rentals.Models
{
    public class PointOfInterestForUpdateDto
    {
        [Required(ErrorMessage = "You should provide a name.")]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Description { get; set; }

    }
}
