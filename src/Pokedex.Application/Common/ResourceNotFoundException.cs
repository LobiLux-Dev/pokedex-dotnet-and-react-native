namespace Pokedex.Application.Common;

/// <summary>
/// Thrown when a catalogue resource does not exist.
/// </summary>
public sealed class ResourceNotFoundException : Exception
{
    public ResourceNotFoundException(string resourceName, string key)
        : base($"{resourceName} '{key}' no se encontró.")
    {
        ResourceName = resourceName;
        Key = key;
    }

    public string ResourceName { get; }

    public string Key { get; }
}
