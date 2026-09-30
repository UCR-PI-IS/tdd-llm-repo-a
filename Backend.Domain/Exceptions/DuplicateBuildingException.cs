namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;

/// <summary>
/// Exception thrown when attempting to add a building that already exists.
/// </summary>
public class DuplicateBuildingException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DuplicateBuildingException"/> class.
    /// </summary>
    /// <param name="buildingName">The name of the duplicate building.</param>
    public DuplicateBuildingException(string buildingName)
        : base($"Building with name '{buildingName}' already exists")
    {
    }
}
