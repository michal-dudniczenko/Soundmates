using MediatR;
using Soundmates.Application.Common;
using Soundmates.Application.Messages.Common;

namespace Soundmates.Application.Messages.Queries.GetConversationsPreview;

public record GetConversationsPreviewQuery(int Limit, int Offset, string? SubClaim) : IRequest<Result<List<MessageDto>>>;
