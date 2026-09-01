using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Interface for the service that manages whiteboard creation.
/// </summary>
public interface IWhiteboardService
{
    /// <summary>
    /// Creates a new whiteboard with the specified parameters and persists it.
    /// </summary>
    /// <param name="componentId">Unique identifier for the whiteboard.</param>
    /// <param name="learningSpaceId">Identifier of the learning space.</param>
    /// <param name="width">Width of the whiteboard in meters.</param>
    /// <param name="height">Height of the whiteboard in meters.</param>
    /// <param name="depth">Depth of the whiteboard in meters.</param>
    /// <param name="x">X coordinate position.</param>
    /// <param name="y">Y coordinate position.</param>
    /// <param name="z">Z coordinate position.</param>
    /// <param name="orientation">Orientation of the whiteboard.</param>
    /// <param name="markerColor">Marker color of the whiteboard.</param>
    /// <returns>The created whiteboard entity.</returns>
    Task<Whiteboard> CreateWhiteboardAsync(
        string componentId,
        string learningSpaceId,
        float width,
        float height,
        float depth,
        float x,
        float y,
        float z,
        string orientation,
        string markerColor);
}
