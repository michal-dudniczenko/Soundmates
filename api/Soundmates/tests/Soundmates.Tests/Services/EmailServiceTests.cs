namespace Soundmates.Tests.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Soundmates.Infrastructure.Services;
using System.Threading.Tasks;
using Xunit;

public class EmailServiceTests
{
    [Fact]
    public async Task SendEmailAsync_LogsWarning_WhenSmtpServerNotConfigured()
    {
        // Arrange
        var configurationMock = new Mock<IConfiguration>();
        var loggerMock = new Mock<ILogger<EmailService>>();

        var sectionMock = new Mock<IConfigurationSection>();
        sectionMock.Setup(s => s["SmtpServer"]).Returns("smtp.example.com"); // default not configured
        sectionMock.Setup(s => s["Port"]).Returns("587");
        sectionMock.Setup(s => s["SenderEmail"]).Returns("sender@example.com");
        sectionMock.Setup(s => s["Password"]).Returns("password");

        configurationMock.Setup(c => c.GetSection("EmailSettings")).Returns(sectionMock.Object);

        var service = new EmailService(configurationMock.Object, loggerMock.Object);

        // Act
        await service.SendEmailAsync("receiver@example.com", "Test Subject", "Test Body");

        // Assert
        loggerMock.Verify(
            l => l.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Email settings are not configured")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once
        );

        loggerMock.Verify(
            l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Email body (not sent)")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once
        );
    }

    [Fact]
    public async Task SendEmailAsync_LogsError_WhenExceptionThrown()
    {
        // Arrange
        var configurationMock = new Mock<IConfiguration>();
        var loggerMock = new Mock<ILogger<EmailService>>();

        var sectionMock = new Mock<IConfigurationSection>();
        sectionMock.Setup(s => s["SmtpServer"]).Returns("invalid-server"); // will cause exception
        sectionMock.Setup(s => s["Port"]).Returns("587");
        sectionMock.Setup(s => s["SenderEmail"]).Returns("sender@example.com");
        sectionMock.Setup(s => s["Password"]).Returns("password");

        configurationMock.Setup(c => c.GetSection("EmailSettings")).Returns(sectionMock.Object);

        var service = new EmailService(configurationMock.Object, loggerMock.Object);

        // Act
        await service.SendEmailAsync("receiver@example.com", "Test Subject", "Test Body");

        // Assert
        loggerMock.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Failed to send email")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once
        );

        loggerMock.Verify(
            l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Email body (failed to send)")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once
        );
    }
}

