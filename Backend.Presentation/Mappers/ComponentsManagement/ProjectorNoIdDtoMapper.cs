﻿using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities.ComponentsManagement;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Domain.ValueObjects;
using UCR.ECCI.PI.ThemePark.Backend.Domain.ValueObjects.ComponentsManagement;
using UCR.ECCI.PI.ThemePark.Backend.Domain.ValueObjects.UniversityManagement;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Dtos.ComponentsManagement;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Mappers;

internal static class ProjectorNoIdDtoMapper
{
    internal static ProjectorNoIdDto ToDto(Projector projector)
    {
        return new ProjectorNoIdDto(
            projector.Orientation.Value,
            new PositionDto(projector.Position.X, projector.Position.Y, projector.Position.Z),
            new DimensionsDto(projector.Dimensions.Width, projector.Dimensions.Length, projector.Dimensions.Height),
            new ProjectionAreaDto(projector.ProjectionArea!.Height, projector.ProjectionArea.Length),
            projector.ProjectedContent!
        );
    }

    internal static Projector ToEntity(ProjectorNoIdDto dto)
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

        // Projection Area
        if (!Area2D.TryCreate(dto.ProjectionArea.ProjectedHeight, dto.ProjectionArea.ProjectedWidth, out var projectionArea))
            errors.Add(new ValidationError("Projection Area", "Invalid projection area dimensions"));

        if (errors.Count > 0)
            throw new ValidationException(errors);

        return new Projector(
            dto.ProjectedContent,
            projectionArea!,
            orientation!,
            position!,
            dimensions!
        );
    }

}