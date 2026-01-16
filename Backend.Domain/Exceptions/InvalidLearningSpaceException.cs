using System;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions
{
    public class InvalidLearningSpaceException : Exception
    {
        public InvalidLearningSpaceException(string message) : base(message)
        {
        }
    }
}
