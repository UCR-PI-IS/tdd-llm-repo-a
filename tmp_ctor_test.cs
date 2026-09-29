using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;
public class Test {
    public static object Create() {
        return new Created<AddBuildingResponse>("/api/buildings/1", new AddBuildingResponse(null!));
    }
}
