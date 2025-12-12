namespace Soundmates.Application.ResponseDTOs.Dictionaries;

public class TagDto
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required Guid TagCategoryId { get; set; }
}
