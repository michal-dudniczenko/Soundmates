using MediatR;
using Soundmates.Application.Common;
using Soundmates.Application.ResponseDTOs.Dictionaries;

namespace Soundmates.Application.Dictionaries.Queries.GetTagCategories;

public record GetTagCategoriesQuery() : IRequest<Result<List<UserTagCategoryDto>>>;
