using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Soundmates.Api.DTOs.Matching;
using Soundmates.Api.DTOs.Users;
using Soundmates.Api.Extensions;
using Soundmates.Domain.Entities;
using Soundmates.Domain.Interfaces.Repositories;

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
    [Authorize]
    public async Task<IActionResult> CreateLike(
        [FromBody] SwipeDto swipeDto)
    {
        var authorizedUser = await this.GetAuthorizedUserAsync(userRepository: _userRepository, checkForFirstLogin: true);

        if (authorizedUser is null)
        {
            return Unauthorized("");
        }

        var receiver = await _userRepository.GetByIdAsync(swipeDto.ReceiverId);
        if (receiver == null || receiver.IsFirstLogin || !receiver.IsActive)
            return NotFound(new { message = $"No user with ID: {swipeDto.ReceiverId} was found." });

        var like = new Like { GiverId = authorizedUser.Id, ReceiverId = swipeDto.ReceiverId };

        await _likeRepository.AddAsync(like);

        if (await _likeRepository.CheckIfExistsAsync(giverId: swipeDto.ReceiverId, receiverId: authorizedUser.Id))
        {
            var match = new Match { User1Id = authorizedUser.Id, User2Id = swipeDto.ReceiverId };
            await _matchRepository.AddAsync(match);
        }

        return Ok();
    }

    // POST /matching/dislike
    [HttpPost("dislike")]
    [Authorize]
    public async Task<IActionResult> CreateDislike(
        [FromBody] SwipeDto swipeDto)
    {
        var authorizedUser = await this.GetAuthorizedUserAsync(userRepository: _userRepository, checkForFirstLogin: true);

        if (authorizedUser is null)
        {
            return Unauthorized("");
        }

        var receiver = await _userRepository.GetByIdAsync(swipeDto.ReceiverId);
        if (receiver == null || receiver.IsFirstLogin || !receiver.IsActive)
            return NotFound(new { message = $"No user with ID: {swipeDto.ReceiverId} was found." });

        var dislike = new Dislike { GiverId = authorizedUser.Id, ReceiverId = swipeDto.ReceiverId };

        await _dislikeRepository.AddAsync(dislike);

        return Ok();
    }

    // GET /matching/matches? limit = 20 & offset = 0
    [HttpGet("matches")]
    [Authorize]
    public async Task<IActionResult> GetMatches(
        [FromQuery] int limit = 20,
        [FromQuery] int offset = 0)
    {
        var authorizedUser = await this.GetAuthorizedUserAsync(userRepository: _userRepository, checkForFirstLogin: true);

        if (authorizedUser is null)
        {
            return Unauthorized("");
        }

        try
        {
            var matches = await _matchRepository.GetUserMatchesAsync(authorizedUser.Id, limit, offset);

            var userProfiles = new List<OtherUserProfileDto>();

            foreach (var match in matches)
            {
                var user = await _userRepository.GetByIdAsync(
                    match.User1Id == authorizedUser.Id ? match.User2Id : match.User1Id
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
