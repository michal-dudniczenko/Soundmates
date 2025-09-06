namespace Soundmates.Domain.Exceptions.MusicSamples;

public class DisplayOrderAlreadyLastException : DomainException
{
    public DisplayOrderAlreadyLastException()
    {
    }

    public DisplayOrderAlreadyLastException(string message)
        : base(message)
    {
    }

    public DisplayOrderAlreadyLastException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}