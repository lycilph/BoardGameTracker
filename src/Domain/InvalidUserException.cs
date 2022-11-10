namespace BoardGameTracker.Domain;

public class InvalidUserException : Exception
{
    public InvalidUserException(string message) : base(message) { }
}
