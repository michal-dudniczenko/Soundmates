using System.ComponentModel.DataAnnotations;

namespace Soundmates.Api.RequestDTOs.Auth;

public class RefreshTokenDto
{
    [Required]
    public required string RefreshToken { get; set; }
}
