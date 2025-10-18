using MediatR;
using Soundmates.Application.Common;
using Soundmates.Application.ResponseDTOs.Dictionaries;

namespace Soundmates.Application.Dictionaries.Queries.GetTags;

public record GetTagsQuery : IRequest<Result<List<UserTagDto>>>;
