using System;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Exception thrown when an invalid learning space ID is provided.
/// </summary>
public class InvalidLearningSpaceException : Exception
{
    public InvalidLearningSpaceException(string message) : base(message) {}
}
