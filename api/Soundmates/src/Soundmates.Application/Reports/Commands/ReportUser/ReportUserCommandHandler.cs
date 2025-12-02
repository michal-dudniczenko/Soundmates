using MediatR;
using Soundmates.Application.Common;
using Soundmates.Domain.Interfaces.Services;
using System.Threading;
using System.Threading.Tasks;

namespace Soundmates.Application.Reports.Commands.ReportUser
{
    public class ReportUserCommandHandler : IRequestHandler<ReportUserCommand, Result>
    {
        private readonly IEmailService _emailService;

        public ReportUserCommandHandler(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task<Result> Handle(ReportUserCommand request, CancellationToken cancellationToken)
        {
            var subject = $"User Report: {request.ReportingUserId} reported {request.ReportedUserId}";
            var body = $@"
                <h1>User Report</h1>
                <p><strong>Reporting User ID:</strong> {request.ReportingUserId}</p>
                <p><strong>Reported User ID:</strong> {request.ReportedUserId}</p>
                <p><strong>Reason:</strong> {request.Reason}</p>
                <p><strong>Description:</strong> {request.Description}</p>
            ";

            await _emailService.SendEmailAsync("soundmatesmoderation@gmail.com", subject, body);

            return Result.Success();
        }
    }
}
