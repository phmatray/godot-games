namespace Raga.Server.Common.Exceptions;

public class GachaException : Exception
{
    public GachaException()
    {
    }

    public GachaException(string message)
        : base(message)
    {
    }

    public GachaException(string message, Exception inner)
        : base(message, inner)
    {
    }
}