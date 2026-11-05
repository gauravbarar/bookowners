namespace BookOwners.Application.Exceptions;

public sealed class UpstreamDataException(string message, Exception? innerException = null)
    : Exception(message, innerException);
