using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Soundmates.Api.Extensions;
using Soundmates.Api.RequestDTOs.Messages;
using Soundmates.Application.Messages.Commands.SendMessage;
using Soundmates.Application.Messages.Commands.ViewConversation;
using Soundmates.Application.Messages.Queries.GetConversation;
using Soundmates.Application.Messages.Queries.GetConversationsPreview;
using Soundmates.Application.ResponseDTOs.Messages;
using System.Security.Claims;

namespace Soundmates.Api.Controllers;

[Route("messages")]
[ApiController]
public class MessagesController(
    IMediator _mediator
) : ControllerBase
{
    // GET /messages/preview
    [HttpGet("preview")]
    [Authorize]
    public async Task<ActionResult<List<MessageDto>>> GetConversationsPreview()
    {
        var subClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var query = new GetConversationsPreviewQuery(SubClaim: subClaim);

        var result = await _mediator.Send(query);

        return this.ResultToHttpResponse(result);
    }

    // GET /messages/{userId}?limit=20&offset=0
    [HttpGet("{userId}")]
    [Authorize]
    public async Task<ActionResult<List<MessageDto>>> GetConversation(
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
    public async Task<ActionResult> SendMessage(
        [FromBody] SendMessageDto sendMessageDto)
    {
        var subClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var command = new SendMessageCommand(
            ReceiverId: sendMessageDto.ReceiverId,
            Content: sendMessageDto.Content.Trim(),
            SubClaim: subClaim);

        var result = await _mediator.Send(command);

        return this.ResultToHttpResponse(result);
    }

    // POST /messages/view-conversation/{userId}
    [HttpPost("view-conversation/{userId}")]
    [Authorize]
    public async Task<ActionResult> ViewConversation(Guid userId)
    {
        var subClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var command = new ViewConversationCommand(
            OtherUserId: userId,
            SubClaim: subClaim);

        var result = await _mediator.Send(command);

        return this.ResultToHttpResponse(result);
    }
}
