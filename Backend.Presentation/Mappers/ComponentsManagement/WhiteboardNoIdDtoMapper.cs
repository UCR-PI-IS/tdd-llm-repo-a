using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities.ComponentsManagement;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Domain.ValueObjects;
using UCR.ECCI.PI.ThemePark.Backend.Domain.ValueObjects.ComponentsManagement;
using UCR.ECCI.PI.ThemePark.Backend.Domain.ValueObjects.UniversityManagement;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Dtos.ComponentsManagement;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Mappers;


internal static class WhiteboardNoIdDtoMapper
{
    internal static WhiteboardNoIdDto ToDto(Whiteboard entity)
    {
        return new WhiteboardNoIdDto(
            entity.Orientation.Value,
            new PositionDto(entity.Position.X, entity.Position.Y, entity.Position.Z),
            new DimensionsDto(entity.Dimensions.Width, entity.Dimensions.Length, entity.Dimensions.Height),
            entity.MarkerColor!.Value!
            );
    }

    internal static Whiteboard ToEntity(WhiteboardNoIdDto dto)
    {
        var errors = new List<ValidationError>();

        // Orientation
        if (!Orientation.TryCreate(dto.Orientation, out var orientation))
            errors.Add(new ValidationError("Orientation", $"Invalid orientation: {dto.Orientation}"));

        // Dimensions
        if (!Dimension.TryCreate(dto.Dimensions.Width, dto.Dimensions.Length, dto.Dimensions.Height, out var dimensions))
            errors.Add(new ValidationError("Dimensions", "Invalid dimensions"));

        // Position
        if (!Coordinates.TryCreate(dto.Position.X, dto.Position.Y, dto.Position.Z, out var position))
            errors.Add(new ValidationError("Position", "Invalid position coordinates"));

        //Colors
        if (!Colors.TryCreate(dto.MarkerColor, out var markerColor))
            errors.Add(new ValidationError("Color", $"Invalid color: {dto.MarkerColor}"));

        if (errors.Any())
            throw new ValidationException(errors);

        return new Whiteboard(
            markerColor,
            orientation,
            position,
            dimensions
        );
    }
}