using Soundmates.Api.RequestDTOs.CustomValidations;
using System.ComponentModel.DataAnnotations;

namespace Soundmates.Api.RequestDTOs.Users;

public class UpdateUserProfileArtistDto : UpdateUserProfileDto
{
    [Required]
    [BirthDate]
    public required DateOnly BirthDate { get; set; }

    [Required]
    public required Guid GenderId { get; set; }
}
