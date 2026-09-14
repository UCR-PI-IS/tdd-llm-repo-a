namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Re-export of CreateLearningComponentHandler from the Endpoints namespace.
/// This allows tests that import the Handlers namespace to access the handler.
/// </summary>
public class CreateLearningComponentHandler : UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints.CreateLearningComponentHandler
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateLearningComponentHandler"/> class.
    /// </summary>
    /// <param name="learningComponentService">The learning component service.</param>
    public CreateLearningComponentHandler(UCR.ECCI.PI.ThemePark.Backend.Application.Services.ILearningComponentService learningComponentService)
        : base(learningComponentService)
    {
    }
}
