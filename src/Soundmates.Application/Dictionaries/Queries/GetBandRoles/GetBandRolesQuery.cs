using MediatR;
using Soundmates.Application.Common;
using Soundmates.Application.ResponseDTOs.Dictionaries;

namespace Soundmates.Application.Dictionaries.Queries.GetBandRoles;

public record GetBandRolesQuery : IRequest<Result<List<BandRoleDto>>>;
