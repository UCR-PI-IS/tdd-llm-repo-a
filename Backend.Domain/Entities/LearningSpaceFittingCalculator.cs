namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Provides calculations for determining whether a whiteboard fits inside a learning space.
/// </summary>
internal static class LearningSpaceFittingCalculator
{
    /// <summary>
    /// Determines whether the specified whiteboard fits within the given learning space.
    /// </summary>
    /// <param name="whiteboard">The whiteboard to check.</param>
    /// <param name="learningSpace">The learning space to check against.</param>
    /// <returns>True if the whiteboard fits; otherwise, false.</returns>
    public static bool FitsInSpace(Whiteboard whiteboard, LearningSpace learningSpace)
    {
        if (whiteboard.Width > learningSpace.Width)
            return false;

        if (whiteboard.Height > learningSpace.Height)
            return false;

        if (whiteboard.Depth > learningSpace.Length)
            return false;

        if (whiteboard.X + whiteboard.Width > learningSpace.Width)
            return false;

        if (whiteboard.Y + whiteboard.Height > learningSpace.Height)
            return false;

        if (whiteboard.Z + whiteboard.Depth > learningSpace.Length)
            return false;

        return true;
    }
}
