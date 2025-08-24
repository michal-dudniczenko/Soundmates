using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soundmates.Api.DTOs.Messages;
using Soundmates.Domain.Entities;
using Soundmates.Domain.Interfaces.Repositories;
using System.IdentityModel.Tokens.Jwt;

namespace Soundmates.Api.Controllers;

[Route("messages")]
[ApiController]
public class MessagesController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IMatchRepository _matchRepository;
    private readonly IMessageRepository _messageRepository;

    public MessagesController(IUserRepository userRepository, IMatchRepository matchRepository, IMessageRepository messageRepository)
    {
        _userRepository = userRepository;
        _matchRepository = matchRepository;
        _messageRepository = messageRepository;
    }

    // GET /messages/preview?limit=20&offset=0
    [HttpGet("preview")]
    [Authorize]
    public async Task<IActionResult> GetMessagesPreview(
        [FromQuery] int limit = 20,
        [FromQuery] int offset = 0)
    {
        var subClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (subClaim is null || !int.TryParse(subClaim, out var authorizedUserId))
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
            var lastMessages = await _messageRepository.GetConversationsLastMessagesAsync(
                userId: authorizedUserId,
                limit: limit,
                offset: offset);

            var lastMessagesDtos = lastMessages.Select(m => new MessageDto
            {
                Content = m.Content,
                Timestamp = m.Timestamp,
                SenderId = m.SenderId,
                ReceiverId = m.ReceiverId
            });

            return Ok(lastMessagesDtos);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // GET /messages/{userId}?limit=20&offset=0
    [HttpGet("{userId}")]
    [Authorize]
    public async Task<IActionResult> GetMessages(
        int userId,
        [FromQuery] int limit = 20,
        [FromQuery] int offset = 0)
    {
        var subClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (subClaim is null || !int.TryParse(subClaim, out var authorizedUserId))
        {
            return Unauthorized(new { message = "Invalid access token." });
        }

        var authorizedUser = await _userRepository.GetByIdAsync(authorizedUserId);
        if (authorizedUser == null || authorizedUser.IsLoggedOut || authorizedUser.IsFirstLogin)
        {
            return Unauthorized(new { message = "Invalid access token." });
        }

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null || !user.IsActive || user.IsFirstLogin)
        {
            return NotFound(new { message = "No user with specified id." });
        }

        try
        {
            var conversation = await _messageRepository.GetConversationAsync(
                user1Id: authorizedUserId,
                user2Id: userId,
                limit: limit,
                offset: offset);

            var conversationDtos = conversation.Select(m => new MessageDto
            {
                Content = m.Content,
                Timestamp = m.Timestamp,
                SenderId = m.SenderId,
                ReceiverId = m.ReceiverId
            });

            return Ok(conversationDtos);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // POST /messages
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> SendMessage(
        [FromBody] SendMessageDto sendMessageDto)
    {
        var subClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (subClaim is null || !int.TryParse(subClaim, out var authorizedUserId))
        {
            return Unauthorized(new { message = "Invalid access token." });
        }

        var authorizedUser = await _userRepository.GetByIdAsync(authorizedUserId);
        if (authorizedUser == null || authorizedUser.IsLoggedOut || authorizedUser.IsFirstLogin)
        {
            return Unauthorized(new { message = "Invalid access token." });
        }

        var user = await _userRepository.GetByIdAsync(sendMessageDto.ReceiverId);
        if (user == null || !user.IsActive || user.IsFirstLogin)
        {
            return NotFound(new { message = "No user with specified id." });
        }

        var doesMatchExists = await _matchRepository.CheckIfMatchExistsAsync(authorizedUserId, sendMessageDto.ReceiverId);

        if (!doesMatchExists)
        {
            return Unauthorized(new { message = "You can send message only to users you have matched with." });
        }

        var message = new Message
        {
            Content = sendMessageDto.Content,
            SenderId = authorizedUserId,
            ReceiverId = sendMessageDto.ReceiverId
        };

        await _messageRepository.AddAsync(message);

        return Ok();
    }
}
