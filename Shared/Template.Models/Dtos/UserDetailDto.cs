using System.ComponentModel.DataAnnotations;

namespace Template.Models.Dtos
{
    public class UserDetailDto
    {
        [Required]
        public required int Id { get; set; }
        [Required]
        public string Username { get; set; } = default!;
        public string? Role { get; set; }
    }
}