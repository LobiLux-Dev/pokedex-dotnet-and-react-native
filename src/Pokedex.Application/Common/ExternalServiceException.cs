namespace Pokedex.Application.Common;

/// <summary>
/// Thrown when the upstream PokeAPI cannot be reached or returns an unexpected payload.
/// </summary>
public sealed class ExternalServiceException : Exception
{
    public ExternalServiceException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}
