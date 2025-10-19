using MediatR;
using Soundmates.Application.Common;
using Soundmates.Application.ResponseDTOs.Dictionaries;
using Soundmates.Domain.Interfaces.Repositories;

namespace Soundmates.Application.Dictionaries.Queries.GetTags;

public class GetTagsQueryHandler(
    IDictionaryRepository _dictionaryRepository
) : IRequestHandler<GetTagsQuery, Result<List<TagDto>>>
{
    public async Task<Result<List<TagDto>>> Handle(GetTagsQuery request, CancellationToken cancellationToken)
    {
        var tags = await _dictionaryRepository.GetAllTagsAsync();

        var tagsDtos = tags.Select(t => new TagDto
        {
            Id = t.Id,
            Name = t.Name,
            TagCategoryId = t.TagCategoryId
        }).ToList();

        return Result<List<TagDto>>.Success(tagsDtos);
    }
}
