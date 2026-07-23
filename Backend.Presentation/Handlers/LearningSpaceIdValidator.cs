namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Handlers;

internal static class LearningSpaceIdValidator
{
    public static bool IsValid(string learningSpaceId)
    {
        if (string.IsNullOrEmpty(learningSpaceId))
            return false;

        return Guid.TryParse(learningSpaceId, out var parsedId) && parsedId != Guid.Empty;
    }
}
