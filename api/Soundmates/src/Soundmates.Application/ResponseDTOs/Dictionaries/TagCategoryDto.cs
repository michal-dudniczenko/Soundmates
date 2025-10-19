namespace Soundmates.Application.ResponseDTOs.Dictionaries;

public class TagCategoryDto
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required bool IsForBand { get; set; }
}
