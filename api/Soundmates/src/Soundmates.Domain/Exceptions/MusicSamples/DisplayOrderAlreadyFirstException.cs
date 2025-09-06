namespace Soundmates.Domain.Exceptions.MusicSamples;

public class DisplayOrderAlreadyFirstException : DomainException
{
    public DisplayOrderAlreadyFirstException()
    {
    }

    public DisplayOrderAlreadyFirstException(string message)
        : base(message)
    {
    }

    public DisplayOrderAlreadyFirstException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
