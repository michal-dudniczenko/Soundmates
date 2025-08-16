using Microsoft.AspNetCore.Mvc;
using Soundmates.Api.DTOs.Matching;
using Soundmates.Api.DTOs.Users;
using Soundmates.Domain.Entities;
using Soundmates.Domain.Interfaces.Repositories;
using System.IdentityModel.Tokens.Jwt;

namespace Soundmates.Api.Controllers;

[Route("matching")]
[ApiController]
public class MatchingController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly ILikeRepository _likeRepository;
    private readonly IDislikeRepository _dislikeRepository;
    private readonly IMatchRepository _matchRepository;

    public MatchingController(IUserRepository userRepository, ILikeRepository likeRepository, IDislikeRepository dislikeRepository, IMatchRepository matchRepository)
    {
        _userRepository = userRepository;
        _likeRepository = likeRepository;
        _dislikeRepository = dislikeRepository;
        _matchRepository = matchRepository;
    }

    // POST /matching/like
    [HttpPost("like")]
    public async Task<IActionResult> CreateLike(
        [FromBody] SwipeDto swipeDto)
    {
        var subClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (subClaim is null || !int.TryParse(subClaim, out var authorizedUserId)
            || !(await _userRepository.CheckIfIdExistsAsync(authorizedUserId)))
        {
            return Unauthorized(new { message = "Invalid access token." });
        }

        var authorizedUser = await _userRepository.GetByIdAsync(authorizedUserId);
        if (authorizedUser == null || authorizedUser.IsLoggedOut || authorizedUser.IsFirstLogin || authorizedUserId != swipeDto.GiverId)
        {
            return Unauthorized(new { message = "Invalid access token." });
        }

        var receiver = await _userRepository.GetByIdAsync(swipeDto.ReceiverId);
        if (receiver == null || receiver.IsFirstLogin || !receiver.IsActive)
            return NotFound(new { message = $"No user with ID: {swipeDto.ReceiverId} was found." });

        var like = new Like { GiverId = swipeDto.GiverId, ReceiverId = swipeDto.ReceiverId };

        await _likeRepository.AddAsync(like);

        if (await _likeRepository.CheckIfExistsAsync(giverId: swipeDto.ReceiverId, receiverId: swipeDto.GiverId))
        {
            var match = new Match { User1Id = swipeDto.GiverId, User2Id = swipeDto.ReceiverId };
            await _matchRepository.AddAsync(match);
        }

        return Ok();
    }

    // POST /matching/dislike
    [HttpPost("dislike")]
    public async Task<IActionResult> CreateDislike(
        [FromBody] SwipeDto swipeDto)
    {
        var subClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (subClaim is null || !int.TryParse(subClaim, out var authorizedUserId)
            || !(await _userRepository.CheckIfIdExistsAsync(authorizedUserId)))
        {
            return Unauthorized(new { message = "Invalid access token." });
        }

        var authorizedUser = await _userRepository.GetByIdAsync(authorizedUserId);
        if (authorizedUser == null || authorizedUser.IsLoggedOut || authorizedUser.IsFirstLogin || authorizedUserId != swipeDto.GiverId)
        {
            return Unauthorized(new { message = "Invalid access token." });
        }

        var receiver = await _userRepository.GetByIdAsync(swipeDto.ReceiverId);
        if (receiver == null || receiver.IsFirstLogin || !receiver.IsActive)
            return NotFound(new { message = $"No user with ID: {swipeDto.ReceiverId} was found." });

        var dislike = new Dislike { GiverId = swipeDto.GiverId, ReceiverId = swipeDto.ReceiverId };

        await _dislikeRepository.AddAsync(dislike);

        return Ok();
    }

    // GET /matching/matches? limit = 20 & offset = 0
    [HttpGet("matches")]
    public async Task<IActionResult> GetMatches(
        [FromQuery] int limit = 20,
        [FromQuery] int offset = 0)
    {
        var subClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (subClaim is null || !int.TryParse(subClaim, out var authorizedUserId)
            || !(await _userRepository.CheckIfIdExistsAsync(authorizedUserId)))
        {
            return Unauthorized(new { message = "Invalid access token." });
        }

        var authorizedUser = await _userRepository.GetByIdAsync(authorizedUserId);
        if (authorizedUser == null || authorizedUser.IsLoggedOut || authorizedUser.IsFirstLogin)
        {
            return Unauthorized(new { message = "Invalid access token." });
        }

        try
        {
            var matches = await _matchRepository.GetUserMatchesAsync(authorizedUserId, limit, offset);

            var userProfiles = new List<OtherUserProfileDto>();

            foreach (var match in matches)
            {
                var user = await _userRepository.GetByIdAsync(
                    match.User1Id == authorizedUserId ? match.User2Id : match.User1Id
                );
                if (user is null || !user.IsActive || user.IsFirstLogin) continue;

                userProfiles.Add(new OtherUserProfileDto
                {
                    Id = user.Id,
                    Name = user.Name!,
                    Description = user.Description!,
                    BirthYear = (int)user.BirthYear!,
                    City = user.City!,
                    Country = user.Country!
                });
            }

            return Ok(userProfiles);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
