using Soundmates.Domain.Enums;

namespace Soundmates.Application.ResponseDTOs.Dictionaries;

public class UserTagCategoryDto
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required UserType UserType { get; set; }
}
