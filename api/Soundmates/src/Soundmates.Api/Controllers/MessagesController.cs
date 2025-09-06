using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Soundmates.Api.DTOs.Messages;
using Soundmates.Api.Extensions;
using Soundmates.Application.Messages.Commands.SendMessage;
using Soundmates.Application.Messages.Queries.GetConversation;
using Soundmates.Application.Messages.Queries.GetConversationsPreview;
using System.Security.Claims;

namespace Soundmates.Api.Controllers;

[Route("messages")]
[ApiController]
public class MessagesController(
    IMediator _mediator
) : ControllerBase
{
    // GET /messages/preview?limit=20&offset=0
    [HttpGet("preview")]
    [Authorize]
    public async Task<IActionResult> GetConversationsPreview(
        [FromQuery] int limit = 20,
        [FromQuery] int offset = 0)
    {
        var subClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var query = new GetConversationsPreviewQuery(
            Limit: limit,
            Offset: offset,
            SubClaim: subClaim);

        var result = await _mediator.Send(query);

        return this.ResultToHttpResponse(result);
    }

    // GET /messages/{userId}?limit=20&offset=0
    [HttpGet("{userId}")]
    [Authorize]
    public async Task<IActionResult> GetConversation(
        Guid userId,
        [FromQuery] int limit = 20,
        [FromQuery] int offset = 0)
    {
        var subClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var query = new GetConversationQuery(
            OtherUserId: userId,
            Limit: limit,
            Offset: offset,
            SubClaim: subClaim);

        var result = await _mediator.Send(query);

        return this.ResultToHttpResponse(result);
    }

    // POST /messages
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> SendMessage(
        [FromBody] SendMessageDto sendMessageDto)
    {
        var subClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var command = new SendMessageCommand(
            ReceiverId: sendMessageDto.ReceiverId,
            Content: sendMessageDto.Content,
            SubClaim: subClaim);

        var result = await _mediator.Send(command);

        return this.ResultToHttpResponse(result);
    }
}
