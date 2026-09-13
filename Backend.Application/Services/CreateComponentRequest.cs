namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Request object carrying the parameters needed to create a learning component.
/// </summary>
/// <param name="componentId">Unique identifier for the component. If null, an ID will be auto-generated.</param>
/// <param name="learningSpaceId">Identifier of the learning space.</param>
/// <param name="width">Width of the component in meters.</param>
/// <param name="height">Height of the component in meters.</param>
/// <param name="depth">Depth of the component in meters.</param>
/// <param name="x">X coordinate position.</param>
/// <param name="y">Y coordinate position.</param>
/// <param name="z">Z coordinate position.</param>
/// <param name="orientation">Orientation of the component.</param>
public record class CreateComponentRequest(
    string? componentId,
    string learningSpaceId,
    float width,
    float height,
    float depth,
    float x,
    float y,
    float z,
    string orientation);
