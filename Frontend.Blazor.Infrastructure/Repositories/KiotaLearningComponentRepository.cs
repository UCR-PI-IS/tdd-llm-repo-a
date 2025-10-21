using UCR.ECCI.PI.ThemePark.Frontend.Blazor.Domain.Entities.ComponentsManagement;
using UCR.ECCI.PI.ThemePark.Frontend.Blazor.Domain.Exceptions;
using DomainValidationError = UCR.ECCI.PI.ThemePark.Frontend.Blazor.Domain.Exceptions.ValidationError;
using UCR.ECCI.PI.ThemePark.Frontend.Blazor.Application.Services;
using UCR.ECCI.PI.ThemePark.Frontend.Blazor.Infrastructure.Kiota;
using UCR.ECCI.PI.ThemePark.Frontend.Blazor.Infrastructure.Mappers;
using UCR.ECCI.PI.ThemePark.Frontend.Blazor.Domain.Repositories.ComponentsManagement;
using UCR.ECCI.PI.ThemePark.Frontend.Blazor.Infrastructure.Kiota.Models;

namespace UCR.ECCI.PI.ThemePark.Frontend.Blazor.Infrastructure.Repositories;

/// <summary>
/// Implements <see cref="ILearningComponentRepository"/> using the Kiota-generated API client
/// to perform operations on learning components such as projectors and whiteboards.
/// </summary>
internal class KiotaLearningComponentRepository : ILearningComponentRepository
{
    private readonly ApiClient _apiClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="KiotaLearningComponentRepository"/> class.
    /// </summary>
    /// <param name="apiClient">The Kiota API client used to communicate with the backend service.</param>
    public KiotaLearningComponentRepository(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    /// <summary>
    /// Adds a new learning component (projector or whiteboard) to a specific learning space.
    /// </summary>
    /// <param name="learningSpaceId">The ID of the learning space where the component will be added.</param>
    /// <param name="learningComponent">The learning component to add.</param>
    /// <returns>A task representing the asynchronous operation, containing true if successful; otherwise false.</returns>
    public async Task<bool> AddComponentAsync(int learningSpaceId, LearningComponent learningComponent)
    {
        try
        {
            if (learningComponent is Projector projector)
            {
                var dto = ProjectorNoIdDtoMapper.ToDto(projector);
                var request = new PostProjectorRequest { Projector = dto };
                await _apiClient.LearningSpaces[learningSpaceId].LearningComponent.Projector.PostAsync(request);
            }
            else if (learningComponent is Whiteboard whiteboard)
            {
                var dto = WhiteboardNoIdDtoMapper.ToDto(whiteboard);
                var request = new PostWhiteboardRequest { Whiteboard = dto };
                await _apiClient.LearningSpaces[learningSpaceId].LearningComponent.Whiteboard.PostAsync(request);
            }
            else
            {
                throw new NotSupportedException("Component type not supported.");
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error in AddComponentAsync: {ex}");
            return false;
        }
    }

    /// <summary>
    /// Deletes a learning component by its unique identifier.
    /// </summary>
    /// <param name="id">The ID of the learning component to delete.</param>
    /// <returns>A task representing the asynchronous operation, containing true if successful; otherwise false.</returns>
    public async Task<bool> DeleteComponentAsync(int id)
    {
        try
        {
            await _apiClient.LearningComponent[id].DeleteAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Retrieves all learning components associated with a specific learning space.
    /// </summary>
    /// <param name="learningSpaceId">The ID of the learning space.</param>
    /// <returns>A task representing the asynchronous operation, containing the list of learning components.</returns>
    /// <exception cref="NotFoundException">Thrown when no components are found.</exception>
    public async Task<IEnumerable<LearningComponent>> GetLearningComponentsByIdAsync(int learningSpaceId)
    {
        var response = await _apiClient.LearningSpaces[learningSpaceId].LearningComponent.GetAsync();

        var components = new List<LearningComponent>();

        if (response?.LearningComponents is null)
        {
            throw new NotFoundException("No components found.");
        }
        else if (!response.LearningComponents.Any())
        {
            return components;
        }

        foreach (var dto in response.LearningComponents)
        {
            switch (dto)
            {
                case ProjectorDto projector:
                    components.Add(ProjectorDtoMapper.ToEntity(projector));
                    break;
                case WhiteboardDto whiteboard:
                    components.Add(WhiteboardDtoMapper.ToEntity(whiteboard));
                    break;
            }
        }
        return components;
    }

    /// <summary>
    /// Retrieves all learning components in the system.
    /// </summary>
    /// <returns>A task representing the asynchronous operation, containing the list of all learning components.</returns>
    /// <exception cref="NotFoundException">Thrown when no components are found.</exception>
    public async Task<IEnumerable<LearningComponent>> GetAllAsync()
    {
        var response = await _apiClient.LearningComponent.GetAsync();

        var components = new List<LearningComponent>();

        if (response?.LearningComponents is null)
        {
            throw new NotFoundException("No components found.");
        }
        else if (!response.LearningComponents.Any())
        {
            return components;
        }

        foreach (var dto in response.LearningComponents)
        {
            switch (dto)
            {
                case ProjectorDto projector:
                    components.Add(ProjectorDtoMapper.ToEntity(projector));
                    break;
                case WhiteboardDto whiteboard:
                    components.Add(WhiteboardDtoMapper.ToEntity(whiteboard));
                    break;
            }
        }

        return components;
    }

    /// <summary>
    /// Retrieves a single learning component by its ID.
    /// </summary>
    /// <param name="id">The ID of the learning component.</param>
    /// <returns>A task representing the asynchronous operation, containing the learning component.</returns>
    /// <exception cref="NotFoundException">Thrown when the component is not found or not supported.</exception>
    Task<LearningComponent> ILearningComponentRepository.GetSingleLearningComponentAsync(int id)
    {
        return GetSingleLearningComponentInternalAsync(id);
    }

    /// <summary>
    /// Not implemented. Updates an existing learning component.
    /// </summary>
    /// <param name="learningSpaceId">The ID of the learning space associated with the component.</param>
    /// <param name="learningComponent">The updated component data.</param>
    /// <returns>Throws a <see cref="NotImplementedException"/>.</returns>
    public async Task<bool> UpdateAsync(int learningSpaceId, LearningComponent learningComponent)
    {
        try
        {
            if (learningComponent is Projector projector)
            {
                var dto = ProjectorNoIdDtoMapper.ToDto(projector);
                var request = new PutProjectorRequest
                {
                    Projector = dto
                };
                await _apiClient.LearningSpaces[learningSpaceId].LearningComponent.Projector[learningComponent.ComponentId].PutAsync(request);
            }
            else if (learningComponent is Whiteboard whiteboard)
            {
                var dto = WhiteboardNoIdDtoMapper.ToDto(whiteboard);
                var request = new PutWhiteboardRequest
                {
                    Whiteboard = dto
                };
                await _apiClient.LearningSpaces[learningSpaceId].LearningComponent.Whiteboard[learningComponent.ComponentId].PutAsync(request);
            }
            else
            {
                throw new NotSupportedException("Component type not supported.");
            }

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Retrieves a single learning component by its ID by searching all existing components.
    /// </summary>
    /// <param name="id">The ID of the learning component.</param>
    /// <returns>A task representing the asynchronous operation, containing the matching component.</returns>
    /// <exception cref="NotFoundException">Thrown when the component is not found.</exception>
    private async Task<LearningComponent> GetSingleLearningComponentInternalAsync(int id)
    {
        var response = await _apiClient.LearningComponent.GetAsync();

        if (response?.LearningComponents is null || !response.LearningComponents.Any())
            throw new NotFoundException("Learning component not found");

        foreach (var dto in response.LearningComponents)
        {
            int? componentId = dto switch
            {
                ProjectorDto projector => projector.Id,
                WhiteboardDto whiteboard => whiteboard.Id,
                _ => null
            };

            if (componentId == id)
            {
                return dto switch
                {
                    ProjectorDto projector => ProjectorDtoMapper.ToEntity(projector),
                    WhiteboardDto whiteboard => WhiteboardDtoMapper.ToEntity(whiteboard),
                    _ => throw new NotFoundException("Learning component type not supported")
                };
            }
        }

        throw new NotFoundException($"Learning component with id {id} not found");
    }
}