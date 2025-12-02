using MediatR;
using Soundmates.Application.Common;

namespace Soundmates.Application.Reports.Commands.ReportUser
{
    public record ReportUserCommand(string ReportingUserId, string ReportedUserId, string Reason, string Description) : IRequest<Result>;
}
