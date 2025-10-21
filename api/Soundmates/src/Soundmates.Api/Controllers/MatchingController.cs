using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Soundmates.Api.Extensions;
using Soundmates.Api.RequestDTOs.Matching;
using Soundmates.Application.Matching.Commands.CreateDislike;
using Soundmates.Application.Matching.Commands.CreateLike;
using Soundmates.Application.Matching.Commands.UpdateMatchPreference;
using Soundmates.Application.Matching.Queries.GetMatches;
using Soundmates.Application.Matching.Queries.GetMatchPreference;
using Soundmates.Application.Matching.Queries.GetPotentialMatchesArtists;
using Soundmates.Application.Matching.Queries.GetPotentialMatchesBands;
using Soundmates.Application.ResponseDTOs.Matching;
using Soundmates.Application.ResponseDTOs.Users;
using System.Security.Claims;

namespace Soundmates.Api.Controllers;

[Route("matching")]
[ApiController]
public class MatchingController(
    IMediator _mediator
) : ControllerBase
{
    // GET /matching/artists?limit=20&offset=0
    [HttpGet("artists")]
    [Authorize]
    public async Task<ActionResult<List<OtherUserProfileArtistDto>>> GetPotentialMatchesArtists(
        [FromQuery] int limit = 20,
        [FromQuery] int offset = 0)
    {
        var subClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var query = new GetPotentialMatchesArtistsQuery(
            Limit: limit,
            Offset: offset,
            SubClaim: subClaim);

        var result = await _mediator.Send(query);

        return this.ResultToHttpResponse(result);
    }

    // GET /matching/bands?limit=20&offset=0
    [HttpGet("bands")]
    [Authorize]
    public async Task<ActionResult<List<OtherUserProfileBandDto>>> GetPotentialMatchesBands(
        [FromQuery] int limit = 20,
        [FromQuery] int offset = 0)
    {
        var subClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var query = new GetPotentialMatchesBandsQuery(
            Limit: limit,
            Offset: offset,
            SubClaim: subClaim);

        var result = await _mediator.Send(query);

        return this.ResultToHttpResponse(result);
    }

    // GET /matching/match-preference
    [HttpGet("match-preference")]
    [Authorize]
    public async Task<ActionResult<MatchPreferenceDto>> GetMatchPreference()
    {
        var subClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var query = new GetMatchPreferenceQuery(SubClaim: subClaim);

        var result = await _mediator.Send(query);

        return this.ResultToHttpResponse(result);
    }

    // PUT /matching/match-preference
    [HttpPut("match-preference")]
    [Authorize]
    public async Task<ActionResult> UpdateMatchPreference(
        [FromBody] UpdateMatchPreferenceDto dto)
    {
        var subClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var query = new UpdateMatchPreferenceCommand(
            ShowArtists: dto.ShowArtists,
            ShowBands: dto.ShowBands,
            MaxDistance: dto.MaxDistance,
            CountryId: dto.CountryId,
            CityId: dto.CityId,
            ArtistMinAge: dto.ArtistMinAge,
            ArtistMaxAge: dto.ArtistMaxAge,
            ArtistGenderId: dto.ArtistGenderId,
            BandMinMembersCount: dto.BandMinMembersCount,
            BandMaxMembersCount: dto.BandMaxMembersCount,
            FilterTagsIds: dto.FilterTagsIds,
            SubClaim: subClaim);

        var result = await _mediator.Send(query);

        return this.ResultToHttpResponse(result);
    }


    // GET /matching/matches?limit=20&offset=0
    [HttpGet("matches")]
    [Authorize]
    public async Task<ActionResult<List<OtherUserProfileDto>>> GetMatches(
        [FromQuery] int limit = 20,
        [FromQuery] int offset = 0)
    {
        var subClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var query = new GetMatchesQuery(
            Limit: limit,
            Offset: offset,
            SubClaim: subClaim);

        var result = await _mediator.Send(query);

        return this.ResultToHttpResponse(result);
    }

    // POST /matching/like
    [HttpPost("like")]
    [Authorize]
    public async Task<ActionResult> CreateLike(
        [FromBody] SwipeDto swipeDto)
    {
        var subClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var command = new CreateLikeCommand(
            ReceiverId: swipeDto.ReceiverId,
            SubClaim: subClaim);

        var result = await _mediator.Send(command);

        return this.ResultToHttpResponse(result);
    }

    // POST /matching/dislike
    [HttpPost("dislike")]
    [Authorize]
    public async Task<ActionResult> CreateDislike(
        [FromBody] SwipeDto swipeDto)
    {
        var subClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var command = new CreateDislikeCommand(
            ReceiverId: swipeDto.ReceiverId,
            SubClaim: subClaim);

        var result = await _mediator.Send(command);

        return this.ResultToHttpResponse(result);
    }
}
