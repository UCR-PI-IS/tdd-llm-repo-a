using Microsoft.EntityFrameworkCore;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for whiteboard operations.
/// </summary>
internal class WhiteboardRepository : IWhiteboardRepository
{
    private readonly UCRDatabaseContext _context;

    /// <summary>
    /// Initializes a new instance of the WhiteboardRepository class.
    /// </summary>
    /// <param name="context">The database context</param>
    public WhiteboardRepository(UCRDatabaseContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task AddAsync(Whiteboard whiteboard)
    {
        await _context.Whiteboards.AddAsync(whiteboard);
    }

    /// <inheritdoc/>
    public async Task<Whiteboard?> GetByIdAsync(string id)
    {
        return await _context.Whiteboards.FindAsync(id);
    }
}
