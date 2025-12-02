namespace Soundmates.Api.RequestDTOs.Reports
{
    public class ReportUserDto
    {
        public string ReportedUserId { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
