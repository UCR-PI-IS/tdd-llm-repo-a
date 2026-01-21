using System;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions
{
    /// <summary>
    /// Exception thrown when a learning space id is invalid.
    /// </summary>
    public class InvalidLearningSpaceException : Exception
    {
        public InvalidLearningSpaceException()
        {
        }

        public InvalidLearningSpaceException(string message) : base(message)
        {
        }

        public InvalidLearningSpaceException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
