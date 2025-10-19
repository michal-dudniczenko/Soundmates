using MediatR;
using Soundmates.Application.Common;
using Soundmates.Application.ResponseDTOs.Dictionaries;

namespace Soundmates.Application.Matching.Commands.UpdateMatchPreference;

public record UpdateMatchPreferenceCommand(
    bool ShowArtists,
    bool ShowBands,
    int? MaxDistance,
    Guid? CountryId,
    Guid? CityId,
    int? ArtistMinAge,
    int? ArtistMaxAge,
    Guid? ArtistGenderId,
    int? BandMinMembersCount,
    int? BandMaxMembersCount,
    IList<TagDto> FilterTags,
    string? SubClaim
) : IRequest<Result>;