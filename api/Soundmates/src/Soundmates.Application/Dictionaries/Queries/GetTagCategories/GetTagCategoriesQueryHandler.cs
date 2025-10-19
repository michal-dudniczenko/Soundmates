using MediatR;
using Soundmates.Application.Common;
using Soundmates.Application.ResponseDTOs.Dictionaries;
using Soundmates.Domain.Interfaces.Repositories;

namespace Soundmates.Application.Dictionaries.Queries.GetTagCategories;

public class GetTagCategoriesQueryHandler(
    IDictionaryRepository _dictionaryRepository
) : IRequestHandler<GetTagCategoriesQuery, Result<List<TagCategoryDto>>>
{
    public async Task<Result<List<TagCategoryDto>>> Handle(GetTagCategoriesQuery request, CancellationToken cancellationToken)
    {
        var tagCategories = await _dictionaryRepository.GetAllTagCategoriesAsync();

        var tagCategoriesDtos = tagCategories.Select(tc => new TagCategoryDto
        {
            Id = tc.Id,
            Name = tc.Name,
            IsForBand = tc.IsForBand
        }).ToList();

        return Result<List<TagCategoryDto>>.Success(tagCategoriesDtos);
    }
}
