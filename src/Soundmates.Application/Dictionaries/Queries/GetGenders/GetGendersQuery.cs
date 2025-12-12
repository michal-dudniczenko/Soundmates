using MediatR;
using Soundmates.Application.Common;
using Soundmates.Application.ResponseDTOs.Dictionaries;

namespace Soundmates.Application.Dictionaries.Queries.GetGenders;

public record GetGendersQuery() : IRequest<Result<List<GenderDto>>>;
