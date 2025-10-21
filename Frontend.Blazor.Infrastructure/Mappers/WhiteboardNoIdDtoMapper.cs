using UCR.ECCI.PI.ThemePark.Frontend.Blazor.Domain.Entities.ComponentsManagement;
using UCR.ECCI.PI.ThemePark.Frontend.Blazor.Domain.ValueObjects;
using UCR.ECCI.PI.ThemePark.Frontend.Blazor.Domain.ValueObjects.ComponentsManagement;
using UCR.ECCI.PI.ThemePark.Frontend.Blazor.Domain.ValueObjects.UniversityManagement;
using DomainValidationError = UCR.ECCI.PI.ThemePark.Frontend.Blazor.Domain.Exceptions.ValidationError;
using UCR.ECCI.PI.ThemePark.Frontend.Blazor.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Frontend.Blazor.Infrastructure.Kiota.Models;

namespace UCR.ECCI.PI.ThemePark.Frontend.Blazor.Infrastructure.Mappers;

/// <summary>
/// Provides methods to map <see cref="Whiteboard"/> domain entities
/// to <see cref="WhiteboardNoIdDto"/> data transfer objects without identifiers.
/// </summary>
internal static class WhiteboardNoIdDtoMapper
{
    /// <summary>
    /// Maps a <see cref="Whiteboard"/> entity to a <see cref="WhiteboardNoIdDto"/> object,
    /// excluding the identifier field, suitable for create operations.
    /// </summary>
    /// <param name="whiteboard">The whiteboard domain entity to map.</param>
    /// <returns>A <see cref="WhiteboardNoIdDto"/> representing the whiteboard data without an ID.</returns>
    internal static WhiteboardNoIdDto ToDto(Whiteboard whiteboard)
    {
        return new WhiteboardNoIdDto
        {
            Orientation = whiteboard.Orientation.Value,

            Dimensions = new DimensionsDto
            {
                Width = whiteboard.Dimensions.Width,
                Length = whiteboard.Dimensions.Length,
                Height = whiteboard.Dimensions.Height,
            },
            Position = new PositionDto
            {
                X = whiteboard.Position.X,
                Y = whiteboard.Position.Y,
                Z = whiteboard.Position.Z,
            },

            MarkerColor = whiteboard.MarkerColor!.Value
        };
    }
}
