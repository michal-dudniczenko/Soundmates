using MediatR;
using Soundmates.Application.Common;
using Soundmates.Application.ResponseDTOs.Messages;

namespace Soundmates.Application.Messages.Queries.GetConversationsPreview;

public record GetConversationsPreviewQuery(string? SubClaim) : IRequest<Result<List<MessageDto>>>;
