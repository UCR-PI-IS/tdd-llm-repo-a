using System;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Exception thrown when a learning space is invalid or not found.
/// </summary>
public class InvalidLearningSpaceException : Exception
{
    public InvalidLearningSpaceException(string message) : base(message) {}
}
