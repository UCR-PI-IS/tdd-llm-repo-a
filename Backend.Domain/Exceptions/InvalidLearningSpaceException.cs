using System;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions
{
    /// <summary>
    /// Exception thrown when an invalid learning space is accessed or referenced.
    /// </summary>
    public class InvalidLearningSpaceException : Exception
    {
        public InvalidLearningSpaceException(string message) : base(message)
        {
        }
    }
}
