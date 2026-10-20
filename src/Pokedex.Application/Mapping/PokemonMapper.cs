using System.Globalization;
using Pokedex.Application.Dtos;
using Pokedex.Domain;

namespace Pokedex.Application.Mapping;

/// <summary>
/// Maps domain entities to the DTOs exposed by the API.
/// </summary>
public static class PokemonMapper
{
    public static PokemonSummaryDto ToSummaryDto(this PokemonSummary summary) => new(
        summary.Id,
        summary.Name,
        ToDisplayName(summary.Name),
        summary.Types,
        summary.SpriteUrl,
        summary.ArtworkUrl);

    public static PokemonDetailDto ToDetailDto(
        this Pokemon pokemon,
        PokemonSpecies? species,
        EvolutionChainDto? evolution,
        IReadOnlyList<PokemonMoveDto> moves) => new(
        pokemon.Id,
        pokemon.Name,
        ToDisplayName(pokemon.Name),
        pokemon.Types.Select(type => type.Name).ToList(),
        pokemon.HeightInMeters,
        pokemon.WeightInKg,
        pokemon.BaseExperience,
        pokemon.TotalStats,
        pokemon.Abilities.Select(ability => new PokemonAbilityDto(ability.Name, ability.IsHidden)).ToList(),
        pokemon.Stats.Select(stat => new PokemonStatDto(stat.Name, stat.BaseValue, stat.Effort)).ToList(),
        new PokemonSpritesDto(
            pokemon.Sprites.FrontDefault,
            pokemon.Sprites.FrontShiny,
            pokemon.Sprites.BackDefault,
            pokemon.Sprites.BackShiny,
            pokemon.Sprites.FrontFemale,
            pokemon.Sprites.FrontShinyFemale),
        species?.Genus,
        species?.Description,
        species?.Habitat,
        species?.Color,
        species?.IsLegendary ?? false,
        species?.IsMythical ?? false,
        moves,
        evolution);

    public static PokemonMoveDto ToMoveDto(this PokemonMove move) => new(
        move.Name,
        move.Type,
        move.DamageClass,
        move.Power,
        move.Accuracy,
        move.Level,
        move.LearnMethod,
        move.VersionGroup);

    public static EvolutionChainDto ToEvolutionDto(this EvolutionChain chain) => new(
        chain.Id,
        ToNodeDto(chain.Root));

    public static PokemonTypeDto ToTypeDto(this PokemonTypeInfo type) => new(type.Name, type.PokemonCount);

    private static EvolutionNodeDto ToNodeDto(EvolutionNode node) => new(
        node.SpeciesName,
        node.Details.Select(detail => new EvolutionDetailDto(
            detail.Trigger,
            detail.MinLevel,
            detail.Item,
            detail.HeldItem,
            detail.KnownMove,
            detail.TimeOfDay,
            detail.TurnUpsideDown,
            detail.Gender)).ToList(),
        node.EvolvesTo.Select(ToNodeDto).ToList());

    /// <summary>
    /// Turns an API identifier such as <c>mr-mime</c> into a human readable name: <c>Mr Mime</c>.
    /// </summary>
    public static string ToDisplayName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return string.Empty;
        }

        var spaced = name.Replace('-', ' ').Replace('_', ' ');
        return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(spaced);
    }
}
