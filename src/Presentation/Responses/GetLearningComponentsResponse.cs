using UCR.ECCI.PI.ThemePark.Backend.Presentation.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Responses
{
    public class GetLearningComponentsResponse
    {
        public List<LearningComponentDto> Components { get; set; } = new List<LearningComponentDto>();
    }
}
